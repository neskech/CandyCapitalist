using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;
using Option;
using static Option.Option;

enum TileMapType 
{
    BaseLayer,
    Collider,
    Decoration,
    NUM_CLASSES
}

public class TileMaster : MonoBehaviour
{
    //singleton
    static TileMaster _instance;
    public static TileMaster Instance { get => _instance; }

    //exposed vars
    [SerializeField] int _tileDimensions;
    [SerializeField] bool _drawGizmos;
    [SerializeField] bool _autoUpdate;


    //private vars
    Tilemap[] _maps;
    TileData[,] _tiles;
    Grid _grid;
    Vector2 _gridBasisUp;
    Vector2 _gridBasisRight;


    //properties 
    //Positive grid right is left, positive grid up is up
    public Vector2 GridBasisUp {get => new Vector2(_gridBasisUp.x, _gridBasisUp.y) / 2;}
    public Vector2 GridBasisRight {get => new Vector2(_gridBasisRight.x, _gridBasisRight.y) / 2;}
    public Vector3 CellSize {get => _maps[(int)TileMapType.BaseLayer].cellSize;}
    public BoundsInt CellBounds {get => _maps[(int)TileMapType.BaseLayer].cellBounds;}
    public int TileDimensions {get => _tileDimensions;}
    public Vector3Int WorldToCellInt(Vector3 world) => Instance._maps[(int)TileMapType.BaseLayer].WorldToCell(world);
    public TileData[,] TileMapGrid => _tiles;


    TileMaster() 
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
        }
        else 
        {
            _instance = this;
        }
    }

    void OnValidate()
    {
        _maps = new Tilemap[(int)TileMapType.NUM_CLASSES];
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

    #region setup

    void SetupTileMaps()
    {
        int count = 0;
        foreach (Transform child in transform)
        {
            Tilemap map = child.GetComponent<Tilemap>();
            if (map != null)
            {
                switch (child.name.ToLower()) {
                    case "baselayer":
                        _maps[(int)TileMapType.BaseLayer] = map;
                        break;
                    case "collider":
                        _maps[(int)TileMapType.Collider] = map;
                        break;
                    case "decoration":
                        _maps[(int)TileMapType.Decoration] = map;
                        break;
                    default: 
                        break;
                }
                count++;
            }
        }

        if (count < (int) TileMapType.NUM_CLASSES)
            throw new System.Exception($"Required tile maps not met. Current are {_maps}");
    }

    void GetGridDirections()
    {
       _gridBasisUp = CellSize.xy();
       _gridBasisRight = new Vector2(-CellSize.x, CellSize.y);
    }
 
    void SetupTileArray()
    {
        Tilemap baseLayer = _maps[(int)TileMapType.BaseLayer];
        BaseLayout script = baseLayer.GetComponent<BaseLayout>();

        int width = script.GridWidth; 
        int height = script.GridHeight;

        //make sure the width and height actually paritions the tilemap
        if (width % _tileDimensions != 0 || height % _tileDimensions != 0)
            throw new System.Exception("Width and Height of base layer must be divisble" +
                                        "by tile dimensions");

        //make sure the base layer is completely flat
        if (baseLayer.cellBounds.z == 1)
            throw new System.Exception("Base layer must be completely flat " +
                                        $"Has min z: {baseLayer.cellBounds.zMin} and max z:" + 
                                        $"{baseLayer.cellBounds.zMax}");

        _tiles = new TileData[height / _tileDimensions, width / _tileDimensions];

        SetupBaseLayer(baseLayer, width, height);
        Tilemap colliderLayer = _maps[(int)TileMapType.Collider];
        SetupColliderLayer(colliderLayer, width, height);

    }

    void SetupBaseLayer(Tilemap baseLayer, int width, int height)
    {

        for (int h = 0; h <= height - _tileDimensions; h += _tileDimensions) 
        {
            for (int w = 0; w <= width - _tileDimensions; w += _tileDimensions)
            {
                //TODO do some chunking, shit is wrong
                 Vector3Int idx = new Vector3Int(h, w, BaseLayout.BASE_LAYER_HEIGHT);
                 Vector2 pos = FromIsometricBasis(new Vector2(w, h)) + 
                               FromIsometricBasis(new Vector2(1, 1)) / 2;

                 TileType isBlocker = baseLayer.HasTile(idx) ? TileType.Free : TileType.Blocker;
                 _tiles[h, w] = new TileData(pos, idx, isBlocker);
            }
                       
        } 

    }

    void SetupColliderLayer(Tilemap colliderLayer, int width, int height)
    {
        BoundsInt bounds = colliderLayer.cellBounds;

        for (int z = bounds.min.z; z < bounds.max.z; z++)
        {
            for (int h = 0; h <= height - _tileDimensions; h += _tileDimensions) 
            {
                for (int w = 0; w <= width - _tileDimensions; w += _tileDimensions)
                {
                    for (int i = 0; i < _tileDimensions; i++) 
                    {
                        for (int j = 0; j < _tileDimensions; j++)
                        {

                                Vector3Int idx = new Vector3Int(h + i, w + j, z);
                                if (colliderLayer.HasTile(idx))
                                {
                                    _tiles[h / _tileDimensions, w / _tileDimensions].SwitchToBlocker();
                                } 
                        
                        }
                    }

                }
                        
            } 
        }
    }

    #endregion

    //transformations
    public static Vector2 ToIsometricBasis(Vector2 world) 
    {
        Vector2 v1 = Instance.GridBasisRight;
        Vector2 v2 = Instance.GridBasisUp;

        float determinant = 1 / (v1.x * v2.y - v2.x * v1.y);
        return determinant * new Vector2(v2.y * world.x - v2.x * world.y, 
                                        -v1.y * world.x + v1.x * world.y);
    }

    public static Vector2 FromIsometricBasis(Vector2 iso)
    {
        return iso.x * Instance.GridBasisRight + iso.y * Instance.GridBasisUp;
    }

    private void OnDrawGizmos() {
        if (!_drawGizmos) return;
        if (_maps[(int)TileMapType.BaseLayer] == null) return;
        if (_tiles.GetLength(0) < 1 || _tiles.GetLength(1) < 1) return;

        for (int h = 0; h < _tiles.GetLength(0); h++)
        {
            for (int w = 0; w < _tiles.GetLength(1); w++)
            {
                Gizmos.color = _tiles[h, w].IsClear() ? Color.white : Color.red;
                Vector3 pos = new Vector3(_tiles[h, w].BottomRight().x, _tiles[h, w].BottomRight().y, 10);
                Gizmos.DrawSphere(_tiles[h, w].WorldPosCenter, 0.09f);
                   
            }
        }
    }
}
