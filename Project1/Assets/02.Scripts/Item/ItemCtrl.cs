using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCtrl : MonoBehaviour
{
    public int ItemType;
    public int ItemLevel;

    int ItemNum;

    private void Start()
    {
        ItemNum = -1;

        GameObject items;
        items = GameObject.Find("Items");
        this.transform.parent = items.transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Survivor")
        {
            ItemNum = GameObject.Find("GameController").GetComponent<ItemsCtrl>().SurvivorEnterItems(this.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Survivor")
        {
            GameObject.Find("GameController").GetComponent<ItemsCtrl>().SurvivorExitItems(ItemNum);
            ItemNum = -1;
        }
    }

    public int GetItemNum()
    {
        return ItemNum;
    }
}
