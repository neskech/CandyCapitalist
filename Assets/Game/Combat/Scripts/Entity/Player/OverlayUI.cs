using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;
using Option;
using static Option.Option;

public class OverlayUI 
{
    Vector2Int _prevMouseTileCoords;
    Tilemap _overlayLayer;
    //don't want to do A* every frame. Only do when mousetilepos moves
    List<Vector2Int> _cachedPath; 
    public OverlayUI(Tilemap overlayLayer)
    {
        _overlayLayer = overlayLayer;
        _prevMouseTileCoords = new Vector2Int();
    }

    public void Update(Vector2Int transformTileCoords)
    {

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

    Option<Tile> GetTileAtMouseCoords()
    {
        Vector3 mouseCords = Input.mousePosition;
        Vector2 worldPos = CameraController.Instance.Camera.ScreenToWorldPoint(mouseCords);
        //return GetTileAtCoords(worldPos);
      //  Vector3 tileCords = _overlayLayer.WorldToCell(worldPos);
        //TODO want to find the tile with the HIGHEST Z coordinate given some (x, y) coordinates
        //TODO can either loop through vertically on all of tilemaster's tilemaps
        //TODO or can have tilemaster store a dict of the highest z tileas at each (x, y)

        //TODO should also repurpose this method
        //TODO have a function that gives the mouse in tile coords and another
        //TODO function that grabs a tile at that tile coord

        return None<Tile>();
    }
}
