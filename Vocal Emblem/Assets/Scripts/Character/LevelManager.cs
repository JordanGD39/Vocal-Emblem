using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private Stats stats;
    private StatGrowths statGrowth;

    // Start is called before the first frame update
    void Start()
    {
        stats = GetComponent<Stats>();
        statGrowth = GetComponent<StatGrowths>();
    }

    //Randomizing stat growth by growth rates
    public void RandomStatsGrowth()
    {
        stats.maxHP += StatCalc(statGrowth.hpGrowth);
        stats.maxStr += StatCalc(statGrowth.strGrowth);
        stats.str = stats.maxStr;
        stats.maxScr += StatCalc(statGrowth.scrGrowth);
        stats.scr = stats.maxScr;
        stats.maxSkill += StatCalc(statGrowth.skillGrowth);
        stats.skill = stats.maxSkill;
        stats.maxSpd += StatCalc(statGrowth.spdGrowth);
        stats.spd = stats.maxSpd;
        stats.maxDef += StatCalc(statGrowth.defGrowth);
        stats.def = stats.maxDef;
        stats.maxRes += StatCalc(statGrowth.resGrowth);
        stats.res = stats.maxRes;
        stats.maxLuk += StatCalc(statGrowth.lukGrowth);
        stats.luk = stats.maxLuk;
    }

    private int StatCalc(int percentage)
    {
        int up = 0;

        float rand = Random.Range(0, 100);

        if (percentage >= rand)
        {
            up = 1;
            Debug.Log("Stat up!");
        }

        if (up == 0)
        {
            Debug.Log("Stat not up");
        }

        return up;
    }
}
