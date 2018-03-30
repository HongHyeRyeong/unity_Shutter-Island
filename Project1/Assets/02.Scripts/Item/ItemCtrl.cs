using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCtrl : MonoBehaviour
{
    public int ItemType;
    public int ItemLevel;

    public int ItemsNum;

    private void Start()
    {
        ItemsNum = -1;

        GameObject items;
        items = GameObject.Find("Items");
        this.transform.parent = items.transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Survivor")
        {
            ItemsNum = GameObject.Find("GameController").GetComponent<ItemsCtrl>().SurvivorEnterItems(this.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Survivor")
        {
            GameObject.Find("GameController").GetComponent<ItemsCtrl>().SurvivorExitItems(ItemsNum);
            ItemsNum = -1;
        }
    }
}
