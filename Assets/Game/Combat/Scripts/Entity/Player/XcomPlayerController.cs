using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;
using Option;
using static Option.Option;

public class XcomPlayerController : MonoBehaviour, IXcomCharacterController
{
    //vars
    OverlayUI _overlayUI;
    Vector2Int _transformTileCoords;
    XcomEntityStateMachine _stateMachine;
    EntityStats _stats;
    Weapon _weapon;

    //TODO have a ref to the state machine 
    //TODO and ability to click and move around with A*
    // Start is called before the first frame update
    void Start()
    {
        //TODO create an overlay tilemap to store overlay tiles
        //TODO want to take advantage of batch rendering
        
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Overlay Layer");
        Debug.Assert(objects.Length == 1);
        Tilemap overlay = objects[0].GetComponent<Tilemap>();

        _overlayUI = new OverlayUI(overlay);

        StateMachineConfig config = new StateMachineConfig(_stats, _weapon);
        _stateMachine = new XcomEntityStateMachine(transform, config);

    }

    // Update is called once per frame
    void Update()
    {
        _overlayUI.Update(_transformTileCoords);
    }

    Option<List<Vector2Int>> FindPathFromTo(Vector2Int from, Vector2Int to)
    {
        //TODO use the A* algo to find a path from 'from' to 'to'
        //TODO if a path doesn't exist return none
        return None<List<Vector2Int>>();

        //TODO after retrieving the path, the state machine should have a 'follow path' function
        //TODO that walks the player along the path
    }

    //Interface methods
    void UpdateTransformTileCoord()
    {
        //TODO update the _transformTileCoord property 
    }

    public void EnterCoroutine(IEnumerator coroutine)
    {
        StartCoroutine(coroutine);
    }

    public Vector2Int GetPosition()
    {
        return new();
    }

    public void TakeDamage(float damage)
    {

    }

}
