using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCtrl : MonoBehaviour
{
    public int ItemType;
    public int ItemLevel;

    public int includeNum;

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
            includeNum = GameObject.Find("GameController").GetComponent<ItemsCtrl>().SurvivorEnterItems(this.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Survivor")
        {
            GameObject.Find("GameController").GetComponent<ItemsCtrl>().SurvivorExitItems(includeNum);
            includeNum = -1;
        }
    }
}
