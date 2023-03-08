using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;
using Option;
using static Option.Option;
using System;

public delegate void PlayerControllerHook(System.Collections.Generic.List<Action> actions);

public class XcomPlayerController : MonoBehaviour
{
    //exposed
    [SerializeField] LayerMask _entityMask;
    [SerializeField] KeyCode _selectionKey;
    [SerializeField] Tile _overlayTile;

    //vars
    XcomEntityStateMachine _stateMachine;
    OverlayUI _overlayUI;

    EntityStats _stats;
    Weapon _weapon;
    Option<PlayerControllerHook> _hook;

    bool _isSelected;

    // Start is called before the first frame update
    void Start()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Overlay Layer");
        Debug.Assert(objects.Length == 1);
        Tilemap overlay = objects[0].GetComponent<Tilemap>();

        _overlayUI = new OverlayUI(overlay, _overlayTile);
        _hook = None<PlayerControllerHook>();

        StateMachineConfig config = new StateMachineConfig(_stats, _weapon);
        _stateMachine = new XcomEntityStateMachine(config, StartCoroutine, GetComponent<Animator>());

    }


    void Update()
    {
        if (_isSelected)
        {
            Debug.Assert(_hook.IsSome());
            Vector2Int transformTileCoords = TileMaster.WorldToCellInt(transform.position).xy2D();
            _overlayUI.Update(transformTileCoords);
            CheckAction();
        }

        if (_stateMachine.IsTurnPlaying())
            _stateMachine.Update(transform);
    }

    void CheckAction()
    {
        if (!Input.GetKeyDown(_selectionKey)) return;

        //Handle case of attacking an enemy
        if (HandleEntitySelected())
            return;

        //If not handle case of moving to a different tile
        HandleTileSelected();
    }

    bool HandleEntitySelected()
    {
        Option<GameObject> entity = EntityAtMousePos();
        if (entity.IsSome())
        {
            GameObject ent = entity.Unwrap();

            if (IsEnemy(ent))
            {
                XcomEnemyController c = ent.GetComponent<XcomEnemyController>();
                Vector2Int pos = TileMaster.WorldToCellInt(c.transform.position).xy2D();

                Action act = new Action.Attack(c.TakeDamage, pos);
                List<Action> actions = new List<Action>(){act};
                _hook.Unwrap()(actions);
                return true;
            }
        }

        return false;
    }

    void HandleTileSelected()
    {
        Vector2Int tileTransform = TileMaster.ToIsometricBasis(transform.position).CeilToInt();
        Vector3 mousePos = CameraController.Instance.Camera.ScreenToWorldPoint(Input.mousePosition);
        Vector2Int tileMouse = TileMaster.ToIsometricBasis(mousePos).CeilToInt();

        //make sure both tile coordinates are in bounds of base layer
        BoundsInt bounds = TileMaster.Instance.CellBounds; 
        if (tileTransform.x < bounds.xMin || tileTransform.x >= bounds.xMax ||
            tileTransform.y < bounds.yMin || tileTransform.y >= bounds.yMax)
            return;

        if (tileMouse.x < bounds.xMin || tileMouse.x >= bounds.xMax ||
            tileMouse.y < bounds.yMin || tileMouse.y >= bounds.yMax)
            return;

        //generate a path from tileTransform to tilMouse
        Result<List<Vector2Int>, PathNotFoundError> result = AStar.FindPath(
                                            map: TileMaster.Instance.TileMapGrid, 
                                            start: tileTransform,
                                            end: tileMouse
                                        );


        if (result.IsErr()) return;

        List<Vector2Int> path = result.Unwrap();

        foreach (var v in path)
            Debug.Log(v);

        //if the path exists, make an action from it and call the hook
        List<Action> actions = ActionPoolFromPath(path);
        _hook.Unwrap()(actions);
    }

    List<Action> ActionPoolFromPath(List<Vector2Int> path)
    {
        List<Action> actions = new List<Action>();

        /*
            First element of path is always the current
            position of the player
        */
        for (int i = 0; i < path.Count; i++)
        {
            actions.Add(new Action.Walk(path[i]));
        }

        return actions;
    }

    public void TryTurn(PlayerControllerHook fn)
    {
        _hook = Some<PlayerControllerHook>(fn);
    }

    public void TakeTurn(List<Action> actions)
    {
        _stateMachine.EnactActionPool(actions, transform);
    }   

    public void TakeDamage(float damage)
    {

    }

    public void Select() 
    {
        _isSelected = true;
    }
    
    public void DeSelect() 
    {
        _isSelected = false;
        _hook = None<PlayerControllerHook>();
    }

    public bool IsTurnOver() => _stateMachine.IsTurnOver();

    Option<GameObject> EntityAtMousePos()
    {
        Vector2 origin = CameraController.Instance.Camera.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.zero, _entityMask);

        if (hit)
            return Some<GameObject>(hit.collider.gameObject);
        return None<GameObject>();
    }

    bool IsPartyMember(GameObject obj) => obj.GetComponent<XcomPlayerController>() != null;
    bool IsEnemy(GameObject obj) => obj.GetComponent<XcomEnemyController>() != null;
}
