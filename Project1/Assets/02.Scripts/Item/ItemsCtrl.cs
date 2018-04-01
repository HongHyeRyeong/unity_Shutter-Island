using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsCtrl : MonoBehaviour
{
    GameObject[] Items = new GameObject[100];
    int numItem = 0;

    public GameObject ItemHat1;
    public GameObject ItemClothes1;
    public GameObject ItemBag1;

    private void Update()
    {
        if (numItem > 0)
        {
            for (int i = 00; i < 100; ++i)
                if (Items[i] != null)
                {
                    GameObject.Find("GameController").GetComponent<UICtrl>().DispItemHUD(
                        Items[i].transform.position,
                        Items[i].GetComponent<ItemCtrl>().ItemType,
                        Items[i].GetComponent<ItemCtrl>().ItemLevel);
                    GameObject.Find("Survivor").GetComponent<SurvivorItem>().SurvivorEnterItem(Items[i]);

                    break;
                }
        }

        if (numItem == 0)
        {
            if (GameObject.Find("GameController").GetComponent<UICtrl>().HUDItem.activeSelf)
                GameObject.Find("GameController").GetComponent<UICtrl>().HUDItem.SetActive(false);
        }
    }

    public void CreateItem(Vector3 position, int type, int level)
    {
        if (type == 1)
        {
            if (level == 1)
                Instantiate(ItemHat1, position, Quaternion.Euler(-90, 0, 0));
        }
        else if (type == 2)
        {
            if (level == 1)
                Instantiate(ItemClothes1, position, Quaternion.Euler(0, 0, 180));
        }
        else if (type == 3)
        {
            if (level == 1)
                Instantiate(ItemBag1, position, Quaternion.identity);
        }
    }

    public int SurvivorEnterItems(GameObject Item)
    {
        Items[numItem++] = Item;
        return numItem - 1;
    }

    public void SurvivorExitItems(int ItemsNum)
    {
        Items[ItemsNum] = null;
        numItem--;
    }
}
