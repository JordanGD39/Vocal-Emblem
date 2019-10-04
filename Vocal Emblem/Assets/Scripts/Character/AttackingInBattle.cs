using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackingInBattle : MonoBehaviour
{
    public float damage;
    public bool done;

    private BattleManager BM;
    public Stats stats;

    private void Start()
    {
        BM = GameObject.FindGameObjectWithTag("BM").GetComponent<BattleManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("AttackHit"))
        {
            Debug.Log("OOOOOFFFFF" + gameObject);
            BM.CharGotHit(gameObject, damage, stats);
        }
    }

    public void AnimDone(string message)
    {
        if (message.Equals("AnimEnded"))
        {
            done = true;
        }
    }
}
