using System.Collections;
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
            ItemPut(Item.GetComponent<ItemCtrl>().ItemType, Item.GetComponent<ItemCtrl>().ItemLevel);
            Destroy(Item);
        }
    }

    public void ItemPut(int type, int level)
    {
        if (type == 1)
        {
            for (int i = 0; i < 3; ++i)
                ItemHat[i] = false;
            ItemHat[level - 1] = true;

            GameObject.Find("Bip001 Head").transform.Find("Item" + type.ToString() + level.ToString()).gameObject.SetActive(true);

            //for (int i = 1; i < 4; ++i)
            //    GameObject.Find("Bip001 Head").transform.Find("Item" + type.ToString() + i.ToString()).gameObject.SetActive(ItemHat[i - 1]);
        }
        else if (type == 2)
        {
            for (int i = 0; i < 3; ++i)
                ItemClothes[i] = false;
            ItemClothes[level - 1] = true;

            GameObject.Find("Bip001 Spine1").transform.Find("Item" + type.ToString() + level.ToString()).gameObject.SetActive(true);
        }
        else if (type == 3)
        {
            for (int i = 0; i < 2; ++i)
                ItemBag[i] = false;
            ItemBag[level - 1] = true;
        }
    }
}