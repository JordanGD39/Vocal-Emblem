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
                break;
            case battlePhase.ENEMY:
                cursor.SetActive(false);
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
}
