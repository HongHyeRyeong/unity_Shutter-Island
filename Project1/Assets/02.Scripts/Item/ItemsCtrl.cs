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

    //int[] hat = new int[] { 10, 4, 2 };
    //int[] Clothes = new int[] { 10, 4, 2 };
    //int[] Bag = new int[] { 10, 4 };
    //int GadgetNum = 35;
    //int keyNum = 10;

    //void Start()
    //{
    //    PhotonNetwork.isMessageQueueRunning = true;

    //    GameObject[] spawns = GameObject.FindGameObjectsWithTag("Spawn");

    //    int hatNum = hat[0] + hat[1] + hat[2];
    //    int clothesNum = Clothes[0] + Clothes[1] + Clothes[2];
    //    int bagNum = Bag[0] + Bag[1];
    //    int itemnum = hatNum + clothesNum + bagNum;

    //    for (int i = 0; i < itemnum; ++i)
    //    {
    //        Vector3 pos = new Vector3(
    //            spawns[PlayerPrefs.GetInt("itemrand" + (i + 1))].transform.position.x + PlayerPrefs.GetFloat("random" + (i + 1)),
    //            spawns[PlayerPrefs.GetInt("itemrand" + (i + 1))].transform.position.y,
    //            spawns[PlayerPrefs.GetInt("itemrand" + (i + 1))].transform.position.z + PlayerPrefs.GetFloat("random" + (i + 47)));

    //        if (i < hatNum)
    //        {
    //            if (i < hat[0])
    //                PhotonNetwork.Instantiate("ItemHat1", pos, Quaternion.Euler(-90, 0, 0), 0);
    //            else if (hat[0] <= i && i < hat[0] + hat[1])
    //                PhotonNetwork.Instantiate("ItemHat2", pos, Quaternion.Euler(-90, 0, 0), 0);
    //            else
    //                PhotonNetwork.Instantiate("ItemHat3", pos, Quaternion.Euler(-90, 0, 0), 0);
    //        }
    //        else if (hatNum <= i && i < hatNum + clothesNum)
    //        {
    //            if (i < hatNum + Clothes[0])
    //                PhotonNetwork.Instantiate("ItemClothes1", pos, Quaternion.identity, 0);
    //            else if (hatNum + Clothes[0] <= i && i < hatNum + Clothes[0] + Clothes[1])
    //                PhotonNetwork.Instantiate("ItemClothes2", pos, Quaternion.identity, 0);
    //            else
    //                PhotonNetwork.Instantiate("ItemClothes3", pos, Quaternion.identity, 0);
    //        }
    //        else
    //        {
    //            if (i < hatNum + clothesNum + Bag[0])
    //                PhotonNetwork.Instantiate("ItemBag1", pos, Quaternion.identity, 0);
    //            else
    //                PhotonNetwork.Instantiate("ItemBag2", pos, Quaternion.identity, 0);
    //        }
    //    }

    //    for (int i = 0; i < GadgetNum; ++i)
    //    {
    //        Vector3 pos = new Vector3(
    //            spawns[PlayerPrefs.GetInt("gadgetrand" + (i + 47))].transform.position.x + PlayerPrefs.GetFloat("random" + (i + 93)),
    //            spawns[PlayerPrefs.GetInt("gadgetrand" + (i + 47))].transform.position.y,
    //            spawns[PlayerPrefs.GetInt("gadgetrand" + (i + 47))].transform.position.z + PlayerPrefs.GetFloat("random" + (i + 128)));

    //        PhotonNetwork.Instantiate("ItemGadget", pos, Quaternion.Euler(-90, 0, 0), 0);
    //    }

    //    for (int i = 0; i < keyNum; ++i)
    //    {
    //        Vector3 pos = new Vector3(
    //            spawns[PlayerPrefs.GetInt("keyrand" + (i + 82))].transform.position.x + PlayerPrefs.GetFloat("random1" + (i + 163)),
    //            spawns[PlayerPrefs.GetInt("keyrand" + (i + 82))].transform.position.y,
    //            spawns[PlayerPrefs.GetInt("keyrand" + (i + 82))].transform.position.z + PlayerPrefs.GetFloat("random2" + (i + 173)));

    //        PhotonNetwork.Instantiate("ItemKey", pos, Quaternion.identity, 0);
    //    }
    //}

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