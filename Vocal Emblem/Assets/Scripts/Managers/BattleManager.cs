using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    private Cursor cursor;

    private Text healthTextPlayer;
    private Text healthTextEnemy;
    private Image healthBarPlayer;
    private Image healthBarEnemy;

    private GameObject playerSprite;
    private GameObject enemySprite;

    private Stats playerBattle;
    private Stats enemyBattle;

    private void Start()
    {
        cursor = GameObject.FindGameObjectWithTag("Cursor").GetComponent<Cursor>();
    }

    private void Update()
    {
        if (healthTextPlayer != null)
        {
            healthTextPlayer.text = playerBattle.hp.ToString("F0");
        }
        if (healthTextEnemy != null)
        {

        }
        if (healthBarPlayer != null)
        {

        }
        if (healthBarEnemy != null)
        {

        }
    }

    public void Battle(Stats player, Stats enemy, float distance, bool playerAttack)
    {
        playerBattle = player;
        enemyBattle = enemy;
        Transform battlePanel = cursor.battlePanel.transform;

        Stats damageHolder = null;

        if (playerAttack)
        {
            damageHolder = player;
        }
        else
        {
            damageHolder = enemy;
        }

        for (int i = 0; i < battlePanel.childCount; i++)
        {
            if (battlePanel.GetChild(i).name == "UI")
            {
                Transform uiThing = battlePanel.GetChild(i);

                for (int j = 0; j < uiThing.childCount; j++)
                {
                    switch (uiThing.GetChild(j).name)
                    {
                        case "PlayerHealth":
                            healthTextPlayer = uiThing.GetChild(j).GetComponentInChildren<Text>();
                            healthBarPlayer = uiThing.GetChild(j).GetChild(2).GetComponent<Image>();
                            uiThing.GetChild(j).GetComponentInChildren<Text>().text = player.hp.ToString();
                            uiThing.GetChild(j).GetChild(2).GetComponent<Image>().fillAmount = player.hp / player.maxHP;
                            break;
                        case "EnemyHealth":
                            uiThing.GetChild(j).GetComponentInChildren<Text>().text = enemy.hp.ToString();
                            uiThing.GetChild(j).GetChild(2).GetComponent<Image>().fillAmount = enemy.hp / enemy.maxHP;
                            break;
                        case "PlayerStats":
                            uiThing.GetChild(j).GetChild(2).GetComponent<Text>().text = damageHolder.GetComponent<Attack>().damage.ToString("F0");
                            uiThing.GetChild(j).GetChild(4).GetComponent<Text>().text = damageHolder.GetComponent<Attack>().acc.ToString("F0");
                            uiThing.GetChild(j).GetChild(6).GetComponent<Text>().text = damageHolder.GetComponent<Attack>().crit.ToString("F0");
                            break;
                        case "EnemyStats":
                            uiThing.GetChild(j).GetChild(2).GetComponent<Text>().text = damageHolder.GetComponent<Attack>().enemyDamage.ToString("F0");
                            uiThing.GetChild(j).GetChild(4).GetComponent<Text>().text = damageHolder.GetComponent<Attack>().enemyAcc.ToString("F0");
                            uiThing.GetChild(j).GetChild(6).GetComponent<Text>().text = damageHolder.GetComponent<Attack>().enemyCrit.ToString("F0");
                            break;
                        case "PlayerName":
                            uiThing.GetChild(j).GetChild(1).GetComponent<Text>().text = player.charName;
                            uiThing.GetChild(j).GetChild(2).GetComponent<Text>().text = player.equippedWeapon.weaponName;
                            break;
                        case "EnemyName":
                            uiThing.GetChild(j).GetChild(1).GetComponent<Text>().text = enemy.charName;
                            uiThing.GetChild(j).GetChild(2).GetComponent<Text>().text = enemy.equippedWeapon.weaponName;
                            break;
                    }                    
                }
            }
            else if (battlePanel.GetChild(i).name == "Battle")
            {
                switch (distance)
                {
                    case 1:
                        battlePanel.GetChild(i).GetChild(0).transform.localPosition = new Vector3(-124.8f, -46f, 0);
                        battlePanel.GetChild(i).GetChild(1).transform.localPosition = new Vector3(129.6f, -46f, 0);
                        playerSprite = Instantiate(player.battlePrefab, battlePanel.GetChild(i), false);
                        enemySprite = Instantiate(enemy.battlePrefab, battlePanel.GetChild(i), false);
                        break;
                    case 2:
                        battlePanel.GetChild(i).GetChild(0).transform.localPosition = new Vector3(-222, -46f, 0);
                        battlePanel.GetChild(i).GetChild(1).transform.localPosition = new Vector3(229, -46f, 0);
                        break;
                }
                if (distance > 2)
                {
                    battlePanel.GetChild(i).GetChild(0).transform.localPosition = new Vector3(-270, -46f, 0);
                    battlePanel.GetChild(i).GetChild(1).transform.localPosition = new Vector3(274, -46f, 0);
                }
            }
        }

        Attack(player, damageHolder.GetComponent<Attack>().damage, damageHolder.GetComponent<Attack>().acc, damageHolder.GetComponent<Attack>().crit, playerSprite, enemySprite);//1st player
        Attack(enemy, damageHolder.GetComponent<Attack>().enemyDamage, damageHolder.GetComponent<Attack>().enemyAcc, damageHolder.GetComponent<Attack>().enemyCrit, enemySprite, playerSprite);//1st enemy
        if (damageHolder.GetComponent<Attack>().doubling == 2)
        {
            Attack(player, damageHolder.GetComponent<Attack>().damage, damageHolder.GetComponent<Attack>().acc, damageHolder.GetComponent<Attack>().crit, playerSprite, enemySprite);//2nd player
        }
        if (damageHolder.GetComponent<Attack>().enemyDoubling == 2)
        {
            Attack(enemy, damageHolder.GetComponent<Attack>().enemyDamage, damageHolder.GetComponent<Attack>().enemyAcc, damageHolder.GetComponent<Attack>().enemyCrit, enemySprite, playerSprite);//2nd enemy
        }
        if(damageHolder.GetComponent<Attack>().doubling == 4)
        {
            Attack(player, damageHolder.GetComponent<Attack>().damage, damageHolder.GetComponent<Attack>().acc, damageHolder.GetComponent<Attack>().crit, playerSprite, enemySprite);//2nd player
            if (damageHolder.GetComponent<Attack>().enemyDoubling == 2 || damageHolder.GetComponent<Attack>().enemyDoubling == 4)
            {
                Attack(enemy, damageHolder.GetComponent<Attack>().enemyDamage, damageHolder.GetComponent<Attack>().enemyAcc, damageHolder.GetComponent<Attack>().enemyCrit, enemySprite, playerSprite);//2nd enemy
            }
            Attack(player, damageHolder.GetComponent<Attack>().damage, damageHolder.GetComponent<Attack>().acc, damageHolder.GetComponent<Attack>().crit, playerSprite, enemySprite);//3rd player
            if (damageHolder.GetComponent<Attack>().enemyDoubling == 4)
            {
                Attack(enemy, damageHolder.GetComponent<Attack>().enemyDamage, damageHolder.GetComponent<Attack>().enemyAcc, damageHolder.GetComponent<Attack>().enemyCrit, enemySprite, playerSprite);//3rd enemy
            }
            Attack(player, damageHolder.GetComponent<Attack>().damage, damageHolder.GetComponent<Attack>().acc, damageHolder.GetComponent<Attack>().crit, playerSprite, enemySprite);//4th player
            if (damageHolder.GetComponent<Attack>().enemyDoubling == 4)
            {
                Attack(enemy, damageHolder.GetComponent<Attack>().enemyDamage, damageHolder.GetComponent<Attack>().enemyAcc, damageHolder.GetComponent<Attack>().enemyCrit, enemySprite, playerSprite);//4th enemy
            }
        }
        if (damageHolder.GetComponent<Attack>().enemyDoubling == 4 && damageHolder.GetComponent<Attack>().doubling != 4)
        {
            Attack(enemy, damageHolder.GetComponent<Attack>().enemyDamage, damageHolder.GetComponent<Attack>().enemyAcc, damageHolder.GetComponent<Attack>().enemyCrit, enemySprite, playerSprite);//2nd enemy
            if (damageHolder.GetComponent<Attack>().doubling == 2)
            {
                Attack(player, damageHolder.GetComponent<Attack>().damage, damageHolder.GetComponent<Attack>().acc, damageHolder.GetComponent<Attack>().crit, playerSprite, enemySprite);//2nd player
            }
            Attack(enemy, damageHolder.GetComponent<Attack>().enemyDamage, damageHolder.GetComponent<Attack>().enemyAcc, damageHolder.GetComponent<Attack>().enemyCrit, enemySprite, playerSprite);//3rd enemy
            Attack(enemy, damageHolder.GetComponent<Attack>().enemyDamage, damageHolder.GetComponent<Attack>().enemyAcc, damageHolder.GetComponent<Attack>().enemyCrit, enemySprite, playerSprite);//4th enemy
        }
        
    }

    private void Attack(Stats attacker, float dmg, float hit, float crit, GameObject sprite, GameObject defendSprite)
    {
        defendSprite.transform.GetChild(0).GetComponent<BoxCollider2D>().enabled = true;

        defendSprite.GetComponent<AttackingInBattle>().damage = dmg;

        bool didHit = Randomizer(hit);
        bool didCrit = Randomizer(crit);

        if (!didHit)
        {
            defendSprite.GetComponentInChildren<Animator>().Play("Evade");
            defendSprite.transform.GetChild(0).GetComponent<BoxCollider2D>().enabled = false;
        }

        switch (attacker.equippedWeapon.typeOfWeapon)
        {
            case Weapon.WeaponType.SWORD:
                sprite.GetComponentInChildren<Animator>().Play("AttackSword");
                break;
            case Weapon.WeaponType.AXE:
                sprite.GetComponentInChildren<Animator>().Play("AttackSword");
                break;
            case Weapon.WeaponType.LANCE:
                sprite.GetComponentInChildren<Animator>().Play("AttackSword");
                break;
            case Weapon.WeaponType.BOW:
                sprite.GetComponentInChildren<Animator>().Play("AttackSword");
                break;
            case Weapon.WeaponType.SCREAM:
                sprite.GetComponentInChildren<Animator>().Play("AttackSword");
                break;
        }        

        Debug.Log(attacker.charName + " hit? " + didHit);
        Debug.Log(attacker.charName + " crit? " + didCrit);
    }

    private bool Randomizer(float number)
    {
        float rand = Random.Range(0, 100);
        bool hit = true;

        if (rand >= number)
        {
            hit = false;
        }
        else if (rand < number)
        {
            hit = true;
        }

        return hit;
    }

    public void CharGotHit(GameObject defender, float dmg)
    {
        if (playerBattle.battlePrefab == defender)
        {
            playerBattle.hp -= dmg;
        }
        else if (enemyBattle.battlePrefab == defender)
        {
            enemyBattle.hp -= dmg;
        }
    }
}
