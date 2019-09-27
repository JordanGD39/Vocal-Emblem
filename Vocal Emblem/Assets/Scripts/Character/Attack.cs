using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Attack : MonoBehaviour
{
    private TileData tileData;
    private Stats stats;
    private GameObject cursor;
    public GameObject target;

    private int weaponTrianglePlayer = 0;
    private int weaponTriangleEnemy = 0;

    [SerializeField]
    private List<GameObject> enemies = new List<GameObject>();

    public int indexEnemies = 0;

    // Start is called before the first frame update
    void Start()
    {
        tileData = GameObject.FindGameObjectWithTag("TileManager").GetComponent<TileData>();
        stats = GetComponent<Stats>();
        cursor = GameObject.FindGameObjectWithTag("Cursor");
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null && cursor.GetComponent<Cursor>().attackPanel.activeSelf)
        {
            if (Input.GetButtonDown("Horizontal") && Input.GetAxis("Horizontal") > 0)
            {
                indexEnemies++;                       

                if (indexEnemies >= enemies.Count)
                {
                    indexEnemies = 0;
                }

                CheckAttackRange();
            }
            else if (Input.GetButtonDown("Horizontal") && (Input.GetAxis("Horizontal") < 0))
            {
                indexEnemies--;
                if (indexEnemies <= -1)
                {
                    indexEnemies = enemies.Count - 1;
                }
                CheckAttackRange();
            }

            if (Input.GetButtonDown("Vertical") && Input.GetAxis("Vertical") > 0)
            {
                indexEnemies++;
                if (indexEnemies >= enemies.Count)
                {
                    indexEnemies = 0;
                }
                CheckAttackRange();
            }
            else if (Input.GetButtonDown("Vertical") && (Input.GetAxis("Vertical") < 0))
            {
                indexEnemies--;
                if (indexEnemies <= -1)
                {
                    indexEnemies = enemies.Count - 1;
                }
                CheckAttackRange();
            }
        }
    }

    public void CheckAttackRange()
    {
        enemies = tileData.CheckEnemiesInRange(Mathf.RoundToInt(transform.position.x - 0.5f), Mathf.RoundToInt(transform.position.y - 0.5f), stats.equippedWeapon.range, stats.equippedWeapon.rangeOneAndTwo);
        if (enemies.Count > 0)
        {
            cursor.transform.position = enemies[indexEnemies].transform.position;
            target = enemies[indexEnemies];
            cursor.GetComponent<Cursor>().selectPanel.SetActive(false);
            cursor.GetComponent<Cursor>().attackPanel.SetActive(true);

            Transform attackPanel = cursor.GetComponent<Cursor>().attackPanel.transform;
            Transform statsPanel = null;
            Transform hpPanel = null;
            Transform mtPanel = null;
            Transform hitPanel = null;
            Transform critPanel = null;            

            //MT calc
            float damage = CalcDamage(gameObject, target);
            int doubling = CalcSpeed(gameObject, target);
            //Hit calc
            float hit = CalcHit(gameObject);
            float enemyEvade = CalcEvade(target);
            float acc = CalcAccuracy(hit, enemyEvade);
            //Crit calc
            float critRate = CalcCrit(gameObject);
            float enemyCritEvade = CalcCritEvade(target);
            float crit = CalcCritHit(critRate, enemyCritEvade);

            float enemyDamage = 0;
            int enemyDoubling = 0;
            float enemyHit = 0;
            float evade = 0;
            float enemyAcc = 0;
            float enemyCritRate = 0;
            float critEvade = 0;
            float enemyCrit = 0;

            if ((gameObject.transform.position.x - target.transform.position.x) + (gameObject.transform.position.y - target.transform.position.y) <= target.GetComponent<Stats>().equippedWeapon.range || target.GetComponent<Stats>().equippedWeapon.counterAll)
            {
                enemyDamage = CalcDamage(target, gameObject);
                enemyDoubling = CalcSpeed(target, gameObject);
                enemyHit = CalcHit(target);
                evade = CalcEvade(gameObject);
                enemyAcc = CalcAccuracy(enemyHit, evade);

                enemyCritRate = CalcCrit(target);
                critEvade = CalcCritEvade(gameObject);
                enemyCrit = CalcCritHit(enemyCritRate, critEvade);
            }           

            for (int i = 0; i < attackPanel.childCount; i++)
            {
                if (attackPanel.GetChild(i).name == "Stats")
                {
                    statsPanel = attackPanel.GetChild(i);
                    Debug.Log("StatsFound");
                }
            }

            for (int i = 0; i < statsPanel.childCount; i++)
            {
                switch (statsPanel.GetChild(i).name)
                {
                    case "HP":
                        hpPanel = statsPanel.GetChild(i);

                        for (int j = 0; j < hpPanel.childCount; j++)
                        {
                            if (hpPanel.GetChild(j).name == "PlayerHPbar")
                            {
                                Debug.Log(hpPanel.GetChild(j));
                                for (int k = 0; k < hpPanel.GetChild(j).childCount; k++)
                                {
                                    if (hpPanel.GetChild(j).GetChild(k).name == "Health")
                                    {
                                        Image bar = hpPanel.GetChild(j).GetChild(k).GetComponent<Image>();
                                        bar.fillAmount = stats.hp / stats.maxHP;
                                    }
                                    else if(hpPanel.GetChild(j).GetChild(k).name == "HealthAfterAttack")
                                    {
                                        float healthAfterAttack = stats.hp - (enemyDamage * enemyDoubling);
                                        if (healthAfterAttack < 0)
                                        {
                                            healthAfterAttack = 0;
                                        }

                                        Image bar = hpPanel.GetChild(j).GetChild(k).GetComponent<Image>();
                                        bar.fillAmount = healthAfterAttack / stats.maxHP;
                                    }
                                    else if(hpPanel.GetChild(j).GetChild(k).name == "Text")
                                    {
                                        Text hp = hpPanel.GetChild(j).GetChild(k).GetComponent<Text>();
                                        hp.text = stats.hp.ToString();
                                    }
                                    else if(hpPanel.GetChild(j).GetChild(k).name == "TextAfterAttack")
                                    {
                                        float healthAfterAttack = stats.hp - (enemyDamage * enemyDoubling);
                                        if (healthAfterAttack < 0)
                                        {
                                            healthAfterAttack = 0;
                                        }

                                        Text hp = hpPanel.GetChild(j).GetChild(k).GetComponent<Text>();
                                        hp.text = healthAfterAttack.ToString();
                                    }
                                }                                
                            }
                            else if (hpPanel.GetChild(j).name == "EnemyHPbar")
                            {
                                for (int k = 0; k < hpPanel.GetChild(j).childCount; k++)
                                {
                                    if (hpPanel.GetChild(j).GetChild(k).name == "Health")
                                    {
                                        Image bar = hpPanel.GetChild(j).GetChild(k).GetComponent<Image>();
                                        bar.fillAmount = target.GetComponent<Stats>().hp / target.GetComponent<Stats>().maxHP;
                                    }
                                    else if (hpPanel.GetChild(j).GetChild(k).name == "HealthAfterAttack")
                                    {
                                        float healthAfterAttack = target.GetComponent<Stats>().hp - (damage * doubling);
                                        if (healthAfterAttack < 0)
                                        {
                                            healthAfterAttack = 0;
                                        }

                                        Image bar = hpPanel.GetChild(j).GetChild(k).GetComponent<Image>();
                                        bar.fillAmount = healthAfterAttack / target.GetComponent<Stats>().maxHP;
                                    }
                                    else if (hpPanel.GetChild(j).GetChild(k).name == "Text")
                                    {
                                        Text hp = hpPanel.GetChild(j).GetChild(k).GetComponent<Text>();
                                        hp.text = target.GetComponent<Stats>().hp.ToString();
                                    }
                                    else if (hpPanel.GetChild(j).GetChild(k).name == "TextAfterAttack")
                                    {
                                        float healthAfterAttack = target.GetComponent<Stats>().hp - (damage * doubling);
                                        if (healthAfterAttack < 0)
                                        {
                                            healthAfterAttack = 0;
                                        }

                                        Text hp = hpPanel.GetChild(j).GetChild(k).GetComponent<Text>();
                                        hp.text = healthAfterAttack.ToString("F0");
                                    }
                                }
                            }
                        }
                        break;
                    case "MT":
                        mtPanel = statsPanel.GetChild(i);

                        for (int j = 0; j < mtPanel.childCount; j++)
                        {
                            if (mtPanel.GetChild(j).name == "PlayerMT")
                            {
                                Debug.Log(mtPanel.GetChild(j));
                                for (int k = 0; k < mtPanel.GetChild(j).childCount; k++)
                                {
                                    if (mtPanel.GetChild(j).GetChild(k).name == "Text")
                                    {
                                        Text mt = mtPanel.GetChild(j).GetChild(k).GetComponent<Text>();
                                        mt.text = damage.ToString();
                                        Debug.Log("Damage: " + damage);
                                    }
                                    else if (mtPanel.GetChild(j).GetChild(k).name == "X2")
                                    {
                                        GameObject x2 = mtPanel.GetChild(j).GetChild(k).gameObject;
                                        x2.SetActive(false);

                                        switch (doubling)
                                        {
                                            case 2:
                                                x2.SetActive(true);
                                                break;
                                            case 4:
                                                x2.SetActive(true);
                                                x2.GetComponentInChildren<Text>().text = "x4";
                                                break;
                                        }
                                    }
                                }
                            }
                            else if (mtPanel.GetChild(j).name == "EnemyMT")
                            {
                                Debug.Log(mtPanel.GetChild(j));
                                for (int k = 0; k < mtPanel.GetChild(j).childCount; k++)
                                {
                                    if (mtPanel.GetChild(j).GetChild(k).name == "Text")
                                    {
                                        Text mt = mtPanel.GetChild(j).GetChild(k).GetComponent<Text>();                                        
                                        mt.text = enemyDamage.ToString();
                                        Debug.Log("EnemyDamage: " + enemyDamage);
                                    }
                                    else if (mtPanel.GetChild(j).GetChild(k).name == "X2")
                                    {
                                        GameObject x2 = mtPanel.GetChild(j).GetChild(k).gameObject;
                                        x2.SetActive(false);

                                        switch (enemyDoubling)
                                        {
                                            case 2:
                                                x2.SetActive(true);
                                                break;
                                            case 4:
                                                x2.SetActive(true);
                                                x2.GetComponentInChildren<Text>().text = "x4";
                                                break;
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case "Hit":
                        hitPanel = statsPanel.GetChild(i);

                        for (int j = 0; j < hitPanel.childCount; j++)
                        {
                            if (hitPanel.GetChild(j).name == "Player")
                            {
                                Debug.Log(hitPanel.GetChild(j));
                                for (int k = 0; k < hitPanel.GetChild(j).childCount; k++)
                                {
                                    if (hitPanel.GetChild(j).GetChild(k).name == "Text")
                                    {
                                        Text hitTxt = hitPanel.GetChild(j).GetChild(k).GetComponent<Text>();
                                        hitTxt.text = acc.ToString("F0");
                                    }                                  
                                }
                            }
                            else if (hitPanel.GetChild(j).name == "Enemy")
                            {
                                Debug.Log(hitPanel.GetChild(j));
                                for (int k = 0; k < hitPanel.GetChild(j).childCount; k++)
                                {
                                    if (hitPanel.GetChild(j).GetChild(k).name == "Text")
                                    {
                                        Text hitTxt = hitPanel.GetChild(j).GetChild(k).GetComponent<Text>();
                                        hitTxt.text = enemyAcc.ToString("F0");
                                    }
                                }
                            }
                        }
                        break;
                    case "Crit":
                        critPanel = statsPanel.GetChild(i);

                        for (int j = 0; j < critPanel.childCount; j++)
                        {
                            if (critPanel.GetChild(j).name == "Player")
                            {
                                Debug.Log(critPanel.GetChild(j));
                                for (int k = 0; k < critPanel.GetChild(j).childCount; k++)
                                {
                                    if (critPanel.GetChild(j).GetChild(k).name == "Text")
                                    {
                                        Text critTxt = critPanel.GetChild(j).GetChild(k).GetComponent<Text>();
                                        critTxt.text = crit.ToString("F0");
                                    }
                                }
                            }
                            else if (critPanel.GetChild(j).name == "Enemy")
                            {
                                Debug.Log(critPanel.GetChild(j));
                                for (int k = 0; k < critPanel.GetChild(j).childCount; k++)
                                {
                                    if (critPanel.GetChild(j).GetChild(k).name == "Text")
                                    {
                                        Text critTxt = critPanel.GetChild(j).GetChild(k).GetComponent<Text>();
                                        critTxt.text = enemyCrit.ToString("F0");
                                    }
                                }
                            }
                        }
                        break;
                }
            }
        }
    }

    private float CalcCrit(GameObject character)
    {
        float crit = 0;

        crit = character.GetComponent<Stats>().equippedWeapon.crit + character.GetComponent<Stats>().skill / 2 + 0 + 0; //0's for missing parts

        return crit;
    }

    private float CalcCritEvade(GameObject character)
    {
        float critEvade = 0;

        critEvade = character.GetComponent<Stats>().luk + 0 + 0;//0's missing parts

        return critEvade;
    }

    private float CalcCritHit(float hit, float evade)
    {
        float crit = hit - evade;

        if (crit < 0)
        {
            crit = 0;
        }
        else if(crit > 100)
        {
            crit = 100;
        }

        return crit;
    }

    private float CalcAccuracy(float hit, float evasion)
    {
        float accuracy = hit - evasion;
        if (accuracy < 0)
        {
            accuracy = 0;
        }
        else if (accuracy > 100)
        {
            accuracy = 100;
        }

        return accuracy;
    }

    private float CalcEvade(GameObject character)
    {
        float evasion = 0;

        evasion = character.GetComponent<Stats>().spd * 2 + character.GetComponent<Stats>().luk + 0 + 0; //0's are the missing pieces

        return evasion;
    }

    private float CalcDamage(GameObject firstChar, GameObject otherChar)
    {
        float damage = 0;

        if (firstChar.GetComponent<Stats>().equippedWeapon.typeOfWeapon == Weapon.WeaponType.SCREAM)
        {
            damage = firstChar.GetComponent<Stats>().scr + firstChar.GetComponent<Stats>().equippedWeapon.mt; //Add Support
            damage -= otherChar.GetComponent<Stats>().res;
            if (damage < 0)
            {
                damage = 0;
            }
        }
        else
        {
            int weaponTriangle = 0;
            int weaponEffectiveness = 1;

            switch (firstChar.GetComponent<Stats>().equippedWeapon.typeOfWeapon)
            {
                case Weapon.WeaponType.SWORD:
                    switch (otherChar.GetComponent<Stats>().equippedWeapon.typeOfWeapon)
                    {
                        case Weapon.WeaponType.AXE:
                            weaponTriangle = 1;
                            break;
                        case Weapon.WeaponType.LANCE:
                            weaponTriangle = -1;
                            break;
                        default:
                            break;
                    }
                    break;
                case Weapon.WeaponType.AXE:
                    switch (otherChar.GetComponent<Stats>().equippedWeapon.typeOfWeapon)
                    {
                        case Weapon.WeaponType.SWORD:
                            weaponTriangle = -1;
                            break;
                        case Weapon.WeaponType.LANCE:
                            weaponTriangle = 1;
                            break;
                        default:
                            break;
                    }
                    break;
                case Weapon.WeaponType.LANCE:
                    switch (otherChar.GetComponent<Stats>().equippedWeapon.typeOfWeapon)
                    {
                        case Weapon.WeaponType.AXE:
                            weaponTriangle = -1;
                            break;
                        case Weapon.WeaponType.SWORD:
                            weaponTriangle = 1;
                            break;
                        default:
                            break;
                    }
                    break;
                case Weapon.WeaponType.BOW:
                    if (otherChar.GetComponent<Stats>().typeMovement == Stats.movementType.FLIER)
                    {
                        weaponEffectiveness = 2;
                    }
                    break;
            }            
            damage = firstChar.GetComponent<Stats>().str + (firstChar.GetComponent<Stats>().equippedWeapon.mt + weaponTriangle) * weaponEffectiveness; //Add support
            damage -= otherChar.GetComponent<Stats>().def;
            if (damage < 0)
            {
                damage = 0;
            }
            if (firstChar.GetComponent<PlayerMovement>() != null)
            {
                weaponTrianglePlayer = weaponTriangle;
            }
            else
            {
                weaponTriangleEnemy = weaponTriangle;
            }
        }
        
        return damage;
    }

    private int CalcSpeed(GameObject firstChar, GameObject otherChar)
    {
        int i = 1;

        if (firstChar.GetComponent<Stats>().spd - 4 > otherChar.GetComponent<Stats>().spd)
        {
            i = 2;
        }
        if (firstChar.GetComponent<Stats>().spd - 8 > otherChar.GetComponent<Stats>().spd)
        {
            i = 4;
        }
        return i;
    }

    private float CalcHit(GameObject character)
    {
        float hitRate = 0;

        hitRate = character.GetComponent<Stats>().equippedWeapon.accuracy + character.GetComponent<Stats>().skill * 2 + character.GetComponent<Stats>().luk / 2 + 0 + 0; //0's for missing parts

        return hitRate;
    }
}
