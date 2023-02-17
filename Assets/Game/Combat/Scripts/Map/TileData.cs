using UnityEngine;

public enum TileType {
    Free,
    Blocker
}

public class TileData
{

    private TileType _type;
    Vector3 _worldPosCenter;
    Vector3Int _index;

    public Vector3 WorldPosCenter {get => _worldPosCenter;}
    public Vector3Int Index {get => _index;}

    public TileData(Vector3 worldPosCenter_, Vector3Int index_,  TileType type_) 
    {
        _worldPosCenter = worldPosCenter_;
        _index = index_;
        _type = type_;
    }

     public TileData(Vector3 worldPosCenter_, Vector3Int index_) 
    {
        _worldPosCenter = worldPosCenter_;
        _index = index_;
        _type = TileType.Free;
    }

    public bool IsClear() => _type == TileType.Free;
    public bool IsBlocked() => _type == TileType.Blocker;
    public void SwitchToBlocker() => _type = TileType.Blocker;

    public Vector3 BottomLeft() 
    {
        Vector3 shift = TileMaster.FromIsometricBasis(new Vector2(0.5f, -0.5f));
        return _worldPosCenter + shift * TileMaster.Instance.TileDimensions;
    }

    public Vector3 TopLeft() 
    {
        Vector3 shift = TileMaster.FromIsometricBasis(new Vector2(-0.5f, 0.5f));
        return _worldPosCenter + shift * TileMaster.Instance.TileDimensions;
    }

    public Vector3 BottomRight() 
    {
        Vector3 shift = TileMaster.FromIsometricBasis(new Vector2(-0.5f, -0.5f));
        return _worldPosCenter + shift * TileMaster.Instance.TileDimensions;
    }

    public Vector3 TopRight() 
    {
         Vector3 shift = TileMaster.FromIsometricBasis(new Vector2(-0.5f, 0.5f));
        return _worldPosCenter + shift * TileMaster.Instance.TileDimensions;
    }
}
