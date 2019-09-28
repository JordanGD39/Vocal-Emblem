using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileData : MonoBehaviour
{
    public GameObject tilePrefab;
    public GameObject mapEmpty;
    public List<GameObject> rowsMovement = new List<GameObject>();
    public List<GameObject> enemies = new List<GameObject>();
    public List<GameObject> players = new List<GameObject>();
    public List<GameObject> enemiesInGame = new List<GameObject>();

    public bool doneLoading = false;

    public int[,] currMap;
    public int[,] currMapCharPos;

    public int mapNumber = 0;

    public Sprite[] mapSprites;
    public Sprite[] terrainSprites;
    public Sprite[] obstacleSprites;
    public Sprite[] wallSprites;
    public List<GameObject> openList = new List<GameObject>();
    public List<GameObject> closedList = new List<GameObject>();
    public GameObject current;
    public GameObject currentTop;
    public GameObject currentBottom;
    public GameObject currentLeft;
    public GameObject currentRight;
    private bool checkingDown = true;

    private MapData mapData;
    // Start is called before the first frame update
    void Start()
    {
        mapData = GetComponent<MapData>();
        switch (mapNumber)
        {
            case 1:
                currMap = mapData.mapTest;
                currMapCharPos = mapData.mapTestCharacterPos;
                break;
        }

        for (int y = 0; y < currMap.GetLength(0); y++) //The Y axis loop
        {
            for (int x = 0; x < currMap.GetLength(1); x++) //The X axis loop
            {
                switch (currMap[y, x])
                {
                    case -20:
                        GameObject tile2 = Instantiate(tilePrefab, mapEmpty.transform);
                        tile2.transform.position = new Vector3(x + 0.5f, -y + 0.5f, 0);
                        tile2.GetComponent<SpriteRenderer>().sprite = wallSprites[0];
                        break;
                    case 0:
                        GameObject tile = Instantiate(tilePrefab, mapEmpty.transform);
                        tile.transform.position = new Vector3(x + 0.5f, -y + 0.5f, 0);
                        tile.GetComponent<SpriteRenderer>().sprite = obstacleSprites[0];
                        break;
                    case 1:
                        GameObject tile1 = Instantiate(tilePrefab, mapEmpty.transform);
                        tile1.transform.position = new Vector3(x + 0.5f, -y + 0.5f, 0);
                        tile1.GetComponent<SpriteRenderer>().sprite = mapSprites[0];
                        break;
                    case 20:
                        GameObject tile3 = Instantiate(tilePrefab, mapEmpty.transform);
                        tile3.transform.position = new Vector3(x + 0.5f, -y + 0.5f, 0);
                        tile3.GetComponent<SpriteRenderer>().sprite = terrainSprites[0];
                        break;
                }
            }
        }

        for (int x = 0; x < currMapCharPos.GetLength(1); x++)
        {
            for (int y = 0; y < currMapCharPos.GetLength(0); y++)
            {
                switch (currMapCharPos[y, x])
                {
                    case 1:
                        GameObject player = Instantiate(GameManager.instance.playerTeam[GameManager.instance.playerTeamCount], new Vector3(x + 0.5f, -y + 0.5f, 0), GameManager.instance.playerTeam[GameManager.instance.playerTeamCount].transform.rotation);
                        break;
                    case 2:
                        if (enemies.Count > 0)
                        {
                            GameObject enemy = Instantiate(enemies[0], new Vector3(x + 0.5f, -y + 0.5f, 0), enemies[0].transform.rotation);
                        }
                        break;
                    case 3:
                        if (enemies.Count > 0)
                        {
                            GameObject enemy = Instantiate(enemies[1], new Vector3(x + 0.5f, -y + 0.5f, 0), enemies[1].transform.rotation);
                        }
                        break;
                    case 4:
                        if (enemies.Count > 0)
                        {
                            GameObject enemy = Instantiate(enemies[2], new Vector3(x + 0.5f, -y + 0.5f, 0), enemies[2].transform.rotation);
                        }
                        break;
                    case 5:
                        if (enemies.Count > 0)
                        {
                            GameObject enemy = Instantiate(enemies[3], new Vector3(x + 0.5f, -y + 0.5f, 0), enemies[3].transform.rotation);
                        }
                        break;
                    case 6:
                        if (enemies.Count > 0)
                        {
                            GameObject enemy = Instantiate(enemies[4], new Vector3(x + 0.5f, -y + 0.5f, 0), enemies[4].transform.rotation);
                        }
                        break;
                }
                if (GameManager.instance.playerTeamCount < GameManager.instance.playerTeam.Count - 1)
                {
                    GameManager.instance.playerTeamCount++;
                }
            }
        }

        for (int i = 0; i < GameObject.FindGameObjectWithTag("Rows").transform.childCount; i++)
        {
            rowsMovement.Add(GameObject.FindGameObjectWithTag("Rows").transform.GetChild(i).gameObject);
        }
        players.AddRange(GameObject.FindGameObjectsWithTag("Player"));
        enemiesInGame.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));

        doneLoading = true;
    }

    public void RevealTiles(int x, int y, int mov, int range, bool flier)
    {
        int wall = 0;
        if (flier)
        {
            wall = -20;
        }
        else
        {
            wall = 0;
        }
        for (int i = 0; i < rowsMovement.Count; i++)
        {
            for (int j = 0; j < rowsMovement[i].transform.childCount; j++)
            {
                openList.Add(rowsMovement[i].transform.GetChild(j).gameObject);
                rowsMovement[i].transform.GetChild(j).GetComponent<SpriteRenderer>().color = new Color32(0, 129, 255, 168);
                rowsMovement[i].transform.GetChild(j).GetComponent<TileNumber>().number = 0;
                rowsMovement[i].transform.GetChild(j).tag = "Untagged";
            }
        }

        current = rowsMovement[-y].transform.GetChild(x).gameObject;
        AddToClosed(current);
        current.SetActive(true);
        current.tag = "MoveTile";
        current.GetComponent<SpriteRenderer>().color = new Color32(0, 129, 255, 168);
        Debug.Log(x);
        Debug.Log(-y);
        int beginX = x;
        int beginY = -y;
        if (currMap[-y, x] >= 20 && !flier)
        {
            float f = mov;
            f *= 0.75f;
            mov = Mathf.RoundToInt(f);
            mov -= 1;
        }
        CheckNeighbors(-y, x, wall, mov, beginX, beginY, range, 0);
    }

    private void CheckNeighbors(int y, int x, int wall, int mov, int beginX, int beginY, int range, int count)
    {
        GameObject topNeighbor = null;
        GameObject rightNeighbor = null;
        GameObject leftNeighbor = null;
        GameObject bottomNeighbor = null;

        int giveCount = count + 1;
        //Debug.Log(giveCount);

        if (y - 1 > -1)
        {
            topNeighbor = rowsMovement[y - 1].transform.GetChild(x).gameObject;
            //distanceTop = Mathf.Abs(x - beginX) + Mathf.Abs((y-1) - beginY);
        }
        else
        {
            y = 1;
        }
        if (y + 1 < rowsMovement.Count)
        {
            bottomNeighbor = rowsMovement[y + 1].transform.GetChild(x).gameObject;
            //distanceBottom = Mathf.Abs(x - beginX) + Mathf.Abs((y + 1) - beginY);
        }
        else
        {
            y = rowsMovement.Count - 1;
        }
        if (x + 1 < rowsMovement[y].transform.childCount)
        {
            rightNeighbor = rowsMovement[y].transform.GetChild(x + 1).gameObject;
            //distanceRight = Mathf.Abs((x + 1) - beginX) + Mathf.Abs(y - beginY);
        }
        else
        {
            x = rowsMovement[y].transform.childCount - 1;
        }
        if (x - 1 > -1)
        {
            leftNeighbor = rowsMovement[y].transform.GetChild(x - 1).gameObject;
            //distanceLeft = Mathf.Abs((x - 1) - beginX) + Mathf.Abs(y - beginY);
        }
        else
        {
            x = 1;
        }

        if (topNeighbor != null) //Top check
        {
            if (openList.Contains(topNeighbor) || giveCount < topNeighbor.GetComponent<TileNumber>().number)
            {
                if (giveCount <= mov && currMap[y - 1, x] > wall && currMapCharPos[y - 1, x] == 0)
                {
                    //Debug.Log("TOP");
                    topNeighbor.GetComponent<TileNumber>().number = giveCount;
                    topNeighbor.SetActive(true);
                    currentTop = topNeighbor;
                    AddToClosed(topNeighbor);
                    topNeighbor.GetComponent<SpriteRenderer>().color = new Color32(0, 129, 255, 168);
                    topNeighbor.tag = "MoveTile";
                    CheckNeighbors(y - 1, x, wall, mov, beginX, beginY, range, giveCount);
                }
                if (giveCount > mov && giveCount <= mov + range || currMap[y - 1, x] <= wall || currMapCharPos[y - 1, x] != 0)
                {
                    if (!topNeighbor.CompareTag("MoveTile"))
                    {
                        topNeighbor.GetComponent<TileNumber>().number = giveCount;
                        topNeighbor.SetActive(true);
                        currentTop = topNeighbor;
                        AddToClosed(topNeighbor);
                        topNeighbor.tag = "MoveTileRed";
                        topNeighbor.GetComponent<SpriteRenderer>().color = new Color32(255, 0, 0, 168);
                        if (range > 1)
                        {
                            AttackCheck(y - 1, x, range, mov);
                        }
                    }
                }
            }
        }
        if (bottomNeighbor != null) //Bottom check
        {
            if (openList.Contains(bottomNeighbor) || giveCount < bottomNeighbor.GetComponent<TileNumber>().number)
            {
                if (giveCount <= mov && currMap[y + 1, x] > wall && currMapCharPos[y + 1, x] == 0)
                {
                    // Debug.Log("Bottom");
                    bottomNeighbor.GetComponent<TileNumber>().number = giveCount;
                    bottomNeighbor.SetActive(true);
                    currentBottom = bottomNeighbor;
                    AddToClosed(bottomNeighbor);
                    bottomNeighbor.GetComponent<SpriteRenderer>().color = new Color32(0, 129, 255, 168);
                    bottomNeighbor.tag = "MoveTile";
                    CheckNeighbors(y + 1, x, wall, mov, beginX, beginY, range, giveCount);
                }
                if (giveCount > mov && giveCount <= mov + range || currMap[y + 1, x] <= wall || currMapCharPos[y + 1, x] != 0)
                {
                    if (!bottomNeighbor.CompareTag("MoveTile"))
                    {
                        bottomNeighbor.GetComponent<TileNumber>().number = giveCount;
                        bottomNeighbor.SetActive(true);
                        currentBottom = bottomNeighbor;
                        AddToClosed(bottomNeighbor);
                        bottomNeighbor.tag = "MoveTileRed";
                        bottomNeighbor.GetComponent<SpriteRenderer>().color = new Color32(255, 0, 0, 168);
                        if (range > 1)
                        {
                            AttackCheck(y + 1, x, range, mov);
                        }
                    }
                }
            }
        }
        if (leftNeighbor != null) //Left check
        {
            if (openList.Contains(leftNeighbor) || giveCount < leftNeighbor.GetComponent<TileNumber>().number)
            {
                if (giveCount <= mov && currMap[y, x - 1] > wall && currMapCharPos[y, x - 1] == 0)
                {
                    //Debug.Log("Left");
                    leftNeighbor.GetComponent<TileNumber>().number = giveCount;
                    leftNeighbor.SetActive(true);
                    currentLeft = leftNeighbor;
                    AddToClosed(leftNeighbor);
                    leftNeighbor.GetComponent<SpriteRenderer>().color = new Color32(0, 129, 255, 168);
                    leftNeighbor.tag = "MoveTile";
                    CheckNeighbors(y, x - 1, wall, mov, beginX, beginY, range, giveCount);
                }
                else if (giveCount > mov && giveCount <= mov + range || currMap[y, x - 1] <= wall || currMapCharPos[y, x - 1] != 0)
                {
                    if (!leftNeighbor.CompareTag("MoveTile"))
                    {
                        leftNeighbor.GetComponent<TileNumber>().number = giveCount;
                        leftNeighbor.SetActive(true);
                        currentLeft = leftNeighbor;
                        AddToClosed(leftNeighbor);
                        leftNeighbor.tag = "MoveTileRed";
                        leftNeighbor.GetComponent<SpriteRenderer>().color = new Color32(255, 0, 0, 168);
                        if (range > 1)
                        {
                            AttackCheck(y, x - 1, range, mov);
                        }
                    }
                }
            }
        }
        if (rightNeighbor != null) //Right check
        {
            if (openList.Contains(rightNeighbor) || giveCount < rightNeighbor.GetComponent<TileNumber>().number)
            {
                if (giveCount <= mov && currMap[y, x + 1] > wall && currMapCharPos[y, x + 1] == 0)
                {
                    //Debug.Log("right");
                    rightNeighbor.GetComponent<TileNumber>().number = giveCount;
                    rightNeighbor.SetActive(true);
                    currentRight = rightNeighbor;
                    AddToClosed(rightNeighbor);
                    rightNeighbor.GetComponent<SpriteRenderer>().color = new Color32(0, 129, 255, 168);
                    rightNeighbor.tag = "MoveTile";
                    CheckNeighbors(y, x + 1, wall, mov, beginX, beginY, range, giveCount);
                }
                if (giveCount > mov && giveCount <= mov + range || currMap[y, x + 1] <= wall || currMapCharPos[y, x + 1] != 0)
                {
                    if (!rightNeighbor.CompareTag("MoveTile"))
                    {
                        rightNeighbor.GetComponent<TileNumber>().number = giveCount;
                        rightNeighbor.SetActive(true);
                        currentRight = rightNeighbor;
                        AddToClosed(rightNeighbor);
                        rightNeighbor.tag = "MoveTileRed";
                        rightNeighbor.GetComponent<SpriteRenderer>().color = new Color32(255, 0, 0, 168);
                        if (range > 1)
                        {
                            AttackCheck(y, x + 1, range, mov);
                        }
                    }
                }

            }
        }
    }

    private void AttackCheck(int y, int x, int range, int mov)
    {
        for (int i = 0; i < range; i++)
        {
            if (y + i < rowsMovement.Count && !rowsMovement[y + i].transform.GetChild(x).CompareTag("MoveTile"))
            {
                rowsMovement[y + i].transform.GetChild(x).gameObject.SetActive(true);
                rowsMovement[y + i].transform.GetChild(x).GetComponent<SpriteRenderer>().color = new Color32(255, 0, 0, 168);
                for (int j = 0; j < range - i; j++)
                {
                    if (x + j < rowsMovement[y+i].transform.childCount && !rowsMovement[y + i].transform.GetChild(x + j).CompareTag("MoveTile"))
                    {
                        rowsMovement[y + i].transform.GetChild(x + j).gameObject.SetActive(true);
                        rowsMovement[y + i].transform.GetChild(x + j).GetComponent<SpriteRenderer>().color = new Color32(255, 0, 0, 168);
                    }                    
                }
            }

            if (y - i > -1 && !rowsMovement[y - i].transform.GetChild(x).CompareTag("MoveTile"))
            {
                rowsMovement[y - i].transform.GetChild(x).gameObject.SetActive(true);
                rowsMovement[y - i].transform.GetChild(x).GetComponent<SpriteRenderer>().color = new Color32(255, 0, 0, 168);
                for (int j = 0; j < range - i; j++)
                {
                    if (x - j > -1 && !rowsMovement[y - i].transform.GetChild(x - j).CompareTag("MoveTile"))
                    {
                        rowsMovement[y - i].transform.GetChild(x - j).gameObject.SetActive(true);
                        rowsMovement[y - i].transform.GetChild(x - j).GetComponent<SpriteRenderer>().color = new Color32(255, 0, 0, 168);
                    }                   
                }
            }     
        }
    }

    private void AddToClosed(GameObject tile)
    {
        openList.Remove(tile);
        closedList.Add(tile);
    }

    public List<GameObject> CheckEnemiesInRange(int x, int y, int range, bool oneTwo)
    {
        List<GameObject> enemiesFound = new List<GameObject>();        

        for (int i = 0; i < range + 1; i++)
        { 
            for (int j = 0; j < range + 1 - i; j++)
            {
                if (!oneTwo && i == 0 && j < 2)
                {
                    j = 2;
                }
                if (!oneTwo && i == 1 && j < 1)
                {
                    j = 1;
                }

                if (-y + i < rowsMovement.Count && x + j < rowsMovement[-y + i].transform.childCount)
                {
                    for (int k = 0; k < enemiesInGame.Count; k++)
                    {
                        rowsMovement[-y + i].transform.GetChild(x + j).GetComponent<SpriteRenderer>().color = new Color32(255, 0, 0, 168);
                        rowsMovement[-y + i].transform.GetChild(x + j).gameObject.SetActive(true);

                        //Debug.Log("down right " + (x + j) + " X: " + (enemiesInGame[k].transform.position.x - 0.5f) + " " + (-y + i) + " Y: " + (-enemiesInGame[k].transform.position.y + 0.5f));

                        if (x + j == enemiesInGame[k].transform.position.x - 0.5f && -y + i == -enemiesInGame[k].transform.position.y + 0.5f)
                        {
                            if (!enemiesFound.Contains(enemiesInGame[k]))
                            {
                                enemiesFound.Add(enemiesInGame[k]);
                            }
                        }
                    }
                }
                if (-y - i > -1 && x + j < rowsMovement[-y - i].transform.childCount && i != 0) //Up
                {
                    for (int k = 0; k < enemiesInGame.Count; k++)
                    {
                        rowsMovement[-y - i].transform.GetChild(x + j).GetComponent<SpriteRenderer>().color = new Color32(255, 0, 0, 168);
                        rowsMovement[-y - i].transform.GetChild(x + j).gameObject.SetActive(true);

                        //Debug.Log("up right " + (x + j) + " X: " + (enemiesInGame[k].transform.position.x - 0.5f) + " " + (-y - i) + " Y: " + (-enemiesInGame[k].transform.position.y + 0.5f));

                        if (x + j == enemiesInGame[k].transform.position.x - 0.5f && -y - i == -enemiesInGame[k].transform.position.y + 0.5f)
                        {
                            if (!enemiesFound.Contains(enemiesInGame[k]))
                            {
                                enemiesFound.Add(enemiesInGame[k]);
                            }
                        }
                    }
                }
            }            
        } //Right

        for (int i = 0; i < range + 1; i++)
        {
            for (int j = 0; j < range + 1 - i; j++)
            {
                if (!oneTwo && i == 0 && j < 2)
                {
                    j = 2;
                }
                if (!oneTwo && i == 1 && j < 1)
                {
                    j = 1;
                }

                if (-y - i > -1 && x - j > -1)
                {
                    for (int k = 0; k < enemiesInGame.Count; k++)
                    {
                        rowsMovement[-y - i].transform.GetChild(x - j).GetComponent<SpriteRenderer>().color = new Color32(255, 0, 0, 168);
                        rowsMovement[-y - i].transform.GetChild(x - j).gameObject.SetActive(true);

                        //Debug.Log("up left "+(x - j) + " X: " + (enemiesInGame[k].transform.position.x - 0.5f) + " " + (-y - i) + " Y: " + (-enemiesInGame[k].transform.position.y + 0.5f));

                        if (x - j == enemiesInGame[k].transform.position.x - 0.5f && -y - i == -enemiesInGame[k].transform.position.y + 0.5f)
                        {
                            if (!enemiesFound.Contains(enemiesInGame[k]))
                            {
                                enemiesFound.Add(enemiesInGame[k]);
                            }
                        }
                    }
                }
                if (-y + i < rowsMovement.Count && x - j > -1 && i != 0) //Down
                {
                    for (int k = 0; k < enemiesInGame.Count; k++)
                    {
                        rowsMovement[-y + i].transform.GetChild(x - j).GetComponent<SpriteRenderer>().color = new Color32(255, 0, 0, 168);
                        rowsMovement[-y + i].transform.GetChild(x - j).gameObject.SetActive(true);

                        //Debug.Log("down Left " + (x - j) + " X: " + (enemiesInGame[k].transform.position.x - 0.5f) + " " + (-y + i) + " Y: " + (-enemiesInGame[k].transform.position.y + 0.5f));

                        if (x - j == enemiesInGame[k].transform.position.x - 0.5f && -y + i == -enemiesInGame[k].transform.position.y + 0.5f)
                        {
                            if (!enemiesFound.Contains(enemiesInGame[k]))
                            {
                                enemiesFound.Add(enemiesInGame[k]);
                            }
                        }
                    }
                }
            }
            
            
        } //Left
        return enemiesFound;
    }
    

    public void DeselectMovement()
    {
        for (int i = 0; i < rowsMovement.Count; i++)
        {
            for (int j = 0; j < rowsMovement[i].transform.childCount; j++)
            {
                rowsMovement[i].transform.GetChild(j).gameObject.SetActive(false);
            }
        }
    }
}
