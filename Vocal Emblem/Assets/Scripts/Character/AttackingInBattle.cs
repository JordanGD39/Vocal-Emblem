using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackingInBattle : MonoBehaviour
{
    public float damage;

    private BattleManager BSM;

    private void Start()
    {
        BSM = GameObject.FindGameObjectWithTag("BSM").GetComponent<BattleManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("AttackHit"))
        {
            BSM.CharGotHit(gameObject, damage);
        }
    }
}
