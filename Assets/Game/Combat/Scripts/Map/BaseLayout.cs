using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;

public class BaseLayout : MonoBehaviour
{
    [SerializeField] int _gridWidth;
    [SerializeField] int _gridHeight;
    [SerializeField] Tile _baseLayerTile;

    public int GridWidth {get => _gridWidth;}
    public int GridHeight {get => _gridHeight;}
    public BoundsInt MapBounds {get => _map.cellBounds;}
    public const int BASE_LAYER_HEIGHT = 0;
    
    public bool AutoUpdate;

    Tilemap _map;

    private void OnValidate() {
        _map = GetComponent<Tilemap>();
        if (AutoUpdate)
             CreateTileMap();
    }

    public void CreateTileMap()
    {
        for (int h = 0; h < _gridHeight; h++) 
        {
            for (int w = 0; w < _gridWidth; w++)
            {
                 _map.SetTile(new Vector3Int(h, w, BASE_LAYER_HEIGHT), _baseLayerTile);
            }
        }
    }

    public void ResetTileMap()
    {
        _map.ClearAllTiles(); 
    }

}
