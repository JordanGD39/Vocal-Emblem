using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatHolder : MonoBehaviour
{
    public List<Stats> charStats = new List<Stats>();

    public static StatHolder instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < GameManager.instance.playerTeam.Count; i++)
        {
            charStats[i] = GameManager.instance.playerTeam[i].GetComponent<Stats>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
