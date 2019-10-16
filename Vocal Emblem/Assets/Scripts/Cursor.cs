using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Cursor : MonoBehaviour
{
    private TileData tileData;
    private BattleManager BM;

    private bool posSet = false;
    public bool doneCalc = false;

    public GameObject currSelectedChar;
    public GameObject selectPanel;
    public GameObject attackPanel;
    public GameObject battlePanel;

    private List<GameObject> openList = new List<GameObject>();
    private List<GameObject> closedList = new List<GameObject>();

    public GameObject current;

    // Start is called before the first frame update
    void Start()
    {
        tileData = GameObject.FindGameObjectWithTag("TileManager").GetComponent<TileData>();
        BM = GameObject.FindGameObjectWithTag("BM").GetComponent<BattleManager>();
        attackPanel.SetActive(false);
        selectPanel.SetActive(false);
        battlePanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up, 0.1f);
        if (tileData.doneLoading && !posSet)
        {
            transform.position = tileData.players[0].transform.position;
            posSet = true;
        }        
        Controls(hit);

        if (currSelectedChar != null && !selectPanel.activeSelf && !attackPanel.activeSelf)
        {
            PathFinding(hit);
        }
    }

    private void Controls(RaycastHit2D hit)
    {
        if (!selectPanel.activeSelf && !attackPanel.activeSelf)
        {
            if (Input.GetButtonDown("Horizontal") && Input.GetAxis("Horizontal") > 0)
            {
                transform.position = new Vector3(transform.position.x + 1, transform.position.y);
            }
            else if (Input.GetButtonDown("Horizontal") && (Input.GetAxis("Horizontal") < 0))
            {
                transform.position = new Vector3(transform.position.x - 1, transform.position.y);
            }

            if (Input.GetButtonDown("Vertical") && Input.GetAxis("Vertical") > 0)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y + 1);
            }
            else if (Input.GetButtonDown("Vertical") && (Input.GetAxis("Vertical") < 0))
            {
                transform.position = new Vector3(transform.position.x, transform.position.y - 1);
            }

            if (Input.GetButtonDown("Submit"))
            {                
                if (hit.collider != null)
                {
                    if (currSelectedChar == null)
                    {
                        if (hit.collider.CompareTag("Player") && !hit.collider.GetComponent<PlayerMovement>().wait)
                        {
                            currSelectedChar = hit.collider.gameObject;
                            currSelectedChar.GetComponent<PlayerMovement>().Selected();
                            currSelectedChar.GetComponent<BoxCollider2D>().enabled = false;
                        }

                        if (hit.collider.CompareTag("Enemy"))
                        {
                            currSelectedChar = hit.collider.gameObject;
                            currSelectedChar.GetComponent<EnemyAI>().Selected();
                        }                   
                    }

                    if (currSelectedChar != null && currSelectedChar.GetComponent<PlayerMovement>() != null)
                    {
                        if (hit.collider.CompareTag("MoveTile"))
                        {
                            currSelectedChar.GetComponent<PlayerMovement>().oldPos = currSelectedChar.transform.position;
                            currSelectedChar.transform.position = hit.collider.transform.position;
                            currSelectedChar.transform.position = new Vector3(transform.position.x, Mathf.Round(currSelectedChar.transform.position.y) - 0.5f);

                            CheckHealRange();

                            selectPanel.SetActive(true);
                            EventSystem.current.SetSelectedGameObject(null);
                            EventSystem.current.SetSelectedGameObject(selectPanel.transform.GetChild(0).GetChild(0).gameObject);
                        }
                    }
                }
            }

            if (Input.GetButtonDown("Cancel") && currSelectedChar != null)
            {
                tileData.DeselectMovement();
                currSelectedChar.GetComponent<BoxCollider2D>().enabled = true;
                currSelectedChar = null;
            }
        }
        else if (selectPanel.activeSelf && currSelectedChar != null)
        {
            if (Input.GetButtonDown("Cancel"))
            {
                currSelectedChar.transform.position = currSelectedChar.GetComponent<PlayerMovement>().oldPos;
                currSelectedChar.GetComponent<PlayerMovement>().Selected();
                selectPanel.SetActive(false);
            }
        }
        else if (attackPanel.activeSelf && currSelectedChar != null)
        {
            if (Input.GetButtonDown("Cancel"))
            {
                attackPanel.SetActive(false);
                currSelectedChar.GetComponent<Attack>().target = null;
                currSelectedChar.GetComponent<Attack>().indexEnemies = 0;
                selectPanel.SetActive(true);
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(selectPanel.transform.GetChild(0).GetChild(0).gameObject);
            }
            else if (Input.GetButtonDown("Submit") && doneCalc)
            {
                attackPanel.SetActive(false);
                battlePanel.SetActive(true);
                BM.Battle(currSelectedChar.GetComponent<Stats>(), currSelectedChar.GetComponent<Attack>().target.GetComponent<Stats>(), currSelectedChar.GetComponent<Attack>().distance, true);
            }
        }
    }

    private void CheckHealRange()
    {
        bool hasStaff = false;

        for (int i = 0; i < currSelectedChar.GetComponent<Stats>().weapons.Count; i++)
        {
            if (currSelectedChar.GetComponent<Stats>().weapons[i].typeOfWeapon == Weapon.WeaponType.STAFF)
            {
                hasStaff = true;
            }            
        }

        if (currSelectedChar != null && hasStaff)
        {
            List<GameObject> allies = tileData.CheckEnemiesInRange(Mathf.RoundToInt(currSelectedChar.transform.position.x - 0.5f), Mathf.RoundToInt(currSelectedChar.transform.position.y - 0.5f), currSelectedChar.GetComponent<Stats>().equippedWeapon.range, currSelectedChar.GetComponent<Stats>().equippedWeapon.rangeOneAndTwo, false, true);

            currSelectedChar.GetComponent<Heal>().SetAllies(allies);
        }
        else if (!hasStaff)
        {
            GameObject.FindGameObjectWithTag("Canvas").GetComponent<SelectChoices>().healChoice.SetActive(false);
        }
    }

    public void EnemyFight(GameObject enemy)
    {
        battlePanel.SetActive(true);
        BM.Battle(enemy.GetComponent<Attack>().target.GetComponent<Stats>(), enemy.GetComponent<Stats>(), enemy.GetComponent<Attack>().distance, false);
    }

    public IEnumerator DoneCalcStats()
    {        
        yield return new WaitForSeconds(0.2f);
        doneCalc = true;
    }

    private void PathFinding(RaycastHit2D hit)
    {
        int y = Mathf.RoundToInt(-currSelectedChar.transform.position.y + 0.5f);
        int x = Mathf.RoundToInt(currSelectedChar.transform.position.x - 0.5f);

        current = tileData.rowsMovement[y].transform.GetChild(x).gameObject;

        AddToClosed(current);
        current.GetComponent<TileNumber>().f = 0;

        //while(openList.Count > 0)
        //{
            
        //}
    }

    private void CheckNeighbors(int y, int x, int wall)
    {
        GameObject topNeighbor = null;
        GameObject rightNeighbor = null;
        GameObject leftNeighbor = null;
        GameObject bottomNeighbor = null;

        if (y - 1 > -1)
        {
            topNeighbor = tileData.rowsMovement[y - 1].transform.GetChild(x).gameObject;
            //distanceTop = Mathf.Abs(x - beginX) + Mathf.Abs((y-1) - beginY);
        }
        else
        {
            y = 1;
        }
        if (y + 1 < tileData.rowsMovement.Count)
        {
            bottomNeighbor = tileData.rowsMovement[y + 1].transform.GetChild(x).gameObject;
            //distanceBottom = Mathf.Abs(x - beginX) + Mathf.Abs((y + 1) - beginY);
        }
        else
        {
            y = tileData.rowsMovement.Count - 1;
        }
        if (x + 1 < tileData.rowsMovement[y].transform.childCount)
        {
            rightNeighbor = tileData.rowsMovement[y].transform.GetChild(x + 1).gameObject;
            //distanceRight = Mathf.Abs((x + 1) - beginX) + Mathf.Abs(y - beginY);
        }
        else
        {
            x = tileData.rowsMovement[y].transform.childCount - 1;
        }
        if (x - 1 > -1)
        {
            leftNeighbor = tileData.rowsMovement[y].transform.GetChild(x - 1).gameObject;
            //distanceLeft = Mathf.Abs((x - 1) - beginX) + Mathf.Abs(y - beginY);
        }
        else
        {
            x = 1;
        }

        if (topNeighbor != null) //Top check
        {
           
        }
        if (bottomNeighbor != null) //Bottom check
        {
            
        }
        if (leftNeighbor != null) //Left check
        {
            
        }
        if (rightNeighbor != null) //Right check
        {

        }
    }

    private void AddToClosed(GameObject tile)
    {
        openList.Remove(tile);
        closedList.Add(tile);
    }
}
