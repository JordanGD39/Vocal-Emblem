using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Attack : MonoBehaviour
{
    private TileData tileData;
    private Stats stats;
    private Heal heal;
    private GameObject cursor;
    public GameObject target;

    public int triangleBonus = 0;
    public int triangleBonusEnemy = 0;

    public float damage = 0;
    public float acc = 0;
    public float crit = 0;
    public int doubling = 0;

    public float enemyDamage = 0;
    public float enemyAcc = 0;
    public float enemyCrit = 0;
    public int enemyDoubling = 0;

    public float distance = 0;

    [SerializeField] private List<GameObject> enemies = new List<GameObject>();

    public int indexEnemies = 0;

    private bool greenTiles = false;

    // Start is called before the first frame update
    void Start()
    {
        tileData = GameObject.FindGameObjectWithTag("TileManager").GetComponent<TileData>();
        stats = GetComponent<Stats>();
        heal = GetComponent<Heal>();
        cursor = GameObject.FindGameObjectWithTag("Cursor");
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null && cursor.GetComponent<Cursor>().attackPanel.activeSelf && cursor.GetComponent<Cursor>().currSelectedChar == gameObject && gameObject.CompareTag("Player"))
        {
            if (Input.GetButtonDown("Horizontal") && Input.GetAxis("Horizontal") > 0)
            {
                indexEnemies++;                       

                if (indexEnemies >= enemies.Count)
                {
                    indexEnemies = 0;
                }

                CheckAttackRange(greenTiles);
            }
            else if (Input.GetButtonDown("Horizontal") && (Input.GetAxis("Horizontal") < 0))
            {
                indexEnemies--;
                if (indexEnemies <= -1)
                {
                    indexEnemies = enemies.Count - 1;
                }
                CheckAttackRange(greenTiles);
            }

            if (Input.GetButtonDown("Vertical") && Input.GetAxis("Vertical") > 0)
            {
                indexEnemies++;
                if (indexEnemies >= enemies.Count)
                {
                    indexEnemies = 0;
                }
                CheckAttackRange(greenTiles);
            }
            else if (Input.GetButtonDown("Vertical") && (Input.GetAxis("Vertical") < 0))
            {
                indexEnemies--;
                if (indexEnemies <= -1)
                {
                    indexEnemies = enemies.Count - 1;
                }
                CheckAttackRange(greenTiles);
            }
        }
    }

    public void CheckAttackRange(bool healing)
    {
        if (gameObject.CompareTag("Player") && !healing)
        {
            enemies = tileData.CheckEnemiesInRange(Mathf.RoundToInt(transform.position.x - 0.5f), Mathf.RoundToInt(transform.position.y - 0.5f), stats.equippedWeapon.range, stats.equippedWeapon.rangeOneAndTwo, true, false);
        }
        else if(gameObject.CompareTag("Enemy") && !healing)
        {
            enemies = tileData.CheckEnemiesInRange(Mathf.RoundToInt(transform.position.x - 0.5f), Mathf.RoundToInt(transform.position.y - 0.5f), stats.equippedWeapon.range, stats.equippedWeapon.rangeOneAndTwo, false, false);
        }

        if (enemies.Count > 0 || heal.GetAllies().Count > 0)
        {            
            if (!healing)
            {
                greenTiles = false;
                target = enemies[indexEnemies];
                cursor.transform.position = enemies[indexEnemies].transform.position;
            }
            else
            {
                greenTiles = true;
                List<GameObject>allies = heal.GetAllies();
                target = allies[indexEnemies];
                cursor.transform.position = allies[indexEnemies].transform.position;
            }

            if (gameObject.CompareTag("Player"))
            {
                cursor.GetComponent<Cursor>().selectPanel.SetActive(false);
                cursor.GetComponent<Cursor>().attackPanel.SetActive(true);
            }

            Transform attackPanel = cursor.GetComponent<Cursor>().attackPanel.transform;            
            Transform statsPanel = null;
            Transform hpPanel = null;
            Transform mtPanel = null;
            Transform hitPanel = null;
            Transform critPanel = null;            
            Transform weaponPanel = null;

            int x = Mathf.RoundToInt(transform.position.x - 0.5f);
            int y = Mathf.RoundToInt(transform.position.y - 0.5f);
            int xEnemy = Mathf.RoundToInt(target.transform.position.x - 0.5f);
            int yEnemy = Mathf.RoundToInt(target.transform.position.y - 0.5f);
            Debug.Log(xEnemy + " " + yEnemy);

            //MT calc
            triangleBonus = 0;
            triangleBonusEnemy = 0;

            if (!healing)
            {
                damage = CalcDamage(gameObject, target);
                doubling = CalcSpeed(gameObject, target);

                //Hit calc
                float hit = CalcHit(gameObject);
                float enemyEvade = CalcEvade(target, xEnemy, yEnemy);
                acc = CalcAccuracy(hit, enemyEvade, gameObject);
                //Crit calc
                float critRate = CalcCrit(gameObject);
                float enemyCritEvade = CalcCritEvade(target);
                crit = CalcCritHit(critRate, enemyCritEvade);

                enemyDamage = 0;
                enemyCrit = 0;
                enemyDoubling = 0;
                float enemyHit = 0;
                float evade = 0;
                float enemyCritRate = 0;
                float critEvade = 0;
                enemyAcc = 0;

                distance = Mathf.Abs(gameObject.transform.position.x - target.transform.position.x) + Mathf.Abs(gameObject.transform.position.y - target.transform.position.y);

                if (distance <= target.GetComponent<Stats>().equippedWeapon.range && target.GetComponent<Stats>().equippedWeapon.rangeOneAndTwo || distance != 1 && distance <= target.GetComponent<Stats>().equippedWeapon.range && !target.GetComponent<Stats>().equippedWeapon.rangeOneAndTwo || target.GetComponent<Stats>().equippedWeapon.counterAll)
                {
                    enemyDamage = CalcDamage(target, gameObject);
                    enemyDoubling = CalcSpeed(target, gameObject);
                    enemyHit = CalcHit(target);
                    evade = CalcEvade(gameObject, x, y);
                    enemyAcc = CalcAccuracy(enemyHit, evade, target);

                    enemyCritRate = CalcCrit(target);
                    critEvade = CalcCritEvade(gameObject);
                    enemyCrit = CalcCritHit(enemyCritRate, critEvade);
                }
            }
            else
            {
                damage = stats.scr + 10;
                acc = 100;
                crit = 0;
            }            

            if (gameObject.CompareTag("Player"))
            {
                for (int i = 0; i < attackPanel.childCount; i++)
                {
                    if (attackPanel.GetChild(i).name == "Stats")
                    {
                        statsPanel = attackPanel.GetChild(i);
                        Debug.Log("StatsFound");
                    }
                    else if (attackPanel.GetChild(i).name == "Weapon")
                    {
                        weaponPanel = attackPanel.GetChild(i);

                        for (int j = 0; j < weaponPanel.childCount; j++)
                        {
                            if (weaponPanel.GetChild(j).name == "Weapon")
                            {
                                Transform weaponEmpty = weaponPanel.GetChild(j);

                                for (int k = 0; k < weaponEmpty.childCount; k++)
                                {
                                    if (weaponEmpty.GetChild(k).name == "Text")
                                    {
                                        weaponEmpty.GetChild(k).GetComponent<Text>().text = stats.equippedWeapon.weaponName;
                                    }
                                    else if (weaponEmpty.GetChild(k).name == "Dur")
                                    {
                                        weaponEmpty.GetChild(k).GetComponent<Text>().text = stats.equippedWeapon.uses.ToString();
                                    }
                                }
                            }
                            else if (weaponPanel.GetChild(j).name == "Arrow")
                            {
                                Image arrow = weaponPanel.GetChild(j).GetComponent<Image>();

                                if (triangleBonus == 15)
                                {
                                    arrow.gameObject.SetActive(true);
                                    arrow.transform.rotation = Quaternion.Euler(0, 0, 0);
                                    arrow.color = new Color32(105, 255, 102, 255);
                                }
                                else if (triangleBonus == 0)
                                {
                                    arrow.gameObject.SetActive(false);
                                }
                                else if (triangleBonus == -15)
                                {
                                    arrow.gameObject.SetActive(true);
                                    arrow.transform.rotation = Quaternion.Euler(0, 0, 180);
                                    arrow.color = new Color32(255, 104, 102, 255);
                                }
                            }
                        }
                    }
                    else if (attackPanel.GetChild(i).name == "EnemyWeapon")
                    {
                        weaponPanel = attackPanel.GetChild(i);

                        for (int j = 0; j < weaponPanel.childCount; j++)
                        {
                            if (weaponPanel.GetChild(j).name == "Weapon")
                            {
                                Transform weaponEmpty = weaponPanel.GetChild(j);

                                weaponEmpty.GetComponentInChildren<Text>().text = target.GetComponent<Stats>().equippedWeapon.weaponName;
                            }
                            else if (weaponPanel.GetChild(j).name == "Arrow")
                            {
                                Image arrow = weaponPanel.GetChild(j).GetComponent<Image>();

                                if (triangleBonusEnemy == 15)
                                {
                                    arrow.gameObject.SetActive(true);
                                    arrow.transform.rotation = Quaternion.Euler(0, 0, 0);
                                    arrow.color = new Color32(105, 255, 102, 255);
                                }
                                else if (triangleBonusEnemy == 0)
                                {
                                    arrow.gameObject.SetActive(false);
                                }
                                else if (triangleBonusEnemy == -15)
                                {
                                    arrow.gameObject.SetActive(true);
                                    arrow.transform.rotation = Quaternion.Euler(0, 0, 180);
                                    arrow.color = new Color32(255, 104, 102, 255);
                                }
                            }
                        }
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
                                        else if (hpPanel.GetChild(j).GetChild(k).name == "HealthAfterAttack")
                                        {
                                            float healthAfterAttack = stats.hp - (enemyDamage * enemyDoubling);
                                            if (healthAfterAttack < 0)
                                            {
                                                healthAfterAttack = 0;
                                            }

                                            Image bar = hpPanel.GetChild(j).GetChild(k).GetComponent<Image>();
                                            bar.fillAmount = healthAfterAttack / stats.maxHP;
                                        }
                                        else if (hpPanel.GetChild(j).GetChild(k).name == "Text")
                                        {
                                            Text hp = hpPanel.GetChild(j).GetChild(k).GetComponent<Text>();
                                            hp.text = stats.hp.ToString();
                                        }
                                        else if (hpPanel.GetChild(j).GetChild(k).name == "TextAfterAttack")
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
            StartCoroutine(cursor.GetComponent<Cursor>().DoneCalcStats());

            if (gameObject.CompareTag("Enemy"))
            {
                cursor.GetComponent<Cursor>().EnemyFight(gameObject);                
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

    private float CalcAccuracy(float hit, float evasion, GameObject character)
    {
        float triangle = 0;
        if (character == gameObject)
        {
            triangle = triangleBonus;
        }
        else
        {
            triangle = triangleBonusEnemy;
        }
        
        float accuracy = hit - evasion + triangle;
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

    private float CalcEvade(GameObject character, int x, int y)
    {
        float evasion = 0;

        evasion = character.GetComponent<Stats>().spd * 2 + character.GetComponent<Stats>().luk + tileData.rowsMovement[-y].transform.GetChild(x).GetComponent<TileNumber>().terrainBonus + 0; //0's are the missing pieces
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
            if (firstChar == gameObject)
            {
                triangleBonus = weaponTriangle * 15;
            }
            else
            {
                triangleBonusEnemy = weaponTriangle * 15;
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
        Debug.Log("HitRate: " + hitRate);

        return hitRate;
    }
}
