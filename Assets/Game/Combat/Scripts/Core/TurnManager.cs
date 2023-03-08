using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Option;
using static Option.Option;
using static Result;
using Err_ = IdentifierNotFoundError;

public delegate void VoidHook();

public class IdentifierNotFoundError : Error
{
    public override void Present()
    {
        Debug.Log("Identifier not found");
    }
}

public class TurnManager : MonoBehaviour
{
    enum TurnState 
    {
        Start,
        Player,
        Enemy,
        Finished,
    }

    [SerializeField] CameraController _camera;
    [SerializeField] float _delayAfterTurn;
    [SerializeField] float _delayAfterTurnSequence;
    [SerializeField] LayerMask _focusableMask;
    
    List<(int, XcomPlayerController)> _party;
    List<(int, XcomEnemyController)> _enemies;
    GameObject _partyContainer;
    GameObject _enemyContainer;
    TurnState _turnState;


    List<(int, XcomPlayerController)> _partyCandidates;
    int currentPlayerOnFocus;
    bool _turnInProgress;

    // Start is called before the first frame update
    void Awake()
    {
        _party = new List<(int, XcomPlayerController)>();
        _enemies = new List<(int, XcomEnemyController)>();

        _partyContainer = new GameObject("Player Party Container");
        _enemyContainer = new GameObject("Enemy Party Container");

        _turnInProgress = false;
        currentPlayerOnFocus = 0;
        _partyCandidates = new List<(int, XcomPlayerController)>();

        _turnState = TurnState.Finished;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCameraFocus();
        if (_turnState == TurnState.Finished)
            CommenceTurns(() => {Debug.Log("Finished Turn!");});
    }

    public void CommenceTurns(VoidHook fn)
    {
        StartCoroutine(PlayTurns(fn));
    }

    IEnumerator PlayTurns(VoidHook fn)
    {
        _turnState = TurnState.Start;

        yield return PartyTurn();
        Debug.Log("ENEMY TURN TIME!!!!");
        yield return EnemyTurn();

        _turnState = TurnState.Finished;

        fn();

        Debug.Assert(_partyCandidates.Count == 0);

        yield return new WaitForSeconds(_delayAfterTurnSequence);
        Debug.Log("ALLLL TURNSS OVERRRRRRRRRR!!!!");
    }

    IEnumerator PartyTurn()
    {
        Debug.Log("Starting player turn");

       _turnState = TurnState.Player;

       _partyCandidates = new List<(int, XcomPlayerController)>(_party);

       currentPlayerOnFocus = 0;
       _camera.JumpFocusTo(_partyCandidates[currentPlayerOnFocus].Item2.transform);

       while (_partyCandidates.Count > 0)
       {

            int currIdx = currentPlayerOnFocus;
            var (id, controller) = _partyCandidates[currIdx];

            Debug.Log("Selected " + currIdx);
            controller.Select();

            bool tryingTurn = true;
            Option<List<Action>> todo = None<List<Action>>();

            controller.Select();

            controller.TryTurn(actions => {
                tryingTurn = false;
                todo = Some<List<Action>>(actions);
            });

            yield return new WaitUntil(() => currIdx != currentPlayerOnFocus 
                                             || !tryingTurn);

            controller.DeSelect();

            //if we switched while we were still querying the turn, continue
            if ((currIdx != currentPlayerOnFocus && tryingTurn) || todo.IsNone()) continue;

            _turnInProgress = true;

            controller.TakeTurn(todo.Unwrap());

            yield return new WaitUntil(() => controller.IsTurnOver());
            yield return new WaitForSeconds(_delayAfterTurn);

            Debug.Log("It's all over!");
            _partyCandidates.Remove((id, controller));

            if (_partyCandidates.Count != 0)
            {
                Debug.Log("BEFORE!!! " + id + " " + currentPlayerOnFocus);
                currentPlayerOnFocus %= _partyCandidates.Count;
                Debug.Log("AFTER!!! " + id + " " + currentPlayerOnFocus);
                _camera.JumpFocusTo(_partyCandidates[currentPlayerOnFocus].Item2.transform);
            }

            _turnInProgress = false;
       }
    }

