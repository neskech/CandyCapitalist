using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;
using Option;
using static Option.Option;

public class OverlayUI 
{
    Tilemap _overlayLayer;
    Tile _overlayTile;
    List<Vector2Int> _cachedPath; 
    Vector2Int _prevMouseTileCoords;

    public OverlayUI(Tilemap overlayLayer, Tile overlayTile)
    {
        _overlayLayer = overlayLayer;
        _prevMouseTileCoords = new Vector2Int();
        _overlayTile = overlayTile;
    }

    public void Update(Vector2Int transformTileCoords)
    {
        if (UpdateMouseTileCoords())
        {
            ClearOverlayLayer();
            DrawOverlayTile(_prevMouseTileCoords);
            //DrawPath();
            //DrawAreaOfAvailability();
        }
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

    void DrawOverlayTile(Vector2Int mouseCoordsTile)
    {
        //query the z dictionary for the full Vector3Int
        Vector3Int tileCoords = new Vector3Int(mouseCoordsTile.x,
                                               mouseCoordsTile.y,
                                               TileMaster.zMap[mouseCoordsTile] - 1);
        if (tileCoords.z == 0)
            tileCoords.z = 1;
        _overlayLayer.SetTile(tileCoords.yxz(), _overlayTile);
        //Debug.Log(tileCoords);
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

        Vector2Int newMouseTile = GetMouseTileCoords();
        if (newMouseTile != _prevMouseTileCoords)
        {
            _prevMouseTileCoords = newMouseTile;
            return true;
        }

        return false;
    }

    Vector2Int GetMouseTileCoords()
    {
        Vector3 mouseCords = Input.mousePosition;
        Vector3 worldPos = CameraController.Instance.Camera.ScreenToWorldPoint(mouseCords);
        Vector3 tilePos = TileMaster.ToIsometricBasis(worldPos);
        return new Vector2Int(Mathf.CeilToInt(tilePos.x), Mathf.CeilToInt(tilePos.y));
        //return GetTileAtCoords(worldPos);
      //  Vector3 tileCords = _overlayLayer.WorldToCell(worldPos);
        //TODO want to find the tile with the HIGHEST Z coordinate given some (x, y) coordinates
        //TODO can either loop through vertically on all of tilemaster's tilemaps
        //TODO or can have tilemaster store a dict of the highest z tileas at each (x, y)

        //TODO should also repurpose this method
        //TODO have a function that gives the mouse in tile coords and another
        //TODO function that grabs a tile at that tile coord
    }
}
