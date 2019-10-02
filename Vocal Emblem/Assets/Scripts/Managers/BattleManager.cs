using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    private Cursor cursor;

    private GameObject healthTextPlayer;
    private GameObject healthTextEnemy;
    private GameObject healthBarPlayer;
    private GameObject healthBarEnemy;

    private void Start()
    {
        cursor = GameObject.FindGameObjectWithTag("Cursor").GetComponent<Cursor>();
    }

    public void Battle(Stats player, Stats enemy, float distance, bool playerAttack)
    {
        Transform battlePanel = cursor.battlePanel.transform;

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
                            uiThing.GetChild(j).GetComponentInChildren<Text>().text = player.hp.ToString();
                            uiThing.GetChild(j).GetChild(2).GetComponent<Image>().fillAmount = player.hp / player.maxHP;
                            break;
                        case "EnemyHealth":
                            uiThing.GetChild(j).GetComponentInChildren<Text>().text = enemy.hp.ToString();
                            uiThing.GetChild(j).GetChild(2).GetComponent<Image>().fillAmount = enemy.hp / enemy.maxHP;
                            break;
                        case "PlayerStats":
                            uiThing.GetChild(j).GetChild(2).GetComponent<Text>().text = player.GetComponent<Attack>().damage.ToString("F0");
                            uiThing.GetChild(j).GetChild(4).GetComponent<Text>().text = player.GetComponent<Attack>().acc.ToString("F0");
                            uiThing.GetChild(j).GetChild(6).GetComponent<Text>().text = player.GetComponent<Attack>().crit.ToString("F0");
                            break;
                        case "EnemyStats":
                            uiThing.GetChild(j).GetChild(2).GetComponent<Text>().text = player.GetComponent<Attack>().enemyDamage.ToString("F0");
                            uiThing.GetChild(j).GetChild(4).GetComponent<Text>().text = player.GetComponent<Attack>().enemyAcc.ToString("F0");
                            uiThing.GetChild(j).GetChild(6).GetComponent<Text>().text = player.GetComponent<Attack>().enemyCrit.ToString("F0");
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

        if (playerAttack)
        {
            Attack(player, player.GetComponent<Attack>().damage, player.GetComponent<Attack>().acc, player.GetComponent<Attack>().crit);//1st player
            Attack(enemy, player.GetComponent<Attack>().enemyDamage, player.GetComponent<Attack>().enemyAcc, player.GetComponent<Attack>().enemyCrit);//1st enemy
            if (player.GetComponent<Attack>().doubling == 2)
            {
                Attack(player, player.GetComponent<Attack>().damage, player.GetComponent<Attack>().acc, player.GetComponent<Attack>().crit);//2nd player
            }
            if (player.GetComponent<Attack>().enemyDoubling == 2)
            {
                Attack(enemy, player.GetComponent<Attack>().enemyDamage, player.GetComponent<Attack>().enemyAcc, player.GetComponent<Attack>().enemyCrit);//2nd enemy
            }
            if(player.GetComponent<Attack>().doubling == 4)
            {
                Attack(player, player.GetComponent<Attack>().damage, player.GetComponent<Attack>().acc, player.GetComponent<Attack>().crit);//2nd player
                if (player.GetComponent<Attack>().enemyDoubling == 2 || player.GetComponent<Attack>().enemyDoubling == 4)
                {
                    Attack(enemy, player.GetComponent<Attack>().enemyDamage, player.GetComponent<Attack>().enemyAcc, player.GetComponent<Attack>().enemyCrit);//2nd enemy
                }
                Attack(player, player.GetComponent<Attack>().damage, player.GetComponent<Attack>().acc, player.GetComponent<Attack>().crit);//3rd player
                if (player.GetComponent<Attack>().enemyDoubling == 4)
                {
                    Attack(enemy, player.GetComponent<Attack>().enemyDamage, player.GetComponent<Attack>().enemyAcc, player.GetComponent<Attack>().enemyCrit);//3rd enemy
                }
                Attack(player, player.GetComponent<Attack>().damage, player.GetComponent<Attack>().acc, player.GetComponent<Attack>().crit);//4th player
                if (player.GetComponent<Attack>().enemyDoubling == 4)
                {
                    Attack(enemy, player.GetComponent<Attack>().enemyDamage, player.GetComponent<Attack>().enemyAcc, player.GetComponent<Attack>().enemyCrit);//4th enemy
                }
            }
            if (player.GetComponent<Attack>().enemyDoubling == 4 && player.GetComponent<Attack>().doubling != 4)
            {
                Attack(enemy, player.GetComponent<Attack>().enemyDamage, player.GetComponent<Attack>().enemyAcc, player.GetComponent<Attack>().enemyCrit);//2nd enemy
                if (player.GetComponent<Attack>().doubling == 2)
                {
                    Attack(player, player.GetComponent<Attack>().damage, player.GetComponent<Attack>().acc, player.GetComponent<Attack>().crit);//2nd player
                }
                Attack(enemy, player.GetComponent<Attack>().enemyDamage, player.GetComponent<Attack>().enemyAcc, player.GetComponent<Attack>().enemyCrit);//3rd enemy
                Attack(enemy, player.GetComponent<Attack>().enemyDamage, player.GetComponent<Attack>().enemyAcc, player.GetComponent<Attack>().enemyCrit);//4th enemy
            }
        }
        else
        {

        }
        
    }

    private void Attack(Stats attacker, float dmg, float hit, float crit)
    {
        bool didHit = Randomizer(hit);
        bool didCrit = Randomizer(crit);
    }

    private bool Randomizer(float number)
    {
        float rand = Random.Range(0, 100);
        bool hit = true;

        if (rand > number)
        {
            hit = false;
        }
        else if (rand <= number)
        {
            hit = true;
        }

        return hit;
    }
}
