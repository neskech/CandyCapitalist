using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;

enum TileMapType 
{
    BaseLayer,
    Collider,
    Decoration,
    NUM_CLASSES
}

public class TileMaster : MonoBehaviour
{
    [SerializeField] int _tileDimensions;
    [SerializeField] bool _drawGizmos;
    [SerializeField] bool _autoUpdate;
    [SerializeField] Vector2Int _colliderHeightRange;


    Dictionary<TileMapType, Tilemap> _maps;
    TileData[,] _tiles;
    Grid _grid;
    Vector3 _gridUp;
    Vector3 _gridRight;

    // Start is called before the first frame update
    void OnValidate()
    {
        _maps = new Dictionary<TileMapType, Tilemap>();
        _grid = GetComponent<Grid>();

        if (_autoUpdate || _drawGizmos)
             Setup();
    }

    public void Setup()
    {
        SetupTileMaps();
        GetGridDirections();
        SetupTileArray();
    }

    void GetGridDirections()
    {
        Tilemap baseLayer = _maps[TileMapType.BaseLayer];

        Vector2 p1 = baseLayer.CellToWorld(new Vector3Int(0, 0, 0));
        Vector2 p2 = baseLayer.CellToWorld(new Vector3Int(1, 0, 0));
        Vector2 p3 = baseLayer.CellToWorld(new Vector3Int(0, 1, 0));

        _gridUp = (p3 - p1).normalized;
        _gridRight = (p2 - p1).normalized;
    }

    void SetupTileMaps()
    {
        foreach (Transform child in transform)
        {
            Tilemap map = child.GetComponent<Tilemap>();
            if (map != null)
            {
                switch (child.name.ToLower()) {
                    case "baselayer":
                        _maps[TileMapType.BaseLayer] = map;
                        break;
                    case "collider":
                        _maps[TileMapType.Collider] = map;
                        break;
                    case "decoration":
                        _maps[TileMapType.Decoration] = map;
                        break;
                    default: 
                        break;
                }
            }
        }

        if (_maps.Count != (int) TileMapType.NUM_CLASSES)
            throw new System.Exception($"Required tile maps not met. Current are {_maps}");
    }
 
    void SetupTileArray()
    {
        Tilemap baseLayer = _maps[TileMapType.BaseLayer];
        BaseLayout script = baseLayer.GetComponent<BaseLayout>();
        int width = script.GridWidth; 
        int height = script.GridHeight;

        if (width % _tileDimensions != 0 || height % _tileDimensions != 0)
            throw new System.Exception("Width and Height of base layer must be divisble" +
                                        "by tile dimensions");

        Tilemap colliderMap = _maps[TileMapType.Collider];
        _tiles = new TileData[ height / _tileDimensions, width / _tileDimensions];
 
        for (int h = 0; h <= height - _tileDimensions; h += _tileDimensions) 
        {
            for (int w = 0; w <= width - _tileDimensions; w += _tileDimensions)
            {

                //iterate over the nxn area, checking for any blocker tiles
                bool isBlocker = false;
                for (int i = 0; i < _tileDimensions; i++) 
                {
                    for (int j = 0; j < _tileDimensions; j++)
                    {
                        for (int z = _colliderHeightRange.x; z <= _colliderHeightRange.y; z++)
                        {

                            Vector3Int tilePos = new Vector3Int(h + i, w + j, z);
                            if (colliderMap.HasTile(tilePos)) 
                            {
                                isBlocker = true;
                                goto Loopend;
                            }
                        } 
                       
                    }
                }

                Loopend:
                    Vector3Int tilePos_ = new Vector3Int(h, w, BaseLayout.BASE_LAYER_HEIGHT);
                    Vector3 shift = _gridUp * baseLayer.cellSize.x / 2 + _gridRight * baseLayer.cellSize.y;
                    Vector3 topLeftPos = baseLayer.CellToWorld(tilePos_) + shift;

                    TileData data = new TileData(topLeftPos, isBlocker ? TileType.Blocker : TileType.Free);
                    _tiles[h / _tileDimensions, w / _tileDimensions] = data;
                

            }
        }

    }

    private void OnDrawGizmos() {
        if (!_drawGizmos) return;
        if (!_maps.ContainsKey(TileMapType.BaseLayer)) return;
        if (_tiles.GetLength(0) < 1 || _tiles.GetLength(1) < 1) return;

        Tilemap baseLayer = _maps[TileMapType.BaseLayer];

        BaseLayout script = _maps[TileMapType.BaseLayer].GetComponent<BaseLayout>();
        int width = script.GridWidth;
        int height = script.GridHeight;

        //construct a quad 
        TileData d1 = _tiles[0, 0];
        TileData d2 = _tiles[1, 0];
        TileData d3 = _tiles[0, 1];

        Vector3 forwards = -(d2.worldPos - d1.worldPos);
        Vector3 right = (d3.worldPos - d1.worldPos);
        Vector3 center = (-forwards + right) / 2;
        Mesh quad = Mesher.CreateQuadMesh(-_gridUp, _gridRight);

        for (int h = 0; h < _tiles.GetLength(0); h++)
        {
            for (int w = 0; w < _tiles.GetLength(1); w++)
            {
                Gizmos.color = _tiles[h, w].IsClear() ? Color.white : Color.red;
                Vector3 pos = new Vector3(_tiles[h, w].worldPos.x, _tiles[h, w].worldPos.y, 10);
                Gizmos.DrawSphere(pos, 0.07f);
 
                if (h < _tiles.GetLength(0) && w < _tiles.GetLength(1))
                {
                    Gizmos.DrawWireMesh(quad, pos);
                    Gizmos.DrawCube(center + pos, new Vector3(0.4f, 0.4f, 0.1f));
                }
                   
                   
            }
        }
    }
}
