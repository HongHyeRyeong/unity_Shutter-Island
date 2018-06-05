using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsCtrl : MonoBehaviour
{
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

    int[] hat = new int[] { 10, 4, 2 };
    int[] Clothes = new int[] { 10, 4, 2 };
    int[] Bag = new int[] { 10, 4 };
    int GadgetNum = 35;
    int keyNum = 10;

    private void Start()
    {
        GameObject[] spawns = GameObject.FindGameObjectsWithTag("Spawn");

        int hatNum = hat[0] + hat[1] + hat[2];
        int clothesNum = Clothes[0] + Clothes[1] + Clothes[2];
        int bagNum = Bag[0] + Bag[1];
        int itemnum = hatNum + clothesNum + bagNum;

        for (int i = 0; i < itemnum; ++i)
        {
            int random = Random.Range(0, spawns.Length);

            Vector3 pos = new Vector3(
                spawns[random].transform.position.x + Random.Range(-3.0f, 3.0f),
                spawns[random].transform.position.y,
                spawns[random].transform.position.z + Random.Range(-3.0f, 3.0f));

            if (i < hatNum)
            {
                if (i < hat[0])
                    Instantiate(ItemHat1, pos, Quaternion.Euler(-90, 0, 0));
                else if (hat[0] <= i && i < hat[0] + hat[1])
                    Instantiate(ItemHat2, pos, Quaternion.Euler(-90, 0, 0));
                else
                    Instantiate(ItemHat3, pos, Quaternion.Euler(-90, 0, 0));
            }
            else if (hatNum <= i && i < hatNum + clothesNum)
            {
                if (i < hatNum + Clothes[0])
                    Instantiate(ItemClothes1, pos, Quaternion.identity);
                else if (hatNum + Clothes[0] <= i && i < hatNum + Clothes[0] + Clothes[1])
                    Instantiate(ItemClothes2, pos, Quaternion.identity);
                else
                    Instantiate(ItemClothes3, pos, Quaternion.identity);
            }
            else
            {
                if (i < hatNum + clothesNum + Bag[0])
                    Instantiate(ItemBag1, pos, Quaternion.identity);
                else
                    Instantiate(ItemBag2, pos, Quaternion.identity);
            }
        }

        for (int i = 0; i < GadgetNum; ++i)
        {
            int random = Random.Range(0, spawns.Length);

            Vector3 pos = new Vector3(
                spawns[random].transform.position.x + Random.Range(-3.0f, 3.0f),
                spawns[random].transform.position.y,
                spawns[random].transform.position.z + Random.Range(-3.0f, 3.0f));

            Instantiate(ItemGadget, pos, Quaternion.Euler(-90, 0, 0));
        }

        for (int i = 0; i < keyNum; ++i)
        {
            int random = Random.Range(0, spawns.Length);

            Vector3 pos = new Vector3(
                spawns[random].transform.position.x + Random.Range(-3.0f, 3.0f),
                spawns[random].transform.position.y,
                spawns[random].transform.position.z + Random.Range(-3.0f, 3.0f));

            Instantiate(ItemKey, pos, Quaternion.identity);
        }
    }

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