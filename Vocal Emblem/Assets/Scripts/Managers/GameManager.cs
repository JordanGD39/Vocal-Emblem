using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public List<GameObject> playerTeam = new List<GameObject>();
    public List<GameObject> allyTeam = new List<GameObject>();
    public List<GameObject> enemies = new List<GameObject>();
    public int playerTeamCount = 0;

    public static GameManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

    public int EXPcalc(Stats player, Stats enemy)
    {
        int xp = 0;

        int levelDiff = enemy.level - player.level;

        if (levelDiff < 0)
        {
            xp = 1;
        }
        else if (levelDiff >= 3 && levelDiff <= 6)
        {
            xp = 10;
        }
        else if(levelDiff > 12)
        {
            xp = 30;
        }
        switch (levelDiff)
        {
            case 0:
                xp = 3;
                break;
            case 1:
                xp = 6;
                break;
            case 2:
                xp = 8;
                break;
            case 7:
                xp = 13;
                break;
            case 8:
                xp = 16;
                break;
            case 9:
                xp = 19;
                break;
            case 10:
                xp = 22;
                break;
            case 11:
                xp = 25;
                break;
            case 12:
                xp = 28;
                break;
        }

        return xp;
    }
}
