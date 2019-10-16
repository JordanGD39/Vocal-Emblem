using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    private Cursor cursor;
    private TileData tileData;
    private BattleStateMachine BSM;

    private Text healthTextPlayer;
    private Text healthTextEnemy;
    private Image healthBarPlayer;
    private Image healthBarEnemy;

    private GameObject playerSprite;
    private GameObject enemySprite;

    private Stats playerBattle;
    private Stats enemyBattle;

    private float playerHPtarget = 99;
    private float enemyHPtarget = 99;

    private void Start()
    {
        cursor = GameObject.FindGameObjectWithTag("Cursor").GetComponent<Cursor>();
        tileData = GameObject.FindGameObjectWithTag("TileManager").GetComponent<TileData>();
        BSM = GameManager.instance.GetComponent<BattleStateMachine>();
    }

    private void Update()
    {
        if (healthTextPlayer != null)
        {
            healthTextPlayer.text = playerBattle.hp.ToString("F0");
            if (playerHPtarget < playerBattle.hp)
            {
                playerBattle.hp -= 1;
            }
            else
            {
                playerBattle.hp = playerHPtarget;
            }
        }
        if (healthTextEnemy != null)
        {
            healthTextEnemy.text = enemyBattle.hp.ToString("F0");
            if (enemyHPtarget < enemyBattle.hp)
            {
                enemyBattle.hp -= 1;
            }
            else
            {
                enemyBattle.hp = enemyHPtarget;
            }
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
        playerHPtarget = playerBattle.hp;
        enemyHPtarget = enemyBattle.hp;
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
                            if (damageHolder == player)
                            {
                                uiThing.GetChild(j).GetChild(2).GetComponent<Text>().text = damageHolder.GetComponent<Attack>().damage.ToString("F0");
                                uiThing.GetChild(j).GetChild(4).GetComponent<Text>().text = damageHolder.GetComponent<Attack>().acc.ToString("F0");
                                uiThing.GetChild(j).GetChild(6).GetComponent<Text>().text = damageHolder.GetComponent<Attack>().crit.ToString("F0");
                            }
                            else
                            {
                                uiThing.GetChild(j).GetChild(2).GetComponent<Text>().text = damageHolder.GetComponent<Attack>().enemyDamage.ToString("F0");
                                uiThing.GetChild(j).GetChild(4).GetComponent<Text>().text = damageHolder.GetComponent<Attack>().enemyAcc.ToString("F0");
                                uiThing.GetChild(j).GetChild(6).GetComponent<Text>().text = damageHolder.GetComponent<Attack>().enemyCrit.ToString("F0");
                            }                            
                            break;
                        case "EnemyStats":
                            if (damageHolder == enemy)
                            {
                                uiThing.GetChild(j).GetChild(2).GetComponent<Text>().text = damageHolder.GetComponent<Attack>().damage.ToString("F0");
                                uiThing.GetChild(j).GetChild(4).GetComponent<Text>().text = damageHolder.GetComponent<Attack>().acc.ToString("F0");
                                uiThing.GetChild(j).GetChild(6).GetComponent<Text>().text = damageHolder.GetComponent<Attack>().crit.ToString("F0");
                            }
                            else
                            {
                                uiThing.GetChild(j).GetChild(2).GetComponent<Text>().text = damageHolder.GetComponent<Attack>().enemyDamage.ToString("F0");
                                uiThing.GetChild(j).GetChild(4).GetComponent<Text>().text = damageHolder.GetComponent<Attack>().enemyAcc.ToString("F0");
                                uiThing.GetChild(j).GetChild(6).GetComponent<Text>().text = damageHolder.GetComponent<Attack>().enemyCrit.ToString("F0");
                            }
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
                        playerSprite.transform.localPosition = new Vector3(-135.3f, -0.5f, 0);
                        enemySprite.GetComponent<AttackingInBattle>().stats = enemy;
                        enemySprite.transform.localPosition = new Vector3(126.2f, -0.5f, 0);
                        break;
                    case 2:
                        battlePanel.GetChild(i).GetChild(0).transform.localPosition = new Vector3(-222, -46f, 0);
                        battlePanel.GetChild(i).GetChild(1).transform.localPosition = new Vector3(229, -46f, 0);
                        playerSprite = Instantiate(player.battlePrefab, battlePanel.GetChild(i), false);
                        enemySprite = Instantiate(enemy.battlePrefab, battlePanel.GetChild(i), false);
                        playerSprite.GetComponent<AttackingInBattle>().stats = player;
                        enemySprite.GetComponent<AttackingInBattle>().stats = enemy;
                        playerSprite.transform.localPosition = new Vector3(-232, -0.5f, 0);
                        enemySprite.transform.localPosition = new Vector3(226, -0.5f, 0);
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
                    playerSprite.transform.localPosition = new Vector3(-280, -0.5f, 0);
                    enemySprite.transform.localPosition = new Vector3(271, -0.5f, 0);
                }
            }
        }

        if (damageHolder == player)
        {
            StartCoroutine(AttackOrder(player, enemy, damageHolder, playerSprite, enemySprite, distance, 1, false));
        }
        else if (damageHolder == enemy)
        {
            StartCoroutine(AttackOrder(enemy, player, damageHolder, enemySprite, playerSprite, distance, 1, false));
        }                     
    }

    private IEnumerator AttackOrder(Stats player, Stats enemy, Stats damageHolder, GameObject playerSprite, GameObject enemySprite, float distance, int i, bool twiceFourDoubling)
    {
        Debug.Log(playerSprite);
        Debug.Log(enemySprite);
        playerSprite.GetComponent<AttackingInBattle>().done = false;
        enemySprite.GetComponent<AttackingInBattle>().done = false;
        Attack(player, damageHolder.GetComponent<Attack>().damage, damageHolder.GetComponent<Attack>().acc, damageHolder.GetComponent<Attack>().crit, playerSprite, enemySprite);//1st player
        while (!playerSprite.GetComponent<AttackingInBattle>().done)
        {
            yield return new WaitForSeconds(0.2f);            
        }
        playerSprite.GetComponent<Animator>().Play("Idle");
        if (damageHolder.GetComponent<Attack>().enemyDoubling == i && enemy.hp > 0 && distance <= enemy.equippedWeapon.range && enemy.equippedWeapon.rangeOneAndTwo || distance != 1 && distance <= enemy.equippedWeapon.range && !enemy.equippedWeapon.rangeOneAndTwo || enemy.equippedWeapon.counterAll)
        {
            enemySprite.GetComponent<AttackingInBattle>().done = false;
            playerSprite.GetComponent<AttackingInBattle>().done = false;
            Attack(enemy, damageHolder.GetComponent<Attack>().enemyDamage, damageHolder.GetComponent<Attack>().enemyAcc, damageHolder.GetComponent<Attack>().enemyCrit, enemySprite, playerSprite);//1st enemy
            while (!enemySprite.GetComponent<AttackingInBattle>().done)
            {
                yield return new WaitForSeconds(0.2f);                
            }
            enemySprite.GetComponent<Animator>().Play("Idle");
            if (player.hp <= 0)
            {
                tileData.currMapCharPos[Mathf.RoundToInt(-player.GetComponent<PlayerMovement>().oldPos.y + 0.5f), Mathf.RoundToInt(player.GetComponent<PlayerMovement>().oldPos.x - 0.5f)] = 0;
                tileData.currMapCharPos[Mathf.RoundToInt(-player.transform.position.y + 0.5f), Mathf.RoundToInt(player.transform.position.x - 0.5f)] = 0;

                if (damageHolder == playerBattle)
                {
                    tileData.players.Remove(player.gameObject);
                }
                else
                {
                    tileData.enemiesInGame.Remove(player.gameObject);
                }

                Destroy(player.gameObject);

                yield return new WaitForSeconds(1);
                Destroy(playerSprite);
                Destroy(enemySprite);
                cursor.battlePanel.SetActive(false);
                tileData.DeselectMovement();

                if(damageHolder == enemyBattle)
                {
                    tileData.DeselectMovement();
                    BSM.GiveTurnToAI();
                }
            }
        }
        else if(enemy.hp <= 0)
        {
            tileData.currMapCharPos[Mathf.RoundToInt(-enemy.transform.position.y + 0.5f), Mathf.RoundToInt(enemy.transform.position.x - 0.5f)] = 0;            
            if (damageHolder == playerBattle)
            {
                tileData.enemiesInGame.Remove(enemy.gameObject);
            }
            else
            {
                tileData.players.Remove(enemy.gameObject);
            }
            Destroy(enemy.gameObject);
        }

        yield return new WaitForSeconds(0.2f);
        if (damageHolder != null)
        {
            if (player.hp > 0 && enemy.hp > 0 && damageHolder.GetComponent<Attack>().doubling == 2 && i == 1 || enemy.hp > 0 && damageHolder.GetComponent<Attack>().doubling == 4 && i == 1 || enemy.hp > 0 && damageHolder.GetComponent<Attack>().doubling == 4 && i == 2 || enemy.hp > 0 && damageHolder.GetComponent<Attack>().doubling == 4 && i == 4 && twiceFourDoubling)
            {
                int j = 0;
                switch (i)
                {
                    case 1:
                        j = 2;
                        break;
                    case 2:
                        j = 4;
                        twiceFourDoubling = true;
                        break;
                    case 4:
                        twiceFourDoubling = false;
                        break;
                }
                StartCoroutine(AttackOrder(player, enemy, damageHolder, playerSprite, enemySprite, distance, j, twiceFourDoubling));
            }
            else
            {
                yield return new WaitForSeconds(1);
                Destroy(playerSprite);
                Destroy(enemySprite);
                cursor.battlePanel.SetActive(false);

                if (damageHolder == playerBattle)
                {
                    GameObject.FindGameObjectWithTag("Canvas").GetComponent<SelectChoices>().Wait();
                }
                else
                {
                    enemyBattle.GetComponent<EnemyAI>().wait = true;
                    tileData.DeselectMovement();
                    BSM.enemyIndex++;
                    BSM.GiveTurnToAI();
                }
            }
        }
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
        if (didCrit)
        {
            defendSprite.GetComponent<AttackingInBattle>().damage *= 3;
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
            case Weapon.WeaponType.STAFF:
                sprite.GetComponentInChildren<Animator>().Play("AttackSword");
                defendSprite.GetComponent<AttackingInBattle>().damage = -dmg;
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
            playerHPtarget = playerBattle.hp;
            playerHPtarget -= dmg;
            if (playerHPtarget < 0)
            {
                playerHPtarget = 0;
            }
            else if (playerHPtarget > playerBattle.maxHP)
            {
                playerHPtarget = playerBattle.maxHP;
            }
        }
        else if (defender.GetComponent<AttackingInBattle>().stats == enemyBattle)
        {
            enemyHPtarget = enemyBattle.hp;
            enemyHPtarget -= dmg;
            if (enemyHPtarget < 0)
            {
                enemyHPtarget = 0;
            }
            else if (enemyHPtarget > enemyBattle.maxHP)
            {
                enemyHPtarget = enemyBattle.maxHP;
            }
        }
    }
}
