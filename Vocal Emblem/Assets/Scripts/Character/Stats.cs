using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stats : MonoBehaviour
{
    public string charName = "Character";

    public List<Weapon> weapons = new List<Weapon>();

    public Weapon equippedWeapon;

    public enum movementType { INFANTRY, FLIER};
    public movementType typeMovement;

    public float maxHP;
    public float hp;
    public float maxStr;
    public float str;
    public float maxScr;
    public float scr;
    public float maxSkill;
    public float skill;
    public float maxSpd;
    public float spd;
    public float maxLuk;
    public float luk;
    public float maxDef;
    public float def;
    public float maxRes;
    public float res;
    public float mov;
}
