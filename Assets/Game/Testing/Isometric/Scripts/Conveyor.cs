using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Orientation
{
    Horizontal,
    Vectical
}

public struct Conveyor {
    public Orientation _orientation;
    public Vector3Int _index;
    public Vector3 _worldPos;
    public List<Conveyor> _incoming;
    public List<Conveyor> _outgoing;

    public Conveyor(Vector3Int index)
    {
        _index = index;
        _orientation = Orientation.Horizontal;
        _worldPos = new Vector3();
        _incoming = new List<Conveyor>();
        _outgoing = new List<Conveyor>();
    }
}
