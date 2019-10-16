using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapon")]
public class Weapon : ScriptableObject
{
    public string weaponName = "";
    public string description = "";
    public enum WeaponType {SWORD, AXE, LANCE, BOW, SCREAM, STAFF};
    public WeaponType typeOfWeapon;
    public int mt = 5;
    public int uses = 42;
    public int crit = 0;
    public int accuracy = 100;
    public int skillLevel = 1;
    public int range = 1;
    public bool rangeOneAndTwo = true;
    public bool counterAll = false;
    public int worth = 320;
    public int exp = 1;
}
