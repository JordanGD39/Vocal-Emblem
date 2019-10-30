using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public bool wait = false;
    public bool current = false;
    private int wall = 0;
    public int charId = 2;

    private TileData tileData;
    private BattleStateMachine BSM;
    private Stats stats;

    [SerializeField] private Transform target;

    // Start is called before the first frame update
    void Start()
    {
        tileData = GameObject.FindGameObjectWithTag("TileManager").GetComponent<TileData>();
        BSM = GameManager.instance.GetComponent<BattleStateMachine>();
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
            wall = -20;
            tileData.RevealTiles(Mathf.RoundToInt(transform.position.x - 0.5f), Mathf.RoundToInt(transform.position.y - 0.5f), Mathf.RoundToInt(stats.mov), stats.equippedWeapon.range, true, true, 2);
        }
        else
        {
            wall = 0;
            tileData.RevealTiles(Mathf.RoundToInt(transform.position.x - 0.5f), Mathf.RoundToInt(transform.position.y - 0.5f), Mathf.RoundToInt(stats.mov), stats.equippedWeapon.range, false, true, 2);
        }
    }

    public void GetTarget()
    {
        if (!wait)
        {
            bool targetInRange = false;

            List<GameObject> targets = new List<GameObject>();

            int x = Mathf.RoundToInt(transform.position.x - 0.5f);
            int y = Mathf.RoundToInt(transform.position.y - 0.5f);

            current = true;
            tileData.DeselectMovement();
            Selected();

            //Target priority
            for (int i = 0; i < tileData.players.Count; i++)
            {
                if (tileData.rowsMovement[Mathf.RoundToInt(transform.position.y - 0.5f)].transform.GetChild(Mathf.RoundToInt(transform.position.x - 0.5f)).CompareTag("MoveTile") || tileData.rowsMovement[Mathf.RoundToInt(transform.position.y - 0.5f)].transform.GetChild(Mathf.RoundToInt(transform.position.x - 0.5f)).CompareTag("MoveTileRed"))
                {
                    targetInRange = true;
                    targets.Add(tileData.players[i]);
                }
            }

            if (!targetInRange)
            {
                target = GetClosestPlayer(tileData.players);
            }
            else
            {
                float bestDamage = 0;

                for (int i = 0; i < targets.Count; i++)
                {
                    int xEnemy = Mathf.RoundToInt(targets[i].transform.position.x - 0.5f);
                    int yEnemy = Mathf.RoundToInt(targets[i].transform.position.y - 0.5f);

                    Attack attack = GetComponent<Attack>();
                        
                    //Damage calc
                    float damage = attack.CalcDamage(gameObject, targets[i]);

                    //Hit calc
                    float hit = attack.CalcHit(gameObject);
                    float enemyEvade = attack.CalcEvade(targets[i], xEnemy, yEnemy);
                    float acc = attack.CalcAccuracy(hit, enemyEvade, gameObject);

                    if (damage > bestDamage)
                    {
                        if (acc > 70)
                        {
                            bestDamage = damage;
                        }                       
                    }
                }
            }
            
            if (target != null)
            {
                WalkingTowardsTarget(x, y, false, false, false, false);
            }
        }        
    }

    private void WalkingTowardsTarget(int x, int y, bool facingUp, bool facingDown, bool facingLeft, bool facingRight)
    {
        bool inRange = false;

        float distanceTop = 100;
        float distanceBottom = 99;
        float distanceLeft = 98;
        float distanceRight = 97;
        float distanceTopAbs = 100;
        float distanceBottomAbs = 99;
        float distanceLeftAbs = 98;
        float distanceRightAbs = 97;

        bool dontGoUp = false;
        bool dontGoDown = false;
        bool dontGoLeft = false;
        bool dontGoRight = false;

        if (-y - 1 > -1)
        {
            distanceTop = Mathf.Abs(transform.position.x - target.position.x) + Mathf.Abs((transform.position.y + 1) - target.position.y);
            distanceTopAbs = distanceTop;
            Debug.Log("top " + distanceTop + -y);
        }
        if(-y - 1 < 0 || !tileData.rowsMovement[-y - 1].transform.GetChild(x).CompareTag("MoveTile") || facingDown)
        {
            dontGoUp = true;
            distanceTop = 100;
            Debug.Log("U");
        }
        if (tileData.currMap[-y - 1, x] > wall && tileData.currMapCharPos[-y - 1, x] == 0 && tileData.rowsMovement[-y - 1].transform.GetChild(x).CompareTag("MoveTileRed"))
        {
            Debug.Log("BlockU");
            dontGoUp = true;
            dontGoDown = true;
            dontGoLeft = true;
            dontGoRight = true;
        }
        if (-y + 1 < tileData.rowsMovement.Count)
        {
            distanceBottom = Mathf.Abs(transform.position.x - target.position.x) + Mathf.Abs((transform.position.y - 1) - target.position.y);
            distanceBottomAbs = distanceBottom;
            Debug.Log("bottom " + distanceBottom);
        }
        if(-y + 1 >= tileData.rowsMovement.Count || !tileData.rowsMovement[-y + 1].transform.GetChild(x).CompareTag("MoveTile") || facingUp)
        {
            dontGoDown = true;
            distanceBottom = 99;
            Debug.Log("D");
        }
        if (tileData.currMap[-y + 1, x] > wall && tileData.currMapCharPos[-y + 1, x] == 0 && tileData.rowsMovement[-y + 1].transform.GetChild(x).CompareTag("MoveTileRed"))
        {
            Debug.Log("BlockD");
            dontGoUp = true;
            dontGoDown = true;
            dontGoLeft = true;
            dontGoRight = true;
        }
        if (x - 1 > -1)
        {
            distanceLeft = Mathf.Abs((transform.position.x - 1) - target.position.x) + Mathf.Abs(transform.position.y - target.position.y);
            distanceLeftAbs = distanceLeft;
            Debug.Log("left " + distanceLeft);
        }
        if(x - 1 < 0 || !tileData.rowsMovement[-y].transform.GetChild(x - 1).CompareTag("MoveTile") || facingRight)
        {
            dontGoLeft = true;
            distanceLeft = 98;
            Debug.Log("L");
        }
        if (x + 1 < tileData.rowsMovement[-y].transform.childCount)
        {
            distanceRight = Mathf.Abs((transform.position.x + 1) - target.position.x) + Mathf.Abs(transform.position.y - target.position.y);
            distanceRightAbs = distanceRight;
            Debug.Log("right " + distanceRight);
        }
        if(x + 1 >= tileData.rowsMovement[-y].transform.childCount || !tileData.rowsMovement[-y].transform.GetChild(x + 1).CompareTag("MoveTile") || facingLeft)
        {
            dontGoRight = true;
            distanceRight = 97;
            Debug.Log("R");
        }

        if (distanceTopAbs == 0 || distanceBottomAbs == 0 || distanceLeftAbs == 0 || distanceRightAbs == 0)
        {
            if (!stats.equippedWeapon.rangeOneAndTwo)
            {
                if (!dontGoUp)
                {
                    tileData.currMapCharPos[-y, x] = 0;
                    tileData.currMapCharPos[-y - 1, x] = charId;
                    transform.position = new Vector2(transform.position.x, transform.position.y + 1);
                }
                else if(!dontGoDown)
                {
                    tileData.currMapCharPos[-y, x] = 0;
                    transform.position = new Vector2(transform.position.x, transform.position.y - 1);
                    tileData.currMapCharPos[-y + 1, x] = charId;
                }
                else if(!dontGoLeft)
                {
                    tileData.currMapCharPos[-y, x] = 0;
                    transform.position = new Vector2(transform.position.x - 1, transform.position.y);
                    tileData.currMapCharPos[-y, x - 1] = charId;
                }
                else if(!dontGoRight)
                {
                    tileData.currMapCharPos[-y, x] = 0;
                    transform.position = new Vector2(transform.position.x + 1, transform.position.y);
                    tileData.currMapCharPos[-y, x + 1] = charId;
                }
            }
        }

        if (distanceTopAbs < stats.equippedWeapon.range || distanceBottomAbs < stats.equippedWeapon.range || distanceLeftAbs < stats.equippedWeapon.range || distanceRightAbs < stats.equippedWeapon.range)
        {
            inRange = true;
            tileData.DeselectMovement();
            GetComponent<Attack>().CheckAttackRange(false);            
        }

        if (distanceTop >= stats.equippedWeapon.range && distanceBottom >= stats.equippedWeapon.range && distanceLeft >= stats.equippedWeapon.range && distanceRight >= stats.equippedWeapon.range && !inRange)
        {
            if (distanceTop <= distanceBottom && distanceTop <= distanceLeft && distanceTop <= distanceRight && !facingDown && !dontGoUp)
            {
                tileData.currMapCharPos[-y, x] = 0;
                tileData.currMapCharPos[-y - 1, x] = charId;
                transform.position = new Vector2(transform.position.x, transform.position.y + 1);                
                StartCoroutine(DelayMovement(x, y + 1, true, false, false, false));
            }
            else if (distanceBottom < distanceTop && distanceBottom <= distanceLeft && distanceBottom <= distanceRight && !facingUp && !dontGoDown)
            {
                tileData.currMapCharPos[-y, x] = 0;
                transform.position = new Vector2(transform.position.x, transform.position.y - 1);
                tileData.currMapCharPos[-y + 1, x] = charId;
                StartCoroutine(DelayMovement(x, y - 1, false, true, false, false));
            }
            else if (distanceLeft < distanceBottom && distanceLeft < distanceTop && distanceLeft <= distanceRight && !facingRight && !dontGoLeft)
            {
                tileData.currMapCharPos[-y, x] = 0;
                transform.position = new Vector2(transform.position.x - 1, transform.position.y);
                tileData.currMapCharPos[-y, x - 1] = charId;
                StartCoroutine(DelayMovement(x - 1, y, false, false, true, false));
            }
            else if (distanceRight < distanceBottom && distanceRight < distanceLeft && distanceRight < distanceTop && !facingLeft && !dontGoRight)
            {
                tileData.currMapCharPos[-y, x] = 0;
                transform.position = new Vector2(transform.position.x + 1, transform.position.y);
                tileData.currMapCharPos[-y, x + 1] = charId;
                StartCoroutine(DelayMovement(x + 1, y, false, false, false, true));
            }
        }       

        if (dontGoDown && dontGoLeft && dontGoRight && dontGoUp && !tileData.rowsMovement[-Mathf.RoundToInt(target.position.y + 0.5f)].transform.GetChild(Mathf.RoundToInt(target.position.x - 0.5f)).CompareTag("MoveTileRed") && !inRange)
        {            
            tileData.DeselectMovement();
            BSM.enemyIndex++;
            BSM.GiveTurnToAI();
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

        if (players.Count > 0)
        {
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
        }
        return tMin;
    }
}
