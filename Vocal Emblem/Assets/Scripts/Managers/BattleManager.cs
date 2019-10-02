using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    private Cursor cursor;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Battle(Stats attacker, Stats defender)
    {
        Transform battlePanel = cursor.GetComponent<Cursor>().battlePanel.transform;

        for (int i = 0; i < battlePanel.childCount; i++)
        {
            if (battlePanel.GetChild(i).name == "UI")
            {
                Transform uiThing = battlePanel.GetChild(i);

                for (int j = 0; j < uiThing.childCount; j++)
                {
                    if (uiThing.GetChild(j).name == "PlayerHealth")
                    {
                        uiThing.GetChild(j).GetComponentInChildren<Text>().text = attacker.hp.ToString();
                        uiThing.GetChild(j).GetChild(2).GetComponent<Image>().fillAmount = attacker.hp / attacker.maxHP;
                    }
                    else if (uiThing.GetChild(j).name == "EnemyHealth")
                    {
                        uiThing.GetChild(j).GetComponentInChildren<Text>().text = defender.hp.ToString();
                        uiThing.GetChild(j).GetChild(2).GetComponent<Image>().fillAmount = defender.hp / defender.maxHP;
                    }
                }
            }
            else if (battlePanel.GetChild(i).name == "Battle")
            {

            }
        }
    }
}
