using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCtrl : MonoBehaviour
{
    public int ItemType;
    public int ItemLevel;

    int includeNum;

    private void Start()
    {
        includeNum = -1;

        GameObject items;
        items = GameObject.Find("Items");
        this.transform.parent = items.transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Survivor")
        {
            includeNum = GameObject.Find("SurvivorController").GetComponent<ItemsCtrl>().SurvivorEnterItems(this.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Survivor")
        {
            GameObject.Find("SurvivorController").GetComponent<ItemsCtrl>().SurvivorExitItems(includeNum);
            includeNum = -1;
        }
    }

    public int GetincludeNum()
    {
        return includeNum;
    }
}
