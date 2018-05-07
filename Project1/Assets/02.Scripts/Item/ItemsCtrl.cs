using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsCtrl : MonoBehaviour
{
    GameObject Survivor;

    public GameObject ItemHat1;
    public GameObject ItemHat2;
    public GameObject ItemHat3;
    public GameObject ItemClothes1;
    public GameObject ItemClothes2;
    public GameObject ItemClothes3;
    public GameObject ItemBag1;
    public GameObject ItemBag2;
    public GameObject ItemGadget;
    public GameObject ItemKey;

    GameObject[] Items = new GameObject[100];
    int numItem = 0;

    private void Update()
    {
        if (numItem > 0)
        {
            for (int i = 00; i < 100; ++i)
                if (Items[i] != null)
                {
                    GetComponent<SurvivorUICtrl>().DispItemHUD(
                        Items[i].transform.position,
                        Items[i].GetComponent<ItemCtrl>().ItemType,
                        Items[i].GetComponent<ItemCtrl>().ItemLevel);
                    Survivor.GetComponent<SurvivorItem>().SurvivorEnterItem(Items[i]);

                    break;
                }
        }
        else if (numItem == 0)
        {
            if (GetComponent<SurvivorUICtrl>().HUDItem.activeSelf)
                GetComponent<SurvivorUICtrl>().HUDItem.SetActive(false);
        }
    }

    public void CreateItem(Vector3 position, int type, int level)
    {
        if (type == 1)
        {
            if (level == 1)
                Instantiate(ItemHat1, position, Quaternion.Euler(-90, 0, 0));
            else if (level == 2)
                Instantiate(ItemHat2, position, Quaternion.Euler(-90, 0, 0));
            else if (level == 3)
                Instantiate(ItemHat3, position, Quaternion.Euler(-90, 0, 0));
        }
        else if (type == 2)
        {
            if (level == 1)
                Instantiate(ItemClothes1, position, Quaternion.identity);
            else if (level == 2)
                Instantiate(ItemClothes2, position, Quaternion.identity);
            else if (level == 3)
                Instantiate(ItemClothes3, position, Quaternion.identity);
        }
        else if (type == 3)
        {
            if (level == 1)
                Instantiate(ItemBag1, position, Quaternion.identity);
            else if (level == 2)
                Instantiate(ItemBag2, position, Quaternion.identity);
        }
        else if (type == 4)
        {
            Instantiate(ItemGadget, position, Quaternion.identity);
        }
    }

    public int SurvivorEnterItems(GameObject item)
    {
        Items[numItem++] = item;
        return numItem - 1;
    }

    public void SurvivorExitItems(int ItemsNum)
    {
        Items[ItemsNum] = null;
        numItem--;
    }

    public void SetSurvivor(GameObject survivor)
    {
        Survivor = survivor;
    }
}
