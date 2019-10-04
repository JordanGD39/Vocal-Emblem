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
            tileData.RevealTiles(Mathf.RoundToInt(transform.position.x - 0.5f), Mathf.RoundToInt(transform.position.y - 0.5f), Mathf.RoundToInt(stats.mov), stats.equippedWeapon.range, true, !current, 2);
        }
        else
        {
            tileData.RevealTiles(Mathf.RoundToInt(transform.position.x - 0.5f), Mathf.RoundToInt(transform.position.y - 0.5f), Mathf.RoundToInt(stats.mov), stats.equippedWeapon.range, false, !current, 2);
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

            WalkingTowardsTarget(x, y);
        }        
    }

    private void WalkingTowardsTarget(int x, int y)
    {
        float distanceTop = 0;
        float distanceBottom = 0;
        float distanceLeft = 0;
        float distanceRight = 0;

        if (y - 1 > -1 && !tileData.rowsMovement[-y - 1].transform.GetChild(x).CompareTag("MoveTileRed"))
        {
            distanceTop = Mathf.Abs(transform.position.x - target.position.x) + Mathf.Abs((transform.position.y - 1) - target.position.y);
        }
        distanceBottom = Mathf.Abs(transform.position.x - target.position.x) + Mathf.Abs((transform.position.y + 1) - target.position.y);
        distanceLeft = Mathf.Abs((transform.position.x - 1) - target.position.x) + Mathf.Abs(transform.position.y - target.position.y);
        distanceRight = Mathf.Abs((transform.position.x + 1) - target.position.x) + Mathf.Abs(transform.position.y - target.position.y);

        if (distanceTop < distanceBottom && distanceTop < distanceLeft && distanceTop < distanceRight)
        {
            transform.position = new Vector2(0, transform.position.y - 1);
        }
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
