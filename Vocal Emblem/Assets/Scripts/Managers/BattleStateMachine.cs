using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleStateMachine : MonoBehaviour
{
    public enum battlePhase { PLAYER, ALLY, ENEMY};
    public battlePhase phase;

    private GameObject cursor;
    private TileData tileData;

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
                break;
            case battlePhase.ALLY:
                cursor.SetActive(false);
                break;
            case battlePhase.ENEMY:
                cursor.SetActive(false);
                break;
        }
    }

    public bool CheckPlayerWait()
    {
        bool allAreWaiting = true;

        for (int i = 0; i < tileData.players.Count; i++)
        {
            if (!tileData.players[i].GetComponent<PlayerMovement>().wait)
            {
                allAreWaiting = false;
            }
        }

        return allAreWaiting;
    }
}
