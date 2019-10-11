using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleStateMachine : MonoBehaviour
{
    public enum battlePhase { PLAYER, ALLY, ENEMY};
    public battlePhase phase;

    private GameObject cursor;
    private TileData tileData;
    private bool allAreWaiting;
    private bool checkingWait = true;

    public int enemyIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        cursor = GameObject.FindGameObjectWithTag("Cursor");
        tileData = GameObject.FindGameObjectWithTag("TileManager").GetComponent<TileData>();
        phase = battlePhase.PLAYER;
    }

    // Update is called once per frame
    void Update()
    {
        switch (phase)
        {
            case battlePhase.PLAYER:
                cursor.SetActive(true);
                if (!checkingWait && allAreWaiting)
                {
                    phase = battlePhase.ALLY;
                }
                break;
            case battlePhase.ALLY:
                cursor.SetActive(false);
                if (tileData.alliesInGame.Count == 0)
                {
                    phase = battlePhase.ENEMY;
                    RemoveWaitEnemy();
                    GiveTurnToAI();
                }
                break;
            case battlePhase.ENEMY:
                cursor.SetActive(false);
                if (tileData.enemiesInGame.Count == 0)
                {
                    RemoveWaitPlayer();
                    allAreWaiting = false;
                    phase = battlePhase.PLAYER;
                }
                break;
        }
    }

    public void CheckPlayerWait()
    {
        checkingWait = true;
        allAreWaiting = true;

        for (int i = 0; i < tileData.players.Count; i++)
        {
            if (!tileData.players[i].GetComponent<PlayerMovement>().wait)
            {
                allAreWaiting = false;
            }
        }

        checkingWait = false;
    }

    private void RemoveWaitPlayer()
    {
        for (int i = 0; i < tileData.players.Count; i++)
        {
            tileData.players[i].GetComponent<PlayerMovement>().wait = false;            
        }
    }
    private void RemoveWaitEnemy()
    {
        for (int i = 0; i < tileData.enemiesInGame.Count; i++)
        {
            tileData.enemiesInGame[i].GetComponent<EnemyAI>().wait = false;            
        }
    }

    public void GiveTurnToAI()
    {
        if (tileData.enemiesInGame.Count > 0 && enemyIndex < tileData.enemiesInGame.Count)
        {
            tileData.enemiesInGame[enemyIndex].GetComponent<EnemyAI>().GetTarget();
        }    
        else
        {
            RemoveWaitPlayer();
            allAreWaiting = false;
            phase = battlePhase.PLAYER;
        }
    }
}
