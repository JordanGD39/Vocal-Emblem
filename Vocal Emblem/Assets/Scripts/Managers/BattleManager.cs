using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    private Cursor cursor;

    private void Start()
    {
        cursor = GameObject.FindGameObjectWithTag("Cursor").GetComponent<Cursor>();
    }

    public void Battle(Stats attacker, Stats defender, float distance)
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
                            uiThing.GetChild(j).GetComponentInChildren<Text>().text = attacker.hp.ToString();
                            uiThing.GetChild(j).GetChild(2).GetComponent<Image>().fillAmount = attacker.hp / attacker.maxHP;
                            break;
                        case "EnemyHealth":
                            uiThing.GetChild(j).GetComponentInChildren<Text>().text = defender.hp.ToString();
                            uiThing.GetChild(j).GetChild(2).GetComponent<Image>().fillAmount = defender.hp / defender.maxHP;
                            break;
                    }                    
                }
            }
            else if (battlePanel.GetChild(i).name == "Battle")
            {

            }
        }
    }
}
