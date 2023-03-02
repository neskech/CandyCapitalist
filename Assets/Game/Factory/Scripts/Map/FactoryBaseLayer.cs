using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;

public class FactoryBaseLayer : MonoBehaviour
{
    //exposed
    [SerializeField] int _gridWidth;
    [SerializeField] int _gridHeight;
    [SerializeField] Tile _baseLayerTile;

    //properties
    public int gridWidth {get => _gridWidth;}
    public int gridHeight {get => _gridHeight;}
    public BoundsInt mapBounds {get => _map.cellBounds;}
    public Vector2 gridBasisUp {get => new Vector2(_gridBasisUp.x, _gridBasisUp.y) / 2;}
    public Vector2 gridBasisRight {get => new Vector2(_gridBasisRight.x, _gridBasisRight.y) / 2;}
    public Tilemap baseLayer => _map;

    //const
    public const int BASE_LAYER_HEIGHT = 0;
    
    //vars
    public bool autoUpdate;
    Vector2 _gridBasisUp;
    Vector2 _gridBasisRight;
    Tilemap _map;

    private void OnValidate() {
        _map = GetComponent<Tilemap>();
        if (autoUpdate)
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

    public Vector2 ToIsometricBasis(Vector2 world) 
    {
        Vector2 v1 = _gridBasisRight;
        Vector2 v2 = _gridBasisUp;

        float determinant = 1 / (v1.x * v2.y - v2.x * v1.y);
        return determinant * new Vector2(v2.y * world.x - v2.x * world.y, 
                                        -v1.y * world.x + v1.x * world.y);
    }

    public Vector2 FromIsometricBasis(Vector2 iso)
    {
        return iso.x * _gridBasisRight + iso.y * _gridBasisUp;
    }

}
