using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponClass
{
    Pistol,
    Rifle,
    AssaultRifle,
    Sniper,
    ShotGun,
    NUM_CLASSES
}
public class Weapon
{
   public readonly WeaponClass weaponClass;
}
