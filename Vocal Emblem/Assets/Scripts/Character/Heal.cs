using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Heal : MonoBehaviour
{
    [SerializeField] private List<GameObject> allies = new List<GameObject>();

    public List<GameObject> GetAllies(){ return allies;}

    public void SetAllies(List<GameObject> alliesA)
    {
        if (alliesA.Count > 0)
        {
            allies = alliesA;
        }        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
