﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurvivorItem : MonoBehaviour
{
    bool[] ItemHat = new bool[3] { false, false, false };
    bool[] ItemClothes = new bool[3] { false, false, false };
    bool[] ItemBag = new bool[2] { false, false };

    public void SurvivorEnterItem(GameObject Item)
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            this.GetComponent<SurvivorCtrl>().SetAnimation("isPickItem");
            ItemPick(Item.GetComponent<ItemCtrl>().ItemType, Item.GetComponent<ItemCtrl>().ItemLevel);
            Destroy(Item);
        }
    }

    public void ItemPick(int type, int level)
    {
        if (type == 1)
        {
            for (int i = 0; i < 3; ++i)
                if (ItemHat[i])
                    ItemPut(type);

            ItemHat[level - 1] = true;
            GameObject.Find("Bip001 Head").transform.Find("Item" + type.ToString() + level.ToString()).gameObject.SetActive(true);
        }
        else if (type == 2)
        {
            for (int i = 0; i < 3; ++i)
                if (ItemClothes[i])
                    ItemPut(type);

            ItemClothes[level - 1] = true;
        }
        else if (type == 3)
        {
            for (int i = 0; i < 2; ++i)
                if (ItemBag[i])
                    ItemPut(type);

            ItemBag[level - 1] = true;
            GameObject.Find("Bip001 Spine1").transform.Find("Item" + type.ToString() + level.ToString()).gameObject.SetActive(true);
        }
    }

    public void ItemPut(int type)
    {
        if (type == 1)
        {
            for (int i = 0; i < 3; ++i)
            {
                if (ItemHat[i])
                {
                    ItemHat[i] = false;
                    this.GetComponent<SurvivorCtrl>().SetAnimation("isPickItem");

                    int level = i + 1;
                    GameObject.Find("Bip001 Head").transform.Find("Item" + type.ToString() + level.ToString()).gameObject.SetActive(false);
                    GameObject.Find("GameController").GetComponent<ItemsCtrl>().CreateItem(this.transform.position, type, level);

                    break;
                }
            }
        }
        else if (type == 2)
        {

        }
        else if (type == 3)
        {
            for (int i = 0; i < 2; ++i)
            {
                if (ItemBag[i])
                {
                    ItemBag[i] = false;
                    this.GetComponent<SurvivorCtrl>().SetAnimation("isPickItem");

                    int level = i + 1;
                    GameObject.Find("Bip001 Spine1").transform.Find("Item" + type.ToString() + level.ToString()).gameObject.SetActive(false);
                    GameObject.Find("GameController").GetComponent<ItemsCtrl>().CreateItem(this.transform.position, type, level);

                    break;
                }
            }
        }
    }
}