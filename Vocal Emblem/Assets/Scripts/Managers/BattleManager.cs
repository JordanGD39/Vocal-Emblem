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
            healthTextEnemy.text = enemyBattle.hp.ToString("F0");
        }
        if (healthBarPlayer != null)
        {
            healthBarPlayer.fillAmount = playerBattle.hp / playerBattle.maxHP;
        }
        if (healthBarEnemy != null)
        {
            healthBarEnemy.fillAmount = enemyBattle.hp / enemyBattle.maxHP;
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
                            healthTextEnemy = uiThing.GetChild(j).GetComponentInChildren<Text>();
                            healthBarEnemy = uiThing.GetChild(j).GetChild(2).GetComponent<Image>();
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
                        playerSprite.GetComponent<AttackingInBattle>().stats = player;
                        enemySprite.GetComponent<AttackingInBattle>().stats = enemy;
                        break;
                    case 2:
                        battlePanel.GetChild(i).GetChild(0).transform.localPosition = new Vector3(-222, -46f, 0);
                        battlePanel.GetChild(i).GetChild(1).transform.localPosition = new Vector3(229, -46f, 0);
                        playerSprite = Instantiate(player.battlePrefab, battlePanel.GetChild(i), false);
                        enemySprite = Instantiate(enemy.battlePrefab, battlePanel.GetChild(i), false);
                        playerSprite.GetComponent<AttackingInBattle>().stats = player;
                        enemySprite.GetComponent<AttackingInBattle>().stats = enemy;
                        break;
                }
                if (distance > 2)
                {
                    battlePanel.GetChild(i).GetChild(0).transform.localPosition = new Vector3(-270, -46f, 0);
                    battlePanel.GetChild(i).GetChild(1).transform.localPosition = new Vector3(274, -46f, 0);
                    playerSprite = Instantiate(player.battlePrefab, battlePanel.GetChild(i), false);
                    enemySprite = Instantiate(enemy.battlePrefab, battlePanel.GetChild(i), false);
                    playerSprite.GetComponent<AttackingInBattle>().stats = player;
                    enemySprite.GetComponent<AttackingInBattle>().stats = enemy;
                }
            }
        }
        StartCoroutine(AttackOrder(player, enemy, damageHolder, playerSprite, enemySprite, distance));             
    }

    private IEnumerator AttackOrder(Stats player, Stats enemy, Stats damageHolder, GameObject playerSprite, GameObject enemySprite, float distance)
    {
        Attack(player, damageHolder.GetComponent<Attack>().damage, damageHolder.GetComponent<Attack>().acc, damageHolder.GetComponent<Attack>().crit, playerSprite, enemySprite);//1st player
        while (!playerSprite.GetComponent<AttackingInBattle>().done)
        {
            yield return new WaitForSeconds(0.5f);            
        }
        playerSprite.GetComponent<Animator>().Play("Idle");
        if (enemy.hp > 0 && distance <= enemy.equippedWeapon.range && enemy.equippedWeapon.rangeOneAndTwo || distance != 1 && distance <= enemy.equippedWeapon.range && !enemy.equippedWeapon.rangeOneAndTwo || enemy.equippedWeapon.counterAll)
        {
            Attack(enemy, damageHolder.GetComponent<Attack>().enemyDamage, damageHolder.GetComponent<Attack>().enemyAcc, damageHolder.GetComponent<Attack>().enemyCrit, enemySprite, playerSprite);//1st enemy
            while (!enemySprite.GetComponent<AttackingInBattle>().done)
            {
                yield return new WaitForSeconds(0.5f);                
            }
            enemySprite.GetComponent<Animator>().Play("Idle");
            if (player.hp <= 0)
            {
                Destroy(player.gameObject);
            }
        }
        else if(enemy.hp <= 0)
        {
            TileData tileData = GameObject.FindGameObjectWithTag("TileManager").GetComponent<TileData>();
            tileData.currMapCharPos[Mathf.RoundToInt(-enemy.transform.position.y + 0.5f), Mathf.RoundToInt(enemy.transform.position.x - 0.5f)] = 0;
            tileData.enemiesInGame.Remove(enemy.gameObject);
            Destroy(enemy.gameObject);
        }

        yield return new WaitForSeconds(1);
        Destroy(playerSprite);
        Destroy(enemySprite);
        cursor.battlePanel.SetActive(false);
        if (damageHolder == player)
        {
            GameObject.FindGameObjectWithTag("Canvas").GetComponent<SelectChoices>().Wait();
        }        
        //if (damageHolder.GetComponent<Attack>().doubling == 2)
        //{
        //    Attack(player, damageHolder.GetComponent<Attack>().damage, damageHolder.GetComponent<Attack>().acc, damageHolder.GetComponent<Attack>().crit, playerSprite, enemySprite);//2nd player
        //}
        //if (damageHolder.GetComponent<Attack>().enemyDoubling == 2)
        //{
        //    Attack(enemy, damageHolder.GetComponent<Attack>().enemyDamage, damageHolder.GetComponent<Attack>().enemyAcc, damageHolder.GetComponent<Attack>().enemyCrit, enemySprite, playerSprite);//2nd enemy
        //}
        //if (damageHolder.GetComponent<Attack>().doubling == 4)
        //{
        //    Attack(player, damageHolder.GetComponent<Attack>().damage, damageHolder.GetComponent<Attack>().acc, damageHolder.GetComponent<Attack>().crit, playerSprite, enemySprite);//2nd player
        //    if (damageHolder.GetComponent<Attack>().enemyDoubling == 2 || damageHolder.GetComponent<Attack>().enemyDoubling == 4)
        //    {
        //        Attack(enemy, damageHolder.GetComponent<Attack>().enemyDamage, damageHolder.GetComponent<Attack>().enemyAcc, damageHolder.GetComponent<Attack>().enemyCrit, enemySprite, playerSprite);//2nd enemy
        //    }
        //    Attack(player, damageHolder.GetComponent<Attack>().damage, damageHolder.GetComponent<Attack>().acc, damageHolder.GetComponent<Attack>().crit, playerSprite, enemySprite);//3rd player
        //    if (damageHolder.GetComponent<Attack>().enemyDoubling == 4)
        //    {
        //        Attack(enemy, damageHolder.GetComponent<Attack>().enemyDamage, damageHolder.GetComponent<Attack>().enemyAcc, damageHolder.GetComponent<Attack>().enemyCrit, enemySprite, playerSprite);//3rd enemy
        //    }
        //    Attack(player, damageHolder.GetComponent<Attack>().damage, damageHolder.GetComponent<Attack>().acc, damageHolder.GetComponent<Attack>().crit, playerSprite, enemySprite);//4th player
        //    if (damageHolder.GetComponent<Attack>().enemyDoubling == 4)
        //    {
        //        Attack(enemy, damageHolder.GetComponent<Attack>().enemyDamage, damageHolder.GetComponent<Attack>().enemyAcc, damageHolder.GetComponent<Attack>().enemyCrit, enemySprite, playerSprite);//4th enemy
        //    }
        //}
        //if (damageHolder.GetComponent<Attack>().enemyDoubling == 4 && damageHolder.GetComponent<Attack>().doubling != 4)
        //{
        //    Attack(enemy, damageHolder.GetComponent<Attack>().enemyDamage, damageHolder.GetComponent<Attack>().enemyAcc, damageHolder.GetComponent<Attack>().enemyCrit, enemySprite, playerSprite);//2nd enemy
        //    if (damageHolder.GetComponent<Attack>().doubling == 2)
        //    {
        //        Attack(player, damageHolder.GetComponent<Attack>().damage, damageHolder.GetComponent<Attack>().acc, damageHolder.GetComponent<Attack>().crit, playerSprite, enemySprite);//2nd player
        //    }
        //    Attack(enemy, damageHolder.GetComponent<Attack>().enemyDamage, damageHolder.GetComponent<Attack>().enemyAcc, damageHolder.GetComponent<Attack>().enemyCrit, enemySprite, playerSprite);//3rd enemy
        //    Attack(enemy, damageHolder.GetComponent<Attack>().enemyDamage, damageHolder.GetComponent<Attack>().enemyAcc, damageHolder.GetComponent<Attack>().enemyCrit, enemySprite, playerSprite);//4th enemy
        //}        
    }

    private void Attack(Stats attacker, float dmg, float hit, float crit, GameObject sprite, GameObject defendSprite)
    {
        defendSprite.GetComponent<BoxCollider2D>().enabled = true;

        defendSprite.GetComponent<AttackingInBattle>().damage = dmg;

        bool didHit = Randomizer(hit);
        bool didCrit = Randomizer(crit);

        if (!didHit)
        {
            defendSprite.GetComponentInChildren<Animator>().Play("Evade");
            defendSprite.GetComponent<BoxCollider2D>().enabled = false;
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

    public void CharGotHit(GameObject defender, float dmg, Stats stats)
    {
        if (defender.GetComponent<AttackingInBattle>().stats == playerBattle)
        {
            playerBattle.hp -= dmg;
        }
        else if (defender.GetComponent<AttackingInBattle>().stats == enemyBattle)
        {            
            enemyBattle.hp -= dmg;
            Debug.Log(enemyBattle.hp);
        }
    }
}
