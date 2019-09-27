using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleStateMachine : MonoBehaviour
{
    public enum battlePhase { PLAYER, ALLY, ENEMY};
    public battlePhase phase;

    private GameObject cursor;

    // Start is called before the first frame update
    void Start()
    {
        cursor = GameObject.FindGameObjectWithTag("Cursor");
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
}
