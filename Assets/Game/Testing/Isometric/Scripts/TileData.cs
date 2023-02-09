using UnityEngine;

public enum TileType {
    Free,
    Blocker
}

public class TileData
{

    private TileType _type;
    public Vector3 worldPos;

    public TileData(Vector3 worldPos_, TileType type_) 
    {
        worldPos = worldPos_;
        _type = type_;
    }

    public bool IsClear() => _type == TileType.Free;
    public bool IsBlocked() => _type == TileType.Blocker;
}
