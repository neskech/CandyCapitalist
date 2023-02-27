using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Item
{
    Sprite _sprite;
    public Sprite Sprite => _sprite;


    public Item(Sprite sprite)
    {
        _sprite = sprite;
    }

    public override bool Equals(object other)
    {
        throw new System.NotImplementedException();
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public static bool operator ==(Item thisGuy, Item other) => thisGuy.Equals(other);
    public static bool operator !=(Item thisGuy, Item other) => !thisGuy.Equals(other);
}

public struct ItemStack
{
    public Item item;
    public int count;
    public const int MAX_STACK_SIZE = 64;

    bool TryAddItem(Item item)
    {
        if (this.item != item || count == MAX_STACK_SIZE)
            return false;

        count++;
        return true;
    }

    bool TryRemoveItem()
    {
        if (count == 0)
            return false;

        count--;
        return true;
    }
}