    IEnumerator EnemyTurn()
    {
        Debug.Log("Starting enemy turn");

        _turnState = TurnState.Enemy;

       foreach (var (_, controller) in _enemies)
       {
            List<Action> actionPool = controller.GetActions();

            controller.TakeTurn(actionPool);

            _turnInProgress = true;

            yield return new WaitUntil(() => controller.IsTurnOver());
            yield return new WaitForSeconds(_delayAfterTurn);

            _turnInProgress = false;
       }
    }

    Result<T, Err_> GetIdFromList<T>(int id, List<(int, T)> ls)
    {
         Result<T, Err_> ret = Err<T, Err_>(new Err_());

         ls.ForEach(elem => {
            var (id_, c) = elem;

            if (id_.Equals(id))
            {
                if (ret.IsErr())
                    ret = Ok<T, Err_>(c);
                else
                {
                    string type = typeof(T).ToString() == "XcomPlayerController" ? "party" : "enemy";
                    throw new System.Exception($"Error in GetIdFromList(id, {type})." +
                                                "Each element should have a unique identifier");
                }
                   
            }
         });

         return ret;

    }

    Result<XcomPlayerController, Err_> GetPartyMemberWith(int id)
    {
        return GetIdFromList(id, _party);
    }

    Result<XcomEnemyController, Err_> GetEnemyWith(int id)
    {
        return GetIdFromList(id, _enemies);
    }

    public void SpawnPlayer(int id, GameObject prefab, Vector2Int pos, Quaternion rotation)
    {
        Vector3 position = TileMaster.FromIsometricBasis(pos);
        GameObject ret = Instantiate(prefab, position, rotation);
        _party.Add((id, ret.GetComponent<XcomPlayerController>()));
        ret.transform.parent = _partyContainer.transform;
    }

    public void SpawnEnemy(int id, GameObject prefab, Vector2Int pos, Quaternion rotation)
    {
        Vector3 position = TileMaster.FromIsometricBasis(pos);
        GameObject ret = Instantiate(prefab, position, rotation);
        _enemies.Add((id, ret.GetComponent<XcomEnemyController>()));
        ret.transform.parent = _enemyContainer.transform;
    }

    void UpdateCameraFocus()
    {
        //TODO check if user double clicks or whatever on a character controller during the player's turn
        //TODO do some extra checks so that currentPlayerOnFocus is never out of bounds
        if (_turnState != TurnState.Player || _turnInProgress || _partyCandidates.Count == 0) return;

        //short circuit only ever calls one
        bool _ = !_camera.IsJumpingFocus() && (ScrollFocus() || ClickFocus());
      
    }

    bool ScrollFocus()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            currentPlayerOnFocus = (currentPlayerOnFocus + _partyCandidates.Count - 1) % _partyCandidates.Count;
            Debug.Log("Index left " + currentPlayerOnFocus);
            var (_, controller) = _partyCandidates[currentPlayerOnFocus];
            _camera.JumpFocusTo(controller.gameObject.transform);

            return true;
        }
        else if (Input.GetKeyDown(KeyCode.P))
        {
            currentPlayerOnFocus = (currentPlayerOnFocus + 1) % _partyCandidates.Count;
            var (_, controller) = _partyCandidates[currentPlayerOnFocus];
            _camera.JumpFocusTo(controller.gameObject.transform);

            return true;
        }

        return false;

    }

    bool ClickFocus()
    {
        return false; //temporary, check if double click
        //TODO check if double click
        Vector3 mouseCoords = Input.mousePosition;
        Vector3 worldMouseCoords = _camera.Camera.ScreenToWorldPoint(mouseCoords);

        RaycastHit2D hit = Physics2D.Raycast(worldMouseCoords, Vector2.zero, _focusableMask);

        #nullable enable
        if (hit)
        {
            XcomPlayerController? c = hit.collider.gameObject.GetComponent<XcomPlayerController>();

            if (c == null) return false;

            for (int i = 0; i < _partyCandidates.Count; i++)
            {
                var (_, controller) = _partyCandidates[i];
                if (c == controller)
                {
                    currentPlayerOnFocus = i;
                    _camera.JumpFocusTo(c.gameObject.transform);
                    return true;
                }
            }

           
        }
        #nullable disable

        return false;
    }
}
