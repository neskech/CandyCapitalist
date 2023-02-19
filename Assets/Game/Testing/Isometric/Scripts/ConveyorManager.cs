using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;

public class ConveyorManager : MonoBehaviour
{
    List<Conveyor> _conveyors;
    public Tile _conveyorSprite;
    public Tilemap _map;

    // Start is called before the first frame update
    void Start()
    {
        List<Conveyor> l = new List<Conveyor>
        {
            new Conveyor(new Vector3Int(0, 0, 1)),
            new Conveyor(new Vector3Int(15, 0, 1))
        };
        BuildFromConveyorList(l);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void AddConveyorsToTileMap()
    {
        foreach (Conveyor c in _conveyors)
        {
            Vector3Int index = c._index;
            _map.SetTile(index, _conveyorSprite);
        }
    }

    public void BuildFromConveyorList(List<Conveyor> list)
    {
        //TODO
        _conveyors = list;
        AddConveyorsToTileMap();
    }
}
