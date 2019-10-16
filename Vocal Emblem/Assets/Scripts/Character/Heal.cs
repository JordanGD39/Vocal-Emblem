using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Heal : MonoBehaviour
{
    [SerializeField] private List<GameObject> allies = new List<GameObject>();

    public List<GameObject> GetAllies(){ return allies; }

    public void SetAllies(List<GameObject> alliesA)
    {
        allies = alliesA;
    }
}
