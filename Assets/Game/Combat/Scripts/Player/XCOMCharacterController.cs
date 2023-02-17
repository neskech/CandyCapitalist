using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;
using Option;
using static Option.Option;

public class XCOMCharacterController : MonoBehaviour
{
    //exposed
    [SerializeField] Tilemap _overlayLayer;
    [SerializeField] Heuristic _AStarHeuristic;
    [SerializeField] int _maxZDelta; 
    [SerializeField] bool _allowDiagonals;

    //vars
    Vector2Int _prevMouseTileCoords;
    Vector2Int _transformTileCoords;

    //TODO have a ref to the state machine 
    //TODO and ability to click and move around with A*
    // Start is called before the first frame update
    void Start()
    {
        //TODO create an overlay tilemap to store overlay tiles
        //TODO want to take advantage of batch rendering
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    Option<List<Vector2Int>> FindPathFromTo(Vector2Int from, Vector2Int to)
    {
        //TODO use the A* algo to find a path from 'from' to 'to'
        //TODO if a path doesn't exist return none
        return None<List<Vector2Int>>();

        //TODO after retrieving the path, the state machine should have a 'follow path' function
        //TODO that walks the player along the path
    }

    void DrawPath(List<Vector2Int> path)
    {
        //TODO draw a line path from start to finish, specified by he path var
        //TODO should be drawn on the overlay map
    }

    void DrawAreaOfAvailability(List<(bool, Vector2Int)> availables)
    {
        //TODO draw a collection of overlay tiles around the player
        //TODO a WHITE overlay tile denotes the player can move there
        //TODO a RED overlay tile denotes the player CANNOT move there
    }

    void ClearOverlayLayer()
    {
        //TODO the overlay layer should be cleared every time the user's mouse changes
        //TODO to a new mouse position. This is so we can redraw the overlay tiles in a
        //TODO new position

        //This will clear the area of availbility as well. Thus we're gonna have to redraw it
        //Don't know if there's a better solution
        _overlayLayer.ClearAllTiles();
    }

    bool UpdateMouseTileCoords()
    {
        //TODO we should only update the path and the overlay map when our tile coordinate
        //TODO has changed. This function should check the current tile coordinate against
        //TODO the previous tile coordinate of the mouse and return true if it has changed,
        //TODO updating the previous tile coordinate in the processs
        return true;
    }

    Option<Tile> GetTileAtMouseCords()
    {
        Vector3 mouseCords = Input.mousePosition;
        Vector2 worldPos = CameraController.Instance.Camera.ScreenToWorldPoint(mouseCords);
        Vector3 tileCords = _overlayLayer.WorldToCell(worldPos);
        //TODO want to find the tile with the HIGHEST Z coordinate given some (x, y) coordinates
        //TODO can either loop through vertically on all of tilemaster's tilemaps
        //TODO or can have tilemaster store a dict of the highest z tileas at each (x, y)

        //TODO should also repurpose this method
        //TODO have a function that gives the mouse in tile coords and another
        //TODO function that grabs a tile at that tile coord

        return None<Tile>();
    }

    void UpdateTransformTileCoord()
    {
        //TODO update the _transformTileCoord property 
    }
}
