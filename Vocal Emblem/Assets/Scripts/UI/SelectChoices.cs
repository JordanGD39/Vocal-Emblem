using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SelectChoices : MonoBehaviour
{
    private Cursor cursor;
    private TileData tileData;
    private BattleStateMachine BSM;

    [SerializeField] private GameObject itemButtonPrefab;

    public GameObject healChoice; 

    private void Start()
    {
        cursor = GameObject.FindGameObjectWithTag("Cursor").GetComponent<Cursor>();
        tileData = GameObject.FindGameObjectWithTag("TileManager").GetComponent<TileData>();
        BSM = GameObject.FindGameObjectWithTag("GM").GetComponent<BattleStateMachine>();
    }

    public void Attack()
    {
        cursor.doneCalc = false;
        tileData.DeselectMovement();
        cursor.currSelectedChar.GetComponent<Attack>().indexEnemies = 0;
        cursor.currSelectedChar.GetComponent<Attack>().CheckAttackRange(false);   
    }

    public void Items()
    {
        cursor.itemPanel.SetActive(true);
        cursor.selectPanel.SetActive(false);

        for (int i = 0; i < cursor.currSelectedChar.GetComponent<Stats>().weapons.Count; i++)
        {
            GameObject button = Instantiate(itemButtonPrefab, cursor.itemPanel.transform.GetChild(0), false);

            button.GetComponent<ItemButtonScript>().index = i;

            button.transform.GetChild(0).GetComponent<Text>().text = cursor.currSelectedChar.GetComponent<Stats>().weapons[i].weaponName;
            button.transform.GetChild(2).GetComponent<Text>().text = cursor.currSelectedChar.GetComponent<Stats>().weapons[i].uses.ToString();
            //button.transform.GetComponentInChildren<Image>()            
        }

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(cursor.itemPanel.transform.GetChild(0).GetChild(0).gameObject);
    }

    public void Heal()
    {
        cursor.doneCalc = false;
        tileData.DeselectMovement();
        cursor.currSelectedChar.GetComponent<Attack>().indexEnemies = 0;
        cursor.currSelectedChar.GetComponent<Attack>().CheckAttackRange(true);
    }

    public void Wait()
    {
        cursor.currSelectedChar.GetComponent<PlayerMovement>().wait = true;
        tileData.currMapCharPos[Mathf.RoundToInt(-cursor.currSelectedChar.transform.position.y + 0.5f), Mathf.RoundToInt(cursor.currSelectedChar.transform.position.x - 0.5f)] = 1;
        tileData.currMapCharPos[Mathf.RoundToInt(-cursor.currSelectedChar.GetComponent<PlayerMovement>().oldPos.y + 0.5f), Mathf.RoundToInt(cursor.currSelectedChar.GetComponent<PlayerMovement>().oldPos.x - 0.5f)] = 0;
        Debug.Log(tileData.currMapCharPos[4,10] + " "+ tileData.currMapCharPos[4, 12]);
        StartCoroutine(WaitForSetActive());
        BSM.CheckPlayerWait();
        tileData.DeselectMovement();
    }

    public void SetCurrentItem(int i)
    {
        cursor.currSelectedChar.GetComponent<Stats>().equippedWeapon = cursor.currSelectedChar.GetComponent<Stats>().weapons[i];
    }

    private IEnumerator WaitForSetActive()
    {
        yield return new WaitForSeconds(0.05f);
        cursor.currSelectedChar.GetComponent<BoxCollider2D>().enabled = true;
        cursor.currSelectedChar = null;
        cursor.selectPanel.SetActive(false);        
    }
}