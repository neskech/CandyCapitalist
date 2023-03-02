using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Option;

public class ConveyorManager
{
    struct ItemObject
    {
        public GameObject gameObject;
        public ConveyorTile currentConveyor;
        public ConveyorTile targetConveyor;
        public float timeTraveledAlongPath;
        public ItemStack item;
    }

    List<ConveyorTile> _conveyors;
    List<ItemObject> _items;
    float _itemPassDuration;
    FactoryBaseLayer _baseLayer;

    public ConveyorManager(float itemPassDuration, FactoryBaseLayer baseLayer)
    {
        _conveyors = new List<ConveyorTile>();
        _items = new List<ItemObject>();
        _itemPassDuration = itemPassDuration;
        _baseLayer = baseLayer;
    }

    public void Update()
    {
        ItemPassing();
    }

    public void GetItemFrom(ItemStack stack, Vector2Int source)
    {
        /*
            Should be called by the machine manager. 
            We get item stacks spit out from a machine
            at a particular position
        */

        //Instantiate a new gameobject with the item sprite

        //Find which head from the conveyor list the item spawned on given source

        //Make an item object from that, getting the target conveyor

        //Add it to the list

    }

    void ItemPassing()
    {
        for (int i = _items.Count - 1; i >= 0; i--)
        {
            ItemObject obj = _items[i];

            Vector3 currPos = obj.gameObject.transform.position;
            Vector3 destinationPos = _baseLayer.FromIsometricBasis(obj.targetConveyor.tilePosition);

            float oldz = currPos.z;
            float t = obj.timeTraveledAlongPath / _itemPassDuration;
            Vector3 newPos = Vector3.Lerp(currPos, destinationPos, t);
            
            obj.timeTraveledAlongPath += Time.deltaTime;
            if (obj.timeTraveledAlongPath > _itemPassDuration)
            {
                ConveyorTile oldConveyor = obj.currentConveyor;
                obj.currentConveyor = obj.targetConveyor;

                Option<ConveyorTile> next = obj.targetConveyor.GetNextDestinationFrom(oldConveyor);
                if (next.IsSome())
                        obj.targetConveyor = next.Unwrap();
                else
                {
                    _items.Remove(obj);
                    //TODO pass the itemstack within obj to the machine manager
                }
                
            }


        }

    }

    public void QueryNewConveyor(Vector2Int pos)
    {
        ConveyorTile tile = new ConveyorTile(pos);
        bool inAny = _conveyors.TrueForAll((ConveyorTile curr) => IsConveyorIn(curr, tile));

        if (inAny)
            ModifyConveyor(tile);
        else
            AddConveyor(tile);
    }

    public void ModifyConveyor(ConveyorTile tile)
    {

    }

    public void AddConveyor(ConveyorTile tile)
    {
      
  
    }

    bool AddToConveyorList(ConveyorTile head, ConveyorTile target)
    {
        /*
            Returns true if the tile was sucessfully added
        */

 

        return true;
    }

    List<ConveyorTile> GetAdjacentsFromHead(ConveyorTile head, ConveyorTile target)
    {
        /*
            Want to get all adjacent tiles to the target tile from a conveyor
            graph. Depth first search over a conveyor graph for this

            For the case of cyclicur conveyor graphs (I don't know if we even want those)
            we have an explored set
        */

        List<ConveyorTile> adjacents = new List<ConveyorTile>();
        Stack<ConveyorTile> frontier = new Stack<ConveyorTile>();
        HashSet<ConveyorTile> explored = new HashSet<ConveyorTile>();

        frontier.Push(head);
        int[] adj = new int[]{1, -1, 0, 0};

        while (frontier.Count != 0)
        {
            ConveyorTile top = frontier.Pop();

            if (explored.Contains(top)) 
                continue;

            explored.Add(top);


            Vector2Int pos = top.tilePosition;
            for (int i = 0; i < 4; i++)
            {
                Vector2Int adjPos = new Vector2Int(pos.x + adj[i], pos.y + adj[3 - i]);
                if (adjPos == target.tilePosition)
                {
                    adjacents.Add(top);
                    break;
                }
            }

            //loop over adjacent positions
            foreach (ConveyorTile outgoing in top.outgoing)
                frontier.Push(outgoing);
        }

        return adjacents;
    }

    bool IsConveyorIn(ConveyorTile head, ConveyorTile target)
    {
        Queue<ConveyorTile> frontier = new Queue<ConveyorTile>();
        HashSet<ConveyorTile> explored = new HashSet<ConveyorTile>();

        frontier.Enqueue(head);

        while (frontier.Count != 0)
        {
            ConveyorTile top = frontier.Dequeue();

            if (explored.Contains(top)) 
                continue;

            explored.Add(top);

            if (top.tilePosition == target.tilePosition)
                return true;


            //loop over adjacent positions
            foreach (ConveyorTile outgoing in top.outgoing)
                frontier.Enqueue(outgoing);
        }

        return false;
    }
}
