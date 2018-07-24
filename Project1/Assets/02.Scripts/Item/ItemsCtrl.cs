using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsCtrl : MonoBehaviour
{
    public void SetItem(Vector3 position, int type, int level)
    {
        int itemsCount = GameObject.Find("Items").transform.childCount;

        for (int i = 0; i < itemsCount; ++i)
        {
            GameObject item = GameObject.Find("Items").transform.GetChild(i).transform.gameObject;

            if (item.activeSelf)
                continue;

            if (item.GetComponent<ItemCtrl>().ItemType == type &&
                item.GetComponent<ItemCtrl>().ItemLevel == level)
            {
                item.transform.position = position;
                item.SetActive(true);
                break;
            }
        }
    }
}