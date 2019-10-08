using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public bool wait = false;
    public bool current = false;

    private TileData tileData;
    private Stats stats;

    [SerializeField] private Transform target;

    // Start is called before the first frame update
    void Start()
    {
        tileData = GameObject.FindGameObjectWithTag("TileManager").GetComponent<TileData>();
        stats = GetComponent<Stats>();
        stats.equippedWeapon = stats.weapons[0];
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public void Selected()
    {
        if (stats.typeMovement == Stats.movementType.FLIER)
        {
            tileData.RevealTiles(Mathf.RoundToInt(transform.position.x - 0.5f), Mathf.RoundToInt(transform.position.y - 0.5f), Mathf.RoundToInt(stats.mov), stats.equippedWeapon.range, true, true, 2);
        }
        else
        {
            tileData.RevealTiles(Mathf.RoundToInt(transform.position.x - 0.5f), Mathf.RoundToInt(transform.position.y - 0.5f), Mathf.RoundToInt(stats.mov), stats.equippedWeapon.range, false, true, 2);
        }
    }

    public void GetTarget()
    {
        if (!wait)
        {
            int x = Mathf.RoundToInt(transform.position.x - 0.5f);
            int y = Mathf.RoundToInt(transform.position.y - 0.5f);

            current = true;
            tileData.DeselectMovement();
            Selected();

            target = GetClosestPlayer(tileData.players);

            WalkingTowardsTarget(x, y, false, false, false, false);
        }        
    }

    private void WalkingTowardsTarget(int x, int y, bool facingUp, bool facingDown, bool facingLeft, bool facingRight)
    {
        float distanceTop = 100;
        float distanceBottom = 99;
        float distanceLeft = 98;
        float distanceRight = 97;

        bool dontGoUp = false;
        bool dontGoDown = false;
        bool dontGoLeft = false;
        bool dontGoRight = false;

        if (-y - 1 > -1)
        {
            distanceTop = Mathf.Abs(transform.position.x - target.position.x) + Mathf.Abs((transform.position.y + 1) - target.position.y);
            Debug.Log("top " + distanceTop + -y);
        }
        if(-y - 1 < 0 || !tileData.rowsMovement[-y - 1].transform.GetChild(x).CompareTag("MoveTile"))
        {
            dontGoUp = true;
        }
        if (-y + 1 < tileData.rowsMovement.Count)
        {
            distanceBottom = Mathf.Abs(transform.position.x - target.position.x) + Mathf.Abs((transform.position.y - 1) - target.position.y);
            Debug.Log("bottom " + distanceBottom);
        }
        if(-y + 1 >= tileData.rowsMovement.Count || !tileData.rowsMovement[-y + 1].transform.GetChild(x).CompareTag("MoveTile"))
        {
            dontGoDown = true;
        }
        if (x - 1 > -1)
        {
            distanceLeft = Mathf.Abs((transform.position.x - 1) - target.position.x) + Mathf.Abs(transform.position.y - target.position.y);
            Debug.Log("left " + distanceLeft);
        }
        if(x - 1 < 0 || !tileData.rowsMovement[-y].transform.GetChild(x - 1).CompareTag("MoveTile"))
        {
            dontGoLeft = true;
        }
        if (x + 1 < tileData.rowsMovement[-y].transform.childCount)
        {
            distanceRight = Mathf.Abs((transform.position.x + 1) - target.position.x) + Mathf.Abs(transform.position.y - target.position.y);
            Debug.Log("right " + distanceRight);
        }
        if(x + 1 >= tileData.rowsMovement[-y].transform.childCount || !tileData.rowsMovement[-y].transform.GetChild(x + 1).CompareTag("MoveTile"))
        {
            dontGoRight = true;
        }

        if (distanceTop >= stats.equippedWeapon.range && distanceBottom >= stats.equippedWeapon.range && distanceLeft >= stats.equippedWeapon.range && distanceRight >= stats.equippedWeapon.range && !dontGoDown && !dontGoLeft && !dontGoRight && !dontGoUp)
        {
            if (distanceTop <= distanceBottom && distanceTop <= distanceLeft && distanceTop <= distanceRight && !facingDown && !dontGoUp)
            {
                transform.position = new Vector2(transform.position.x, transform.position.y + 1);
                StartCoroutine(DelayMovement(x, y + 1, true, false, false, false));
            }
            else if (distanceBottom < distanceTop && distanceBottom <= distanceLeft && distanceBottom <= distanceRight && !facingUp && !dontGoDown)
            {
                transform.position = new Vector2(transform.position.x, transform.position.y - 1);
                StartCoroutine(DelayMovement(x, y - 1, false, true, false, false));
            }
            else if (distanceLeft < distanceBottom && distanceLeft < distanceTop && distanceLeft <= distanceRight && !facingRight && !dontGoLeft)
            {
                transform.position = new Vector2(transform.position.x - 1, transform.position.y);
                StartCoroutine(DelayMovement(x - 1, y, false, false, true, false));
            }
            else if (distanceRight < distanceBottom && distanceRight < distanceLeft && distanceRight < distanceTop && !facingLeft && !dontGoRight)
            {
                transform.position = new Vector2(transform.position.x + 1, transform.position.y);
                StartCoroutine(DelayMovement(x + 1, y, false, false, false, true));
            }
        }
        else
        {
            tileData.DeselectMovement();
            wait = true;
        }
    }

    private IEnumerator DelayMovement(int x, int y, bool facingUp, bool facingDown, bool facingLeft, bool facingRight)
    {
        yield return new WaitForSeconds(0.4f);
        WalkingTowardsTarget(x, y, facingUp, facingDown, facingLeft, facingRight);
    }

    private Transform GetClosestPlayer(List<GameObject> players)
    {
        Transform tMin = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = transform.position;
        foreach (GameObject t in players)
        {
            float dist = Vector3.Distance(t.transform.position, currentPos);
            if (dist < minDist)
            {
                tMin = t.transform;
                minDist = dist;
            }
        }
        return tMin;
    }
}
