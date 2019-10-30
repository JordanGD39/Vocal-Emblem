using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemButtonScript : MonoBehaviour
{
    public int index = 0;

    public void GiveIndexToSelect()
    {
        GameObject.FindGameObjectWithTag("Canvas").GetComponent<SelectChoices>().SetCurrentItem(index);
    }
}
