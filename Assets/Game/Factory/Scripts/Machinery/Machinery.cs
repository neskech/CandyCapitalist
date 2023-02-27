using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Machinery
{
    protected List<ItemStack> _inventory;
    public abstract void ProcessInventory();
}
