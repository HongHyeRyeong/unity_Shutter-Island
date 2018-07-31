using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsCtrl : MonoBehaviour
{
    public static ItemsCtrl instance;

    public GameObject Items;

    private void Start()
    {
        instance = this;
    }

    public void SetItem(Vector3 position, int type, int level)
    {
        int itemsCount = Items.transform.childCount;

        for (int i = 0; i < itemsCount; ++i)
        {
            GameObject item = Items.transform.GetChild(i).transform.gameObject;

            if (item.activeSelf)
                continue;

            if (item.GetComponent<ItemCtrl>().ItemType == type && item.GetComponent<ItemCtrl>().ItemLevel == level)
            {
                item.transform.position = position;
                item.SetActive(true);
                break;
            }
        }
    }
}