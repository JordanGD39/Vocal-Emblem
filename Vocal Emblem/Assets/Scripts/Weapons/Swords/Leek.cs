using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leek : Weapon
{
    Leek()
    {
        weaponName = "Normal Leek";
        description = "Just a normal leek";
        typeOfWeapon = WeaponType.SWORD;
        mt = 4;
        uses = 39;
        crit = 0;
        accuracy = 100;
        skillLevel = 0;
        range = 1;
        weight = 1;
        worth = 100;
        exp = 1;
    }
}
