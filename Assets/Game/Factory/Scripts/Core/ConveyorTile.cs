using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Option;
using static Option.Option;

public struct ConveyorTile 
{
    private enum Orientation
    {
        Left,
        Right,
        Up,
        Down,
        None
    }

    List<ConveyorTile> _incoming;
    List<ConveyorTile> _outgoing;
    Vector2Int _tilePos;
    Orientation _orientation;
    int _destinationIndex;

    public Vector2Int tilePosition => _tilePos;
    public List<ConveyorTile> incoming => _incoming;
    public List<ConveyorTile> outgoing => _outgoing;

    public ConveyorTile(Vector2Int tilePos)
    {
        _tilePos = tilePos;
        _incoming = new List<ConveyorTile>();
        _outgoing = new List<ConveyorTile>();
        _orientation = Orientation.None;
        _destinationIndex = 0;
    }

    public void AddToOutgoing(ConveyorTile tile)
    {
        _outgoing.Add(tile);

        if (_outgoing.Count == 1)
        {
            Vector2Int delta = tilePosition - tile.tilePosition;

            _orientation = 
            (delta.x, delta.y) switch
            {
                (1, 0) => Orientation.Left,
                (0, 1) => Orientation.Down,
                (-1, 0) => Orientation.Right,
                (0, -1) => Orientation.Up,
                _ => throw new UnreachableException()
            };
        }
        else
            _orientation = Orientation.None;
    }

    public void AddToIncoming(ConveyorTile tile)
    {
        _incoming.Add(tile);
    }

    public void RemoveFromOutgoing(ConveyorTile tile)
    {
        _outgoing.Remove(tile);
    }

    public void RemoveFromIncoming(ConveyorTile tile)
    {
        _incoming.Remove(tile);
    }

    public bool IsInOutgoing(ConveyorTile tile)
    {
        return _outgoing.Contains(tile);
    }

    public bool IsInIncoming(ConveyorTile tile)
    {
        return _incoming.Contains(tile);
    }

    public bool IsConnected(ConveyorTile tile)
    {
        return (IsInOutgoing(tile) && tile.IsInIncoming(this)) ||
               (IsInIncoming(tile) && tile.IsInOutgoing(this));
    }

    public void CopyFrom(ConveyorTile tile)
    {
        Debug.Assert(this.tilePosition == tile.tilePosition);

        this._orientation = tile._orientation;
        this._incoming = tile.incoming;
        this._outgoing = tile.outgoing;
    }

    public Option<ConveyorTile> GetNextDestinationFrom(ConveyorTile tile)
    {
        Debug.Assert(_outgoing.Count == 1 && _outgoing[0].tilePosition == tile.tilePosition);

        if (_outgoing.Count == 0)
            return None<ConveyorTile>();

        ConveyorTile outGoing = _outgoing[_destinationIndex];
        
        if (outGoing.tilePosition == tile.tilePosition)
        {
             _destinationIndex = (_destinationIndex + 1) % _outgoing.Count;
             outGoing = _outgoing[_destinationIndex];
        }
        
        _destinationIndex = (_destinationIndex + 1) % _outgoing.Count;
        return Some<ConveyorTile>(outGoing);
    }
}
