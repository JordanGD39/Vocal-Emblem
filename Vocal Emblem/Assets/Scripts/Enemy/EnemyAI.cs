using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public bool wait = false;
    private TileData tileData;
    private Stats stats;

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
            tileData.RevealTiles(Mathf.RoundToInt(transform.position.x - 0.5f), Mathf.RoundToInt(transform.position.y - 0.5f), Mathf.RoundToInt(stats.mov), stats.equippedWeapon.range, true);
        }
        else
        {
            tileData.RevealTiles(Mathf.RoundToInt(transform.position.x - 0.5f), Mathf.RoundToInt(transform.position.y - 0.5f), Mathf.RoundToInt(stats.mov), stats.equippedWeapon.range, false);
        }
    }
}
