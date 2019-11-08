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

    public Transform target;

    // Start is called before the first frame update
    void Start()
    {
        tileData = GameObject.FindGameObjectWithTag("TileManager").GetComponent<TileData>();
        BSM = GameManager.instance.GetComponent<BattleStateMachine>();
        stats = GetComponent<Stats>();
        stats.equippedWeapon = stats.weapons[0];
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
            Debug.Log("-------------- " + name + " searching target --------------");

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
                if (tileData.rowsMovement[Mathf.RoundToInt(-tileData.players[i].transform.position.y + 0.5f)].transform.GetChild(Mathf.RoundToInt(tileData.players[i].transform.position.x - 0.5f)).CompareTag("MoveTile") || tileData.rowsMovement[Mathf.RoundToInt(-tileData.players[i].transform.position.y + 0.5f)].transform.GetChild(Mathf.RoundToInt(tileData.players[i].transform.position.x - 0.5f)).CompareTag("MoveTileRed"))
                {
                    targetInRange = true;
                    targets.Add(tileData.players[i]);
                    Debug.Log(tileData.players[i] + " added to the list");
                }
            }

            if (!targetInRange)
            {
                Debug.Log("-------------- not listening --------------");
                target = GetClosestPlayer(tileData.players);
            }
            else
            {
                float bestAcc = 0;

                //Check if someone killable with highest hitrate and doesn't die themself
                for (int i = 0; i < targets.Count; i++)
                {
                    int killOnHit = 0;

                    int xEnemy = Mathf.RoundToInt(targets[i].transform.position.x - 0.5f);
                    int yEnemy = Mathf.RoundToInt(targets[i].transform.position.y - 0.5f);

                    Attack attack = GetComponent<Attack>();
                    Stats targetStats = targets[i].GetComponent<Stats>();
                    Stats stats = GetComponent<Stats>();

                    //Damage calc
                    float damage = attack.CalcDamage(gameObject, targets[i]);
                    int doubling = attack.CalcSpeed(gameObject, targets[i]);

                    //Hit calc
                    float hit = attack.CalcHit(gameObject);
                    float enemyEvade = attack.CalcEvade(targets[i], xEnemy, yEnemy);
                    float acc = attack.CalcAccuracy(hit, enemyEvade, gameObject);

                    //Player attacks back check
                    float distance = Mathf.Abs(transform.position.x - targets[i].transform.position.x) + Mathf.Abs(transform.position.y - targets[i].transform.position.y);

                    float enemyDamage = 0;
                    int enemyDoubling = 0;
                    float enemyHit = 0;
                    float evade = 0;
                    float enemyAcc = 0;

                    if (distance <= targets[i].GetComponent<Stats>().equippedWeapon.range && targets[i].GetComponent<Stats>().equippedWeapon.rangeOneAndTwo || distance != 1 && distance <= targets[i].GetComponent<Stats>().equippedWeapon.range && !targets[i].GetComponent<Stats>().equippedWeapon.rangeOneAndTwo || targets[i].GetComponent<Stats>().equippedWeapon.counterAll)
                    {
                        if (targetStats.equippedWeapon.typeOfWeapon != Weapon.WeaponType.STAFF)
                        {
                            enemyDamage = attack.CalcDamage(targets[i], gameObject);
                            enemyDoubling = attack.CalcSpeed(targets[i], gameObject);
                            enemyHit = attack.CalcHit(targets[i]);
                            evade = attack.CalcEvade(gameObject, x, y);
                            enemyAcc = attack.CalcAccuracy(enemyHit, evade, targets[i]);
                        }
                    }

                    if (targetStats.hp - damage <= 0)
                    {
                        Debug.Log(acc + " double " + doubling + " on " + targets[i].name);
                        Debug.Log(bestAcc + " the best");
                        killOnHit = 1;
                        if (acc > bestAcc)
                        {
                            bestAcc = acc;                            
                            target = targets[i].transform;
                        }
                    }

                    if (doubling == 2 && killOnHit == 0)
                    {
                        if (targetStats.hp - damage * 2 <= 0)
                        {
                            Debug.Log(acc + " double " + doubling + " on " + targets[i].name);
                            Debug.Log(bestAcc + " the best");
                            killOnHit = 2;
                            if (acc > bestAcc && stats.hp - enemyDamage > 0 || enemyAcc < 40)
                            {
                                bestAcc = acc;
                                target = targets[i].transform;
                            }
                        }
                    }
                    else if (doubling == 4 && killOnHit == 0)
                    {
                        if (targetStats.hp - damage * 4 <= 0)
                        {
                            Debug.Log(acc + " double " + doubling + " on " + targets[i].name);
                            Debug.Log(bestAcc + " the best");

                            killOnHit = 4;
                            if (acc > bestAcc && enemyDoubling != 4 || stats.hp - enemyDamage * 3 > 0 && stats.hp - enemyDamage * 2 > 0 || enemyAcc < 40)
                            {
                                bestAcc = acc;
                                target = targets[i].transform;
                            }
                        }
                    }
                }

                //Check if someone almost dies from this attack and above and inclusive 65%
                if (target == null)
                {
                    Debug.Log("-------------- Can't kill --------------");

                    float leastHealth = 9999;

                    for (int i = 0; i < targets.Count; i++)
                    {
                        int xEnemy = Mathf.RoundToInt(targets[i].transform.position.x - 0.5f);
                        int yEnemy = Mathf.RoundToInt(targets[i].transform.position.y - 0.5f);

                        Attack attack = GetComponent<Attack>();
                        Stats targetStats = targets[i].GetComponent<Stats>();
                        Stats stats = GetComponent<Stats>();

                        //Damage calc
                        float damage = attack.CalcDamage(gameObject, targets[i]);
                        int doubling = attack.CalcSpeed(gameObject, targets[i]);

                        //Hit calc
                        float hit = attack.CalcHit(gameObject);
                        float enemyEvade = attack.CalcEvade(targets[i], xEnemy, yEnemy);
                        float acc = attack.CalcAccuracy(hit, enemyEvade, gameObject);

                        //Player attacks back check
                        float distance = Mathf.Abs(transform.position.x - targets[i].transform.position.x) + Mathf.Abs(transform.position.y - targets[i].transform.position.y);

                        float enemyDamage = 0;
                        int enemyDoubling = 0;
                        float enemyHit = 0;
                        float evade = 0;
                        float enemyAcc = 0;

                        if (distance <= targets[i].GetComponent<Stats>().equippedWeapon.range && targets[i].GetComponent<Stats>().equippedWeapon.rangeOneAndTwo || distance != 1 && distance <= targets[i].GetComponent<Stats>().equippedWeapon.range && !targets[i].GetComponent<Stats>().equippedWeapon.rangeOneAndTwo || targets[i].GetComponent<Stats>().equippedWeapon.counterAll)
                        {
                            if (targetStats.equippedWeapon.typeOfWeapon != Weapon.WeaponType.STAFF)
                            {
                                enemyDamage = attack.CalcDamage(targets[i], gameObject);
                                enemyDoubling = attack.CalcSpeed(targets[i], gameObject);
                                enemyHit = attack.CalcHit(targets[i]);
                                evade = attack.CalcEvade(gameObject, x, y);
                                enemyAcc = attack.CalcAccuracy(enemyHit, evade, targets[i]);
                            }
                        }

                        if (targetStats.hp - damage * doubling < leastHealth && acc >= 65)
                        {
                            Debug.Log((targetStats.hp - damage * doubling) + " double " + doubling + " on " + targets[i].name);
                            Debug.Log(leastHealth + " the best");
                            if (stats.hp - enemyDamage * enemyDoubling > 0 || enemyAcc < 40)
                            {
                                leastHealth = targetStats.hp;
                                target = targets[i].transform;
                            }
                        }
                    }
                }

                //Get highest hitrate
                if (target == null)
                {
                    Debug.Log("-------------- Low hitrate --------------");

                    float bestAccu = 0;

                    for (int i = 0; i < targets.Count; i++)
                    {
                        int xEnemy = Mathf.RoundToInt(targets[i].transform.position.x - 0.5f);
                        int yEnemy = Mathf.RoundToInt(targets[i].transform.position.y - 0.5f);

                        Attack attack = GetComponent<Attack>();
                        Stats targetStats = targets[i].GetComponent<Stats>();
                        Stats stats = GetComponent<Stats>();

                        //Hit calc
                        float hit = attack.CalcHit(gameObject);
                        float enemyEvade = attack.CalcEvade(targets[i], xEnemy, yEnemy);
                        float acc = attack.CalcAccuracy(hit, enemyEvade, gameObject);

                        //Player attacks back check
                        float distance = Mathf.Abs(transform.position.x - targets[i].transform.position.x) + Mathf.Abs(transform.position.y - targets[i].transform.position.y);

                        float enemyHit = 0;
                        float evade = 0;
                        float enemyAcc = 0;

                        if (distance <= targets[i].GetComponent<Stats>().equippedWeapon.range && targets[i].GetComponent<Stats>().equippedWeapon.rangeOneAndTwo || distance != 1 && distance <= targets[i].GetComponent<Stats>().equippedWeapon.range && !targets[i].GetComponent<Stats>().equippedWeapon.rangeOneAndTwo || targets[i].GetComponent<Stats>().equippedWeapon.counterAll)
                        {
                            if (targetStats.equippedWeapon.typeOfWeapon != Weapon.WeaponType.STAFF)
                            {
                                enemyHit = attack.CalcHit(targets[i]);
                                evade = attack.CalcEvade(gameObject, x, y);
                                enemyAcc = attack.CalcAccuracy(enemyHit, evade, targets[i]);
                            }
                        }

                        Debug.Log(acc + " on " + targets[i].name);
                        Debug.Log(bestAccu + " the best");

                        if (acc > bestAccu)
                        {                            
                            bestAccu = acc;
                            target = targets[i].transform;
                        }
                    }
                }
            }

            if (target != null)
            {
                Debug.Log("TARGET = " + target.name);
                Debug.Log("-------------- Walking --------------");                

                WalkingTowardsTarget(x, y, false, false, false, false);
            }
            else
            {
                tileData.DeselectMovement();
            }
        }        
    }

    private void WalkingTowardsTarget(int x, int y, bool facingUp, bool facingDown, bool facingLeft, bool facingRight)
    {
        bool inRange = false;

        bool moved = false;

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
                    y += 1;
                }
                else if(!dontGoDown)
                {
                    Debug.Log("Do this!!!");
                    tileData.currMapCharPos[-y, x] = 0;
                    tileData.currMapCharPos[-y + 1, x] = charId;
                    transform.position = new Vector2(transform.position.x, transform.position.y - 1);
                    y -= 1;
                }
                else if(!dontGoLeft)
                {
                    tileData.currMapCharPos[-y, x] = 0;
                    tileData.currMapCharPos[-y, x - 1] = charId;
                    transform.position = new Vector2(transform.position.x - 1, transform.position.y);
                    x -= 1;
                }
                else if(!dontGoRight)
                {
                    tileData.currMapCharPos[-y, x] = 0;
                    tileData.currMapCharPos[-y, x + 1] = charId;
                    transform.position = new Vector2(transform.position.x + 1, transform.position.y);
                    x += 1;
                }
            }
        }

        if (distanceTopAbs < stats.equippedWeapon.range || distanceBottomAbs < stats.equippedWeapon.range || distanceLeftAbs < stats.equippedWeapon.range || distanceRightAbs < stats.equippedWeapon.range)
        {
            if (tileData.currMapCharPos[-y, x] == charId)
            {
                inRange = true;
                moved = true;
                tileData.DeselectMovement();
                GetComponent<Attack>().CheckAttackRange(false);
            }
            else
            {
                if (!dontGoUp)
                {
                    tileData.currMapCharPos[-y, x] = 0;
                    transform.position = new Vector2(transform.position.x, transform.position.y + 1);
                    StartCoroutine(DelayMovement(x, y + 1, true, false, false, false));
                }
                else if (!dontGoDown)
                {
                    tileData.currMapCharPos[-y, x] = 0;
                    transform.position = new Vector2(transform.position.x, transform.position.y - 1);
                    StartCoroutine(DelayMovement(x, y - 1, false, true, false, false));
                }
                else if (!dontGoLeft)
                {
                    tileData.currMapCharPos[-y, x] = 0;
                    transform.position = new Vector2(transform.position.x - 1, transform.position.y);
                    StartCoroutine(DelayMovement(x - 1, y, false, false, true, false));
                }
                else if (!dontGoRight)
                {
                    tileData.currMapCharPos[-y, x] = 0;
                    transform.position = new Vector2(transform.position.x + 1, transform.position.y);
                    StartCoroutine(DelayMovement(x + 1, y, false, false, false, true));
                }
            }
        }

        if (distanceTop >= stats.equippedWeapon.range && distanceBottom >= stats.equippedWeapon.range && distanceLeft >= stats.equippedWeapon.range && distanceRight >= stats.equippedWeapon.range && !inRange)
        {
            if (distanceTop <= distanceBottom && distanceTop <= distanceLeft && distanceTop <= distanceRight && !facingDown && !dontGoUp)
            {
                dontGoUp = true;
                dontGoDown = true;
                dontGoLeft = true;
                dontGoRight = true;

                if (tileData.currMapCharPos[-y - 1, x] == 0 || tileData.rowsMovement[-y - 2].transform.GetChild(x).CompareTag("MoveTile") && tileData.currMapCharPos[-y - 2, x] == 0 && tileData.currMapCharPos[-y - 1, x] != 0)
                {
                    dontGoUp = false;
                    dontGoDown = false;
                    dontGoLeft = false;
                    dontGoRight = false;
                    moved = true;

                    tileData.currMapCharPos[-y, x] = 0;
                    if (tileData.currMapCharPos[-y - 1, x] == 0)
                    {
                        tileData.currMapCharPos[-y - 1, x] = charId;
                    }
                    transform.position = new Vector2(transform.position.x, transform.position.y + 1);
                    StartCoroutine(DelayMovement(x, y + 1, true, false, false, false));
                }
            }
            else if (distanceBottom < distanceTop && distanceBottom <= distanceLeft && distanceBottom <= distanceRight && !facingUp && !dontGoDown)
            {
                dontGoUp = true;
                dontGoDown = true;
                dontGoLeft = true;
                dontGoRight = true;

                if (tileData.currMapCharPos[-y + 1, x] == 0 || tileData.rowsMovement[-y + 2].transform.GetChild(x).CompareTag("MoveTile") && -y + 2 < tileData.rowsMovement.Count && tileData.currMapCharPos[-y + 2, x] == 0 && tileData.currMapCharPos[-y + 1, x] != 0)
                {
                    dontGoUp = false;
                    dontGoDown = false;
                    dontGoLeft = false;
                    dontGoRight = false;
                    moved = true;

                    tileData.currMapCharPos[-y, x] = 0;
                    transform.position = new Vector2(transform.position.x, transform.position.y - 1);
                    if (tileData.currMapCharPos[-y + 1, x] == 0)
                    {
                        tileData.currMapCharPos[-y + 1, x] = charId;
                    }
                    StartCoroutine(DelayMovement(x, y - 1, false, true, false, false));
                }
            }
            else if (distanceLeft < distanceBottom && distanceLeft < distanceTop && distanceLeft <= distanceRight && !facingRight && !dontGoLeft)
            {
                dontGoUp = true;
                dontGoDown = true;
                dontGoLeft = true;
                dontGoRight = true;

                if (tileData.currMapCharPos[-y, x - 1] == 0 || tileData.rowsMovement[-y].transform.GetChild(x - 2).CompareTag("MoveTile") && tileData.currMapCharPos[-y, x - 2] == 0 && tileData.currMapCharPos[-y, x - 1] != 0)
                {
                    dontGoUp = false;
                    dontGoDown = false;
                    dontGoLeft = false;
                    dontGoRight = false;
                    moved = true;

                    tileData.currMapCharPos[-y, x] = 0;
                    transform.position = new Vector2(transform.position.x - 1, transform.position.y);
                    if (tileData.currMapCharPos[-y, x - 1] == 0)
                    {
                        tileData.currMapCharPos[-y, x - 1] = charId;
                    }
                    StartCoroutine(DelayMovement(x - 1, y, false, false, true, false));
                }
            }
            else if (distanceRight < distanceBottom && distanceRight < distanceLeft && distanceRight < distanceTop && !facingLeft && !dontGoRight)
            {
                dontGoUp = true;
                dontGoDown = true;
                dontGoLeft = true;
                dontGoRight = true;

                if (tileData.currMapCharPos[-y, x + 1] == 0 || tileData.rowsMovement[-y].transform.GetChild(x + 2).CompareTag("MoveTile") && tileData.currMapCharPos[-y, x + 2] == 0 && tileData.currMapCharPos[-y, x + 1] != 0)
                {
                    dontGoUp = false;
                    dontGoDown = false;
                    dontGoLeft = false;
                    dontGoRight = false;
                    moved = true;

                    tileData.currMapCharPos[-y, x] = 0;
                    transform.position = new Vector2(transform.position.x + 1, transform.position.y);
                    if (tileData.currMapCharPos[-y, x + 1] == 0)
                    {
                        tileData.currMapCharPos[-y, x + 1] = charId;
                    }
                    StartCoroutine(DelayMovement(x + 1, y, false, false, false, true));
                }
            }
        }       

        if (!moved || dontGoDown && dontGoLeft && dontGoRight && dontGoUp && !tileData.rowsMovement[-Mathf.RoundToInt(target.position.y + 0.5f)].transform.GetChild(Mathf.RoundToInt(target.position.x - 0.5f)).CompareTag("MoveTileRed") && !inRange)
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
