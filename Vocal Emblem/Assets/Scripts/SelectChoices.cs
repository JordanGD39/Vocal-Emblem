using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectChoices : MonoBehaviour
{
    private Cursor cursor;
    private TileData tileData;

    private void Start()
    {
        cursor = GameObject.FindGameObjectWithTag("Cursor").GetComponent<Cursor>();
        tileData = GameObject.FindGameObjectWithTag("TileManager").GetComponent<TileData>();
    }

    public void Attack()
    {
        cursor.doneCalc = false;
        tileData.DeselectMovement();
        cursor.currSelectedChar.GetComponent<Attack>().CheckAttackRange();        
    }
    public void Items()
    {

    }
    public void Wait()
    {
        cursor.currSelectedChar.GetComponent<PlayerMovement>().wait = true;
        tileData.currMapCharPos[Mathf.RoundToInt(-cursor.currSelectedChar.transform.position.y + 0.5f), Mathf.RoundToInt(cursor.currSelectedChar.transform.position.x - 0.5f)] = 1;
        tileData.currMapCharPos[Mathf.RoundToInt(-cursor.currSelectedChar.GetComponent<PlayerMovement>().oldPos.y + 0.5f), Mathf.RoundToInt(cursor.currSelectedChar.GetComponent<PlayerMovement>().oldPos.x - 0.5f)] = 0;
        Debug.Log(tileData.currMapCharPos[4,10] + " "+ tileData.currMapCharPos[4, 12]);
        StartCoroutine(WaitForSetActive());
        tileData.DeselectMovement();
    }

    private IEnumerator WaitForSetActive()
    {
        yield return new WaitForSeconds(0.05f);
        cursor.currSelectedChar.GetComponent<BoxCollider2D>().enabled = true;
        cursor.currSelectedChar = null;
        cursor.selectPanel.SetActive(false);        
    }
}
