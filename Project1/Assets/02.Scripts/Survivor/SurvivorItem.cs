using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurvivorItem : MonoBehaviour
{
    GameObject Hat;
    GameObject Clothes;
    GameObject Bag;

    private PhotonView pv = null;

    // type 1~5 순서
    bool[] ItemHat = new bool[3] { false, false, false };
    bool[] ItemClothes = new bool[3] { false, false, false };
    bool[] ItemBag = new bool[2] { false, false };
    int ItemGadget = 0;
    int ItemMaxGadget = 2;
    bool ItemKey = false;

    int currhattype = 1;
    int currclothestype = 2;
    int currbagtype = 3;

    GameObject saveItem;
    int saveType = -1;
    int saveLevel = -1;

    private void Start()
    {
        pv = GetComponent<PhotonView>();

        Hat = this.gameObject.transform.Find("SurvivorModel/Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 Head/ItemHat").gameObject;
        Clothes = this.gameObject.transform.Find("SurvivorModel/ItemClothes").gameObject;
        Bag = this.gameObject.transform.Find("SurvivorModel/Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/ItemBag").gameObject;
    }

    void SurvivorByItem(int type, int level)
    {
        if (type == 1)
        {
            if (level == 0)
                GetComponent<SurvivorCtrl>().SetStatus("WorkSpeed", 0);
            else if (level == 1)
                GetComponent<SurvivorCtrl>().SetStatus("WorkSpeed", 0.1f);
            else if (level == 2)
                GetComponent<SurvivorCtrl>().SetStatus("WorkSpeed", 0.2f);
            else if (level == 3)
                GetComponent<SurvivorCtrl>().SetStatus("WorkSpeed", 0.3f);
        }
        else if (type == 2)
        {
            if (level == 0)
                GetComponent<SurvivorCtrl>().SetStatus("Stamina", 0);
            else if (level == 1)
                GetComponent<SurvivorCtrl>().SetStatus("Stamina", 0.5f);
            else if (level == 2)
                GetComponent<SurvivorCtrl>().SetStatus("Stamina", 1);
            else if (level == 3)
                GetComponent<SurvivorCtrl>().SetStatus("Stamina", 2);
        }
        else if (type == 3)
        {
            if (level == 0)
                ItemMaxGadget = 2;
            else if (level == 1)
                ItemMaxGadget = 3;
            else if (level == 2)
                ItemMaxGadget = 4;

            if (ItemGadget > ItemMaxGadget)
            {
                while (ItemGadget > ItemMaxGadget)
                {
                    ItemGadget--;
                    GameObject.Find("GameController").GetComponent<ItemsCtrl>().SetItem(this.transform.position, 4, 0);
                }
            }

            GameObject.Find("SurvivorController").GetComponent<SurvivorUICtrl>().DisGadget(ItemGadget);
        }
    }

    public void SurvivorEnterItem(GameObject Item)
    {
        int state = GetComponent<SurvivorCtrl>().GetState();

        if (Input.GetKeyDown(KeyCode.F) && (state == 0 || state == 1 || state == 2))
        {
            saveItem = Item;
            saveType = Item.GetComponent<ItemCtrl>().ItemType;
            saveLevel = Item.GetComponent<ItemCtrl>().ItemLevel;

            if (saveType == 4)
            {
                if (ItemGadget == ItemMaxGadget)
                {
                    print("Max Gadget");
                    return;
                }
            }
            else if (saveType == 5)
            {
                if (ItemKey)
                {
                    print("Max Key");
                    return;
                }
            }

            GetComponent<SurvivorCtrl>().SetState(4);
        }
    }

    public void PickAnimEnd()
    {
        if (saveItem == null)
            return;

        ItemPick(saveType, saveLevel);
        GameObject.Find("SurvivorController").GetComponent<SurvivorUICtrl>().
            SurvivorExitItems(saveItem.GetComponent<ItemCtrl>().GetincludeNum());
        saveItem.GetComponent<ItemCtrl>().SetUse(false);

        saveItem = null;
        saveType = -1;
        saveLevel = -1;
    }

    void ItemPick(int type, int level)
    {
        if (type == 1)
        {
            for (int i = 0; i < 3; ++i)
                if (ItemHat[i])
                    ItemPut(type);

            ItemHat[level - 1] = true;
            SurvivorByItem(type, level);

            pv.RPC("HatEquip", PhotonTargets.AllBuffered, type, level);
            GameObject.Find("SurvivorController").GetComponent<SurvivorUICtrl>().UpdateItemInformation(type);
        }
        else if (type == 2)
        {
            for (int i = 0; i < 3; ++i)
                if (ItemClothes[i])
                    ItemPut(type);

            ItemClothes[level - 1] = true;
            SurvivorByItem(type, level);

            pv.RPC("ClothesEquip", PhotonTargets.AllBuffered, type, level);
            GameObject.Find("SurvivorController").GetComponent<SurvivorUICtrl>().UpdateItemInformation(type);
        }
        else if (type == 3)
        {
            for (int i = 0; i < 2; ++i)
                if (ItemBag[i])
                    ItemPut(type);

            ItemBag[level - 1] = true;
            SurvivorByItem(type, level);

            pv.RPC("BagEquip", PhotonTargets.AllBuffered, type, level);
            GameObject.Find("SurvivorController").GetComponent<SurvivorUICtrl>().UpdateItemInformation(type);
        }
        else if (type == 4)
        {
            ItemGadget++;
            GameObject.Find("SurvivorController").GetComponent<SurvivorUICtrl>().DisGadget(ItemGadget);
        }
        else if (type == 5)
        {
            ItemKey = true;
            GameObject.Find("SurvivorController").GetComponent<SurvivorUICtrl>().DisKey(true);
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
                    GetComponent<SurvivorCtrl>().SetState(4);

                    int level = i + 1;
                    pv.RPC("HatTakeOff", PhotonTargets.AllBuffered, type, level);

                    break;
                }
            }
        }
        else if (type == 2)
        {
            for (int i = 0; i < 3; ++i)
            {
                if (ItemClothes[i])
                {
                    ItemClothes[i] = false;
                    GetComponent<SurvivorCtrl>().SetState(4);

                    int level = i + 1;
                    pv.RPC("ClothesTakeOff", PhotonTargets.AllBuffered, type, level);

                    break;
                }
            }
        }
        else if (type == 3)
        {
            for (int i = 0; i < 2; ++i)
            {
                if (ItemBag[i])
                {
                    ItemBag[i] = false;
                    GetComponent<SurvivorCtrl>().SetState(4);

                    int level = i + 1;
                    pv.RPC("BagTakeOff", PhotonTargets.AllBuffered, type, level);

                    break;
                }
            }
        }

        SurvivorByItem(type, 0);
    }

    public int ItemGet(int type)
    {
        if (type == 1)
        {
            for (int i = 0; i < 3; ++i)
                if (ItemHat[i])
                    return i + 1;
        }
        else if (type == 2)
        {
            for (int i = 0; i < 3; ++i)
                if (ItemClothes[i])
                    return i + 1;
        }
        else if (type == 3)
        {
            for (int i = 0; i < 2; ++i)
                if (ItemBag[i])
                    return i + 1;
        }
        else if (type == 4)
        {
            return ItemGadget;
        }
        else if (type == 5)
        {
            if (ItemKey)
                return 1;
            else
                return 0;
        }

        return 0;
    }

    public void ItemSet(int type, int num)
    {
        if (type == 4)
        {
            ItemGadget = num;
            GameObject.Find("SurvivorController").GetComponent<SurvivorUICtrl>().DisGadget(ItemGadget);
        }
        else if (type == 5)
        {
            if (num == 1)
                ItemKey = true;
            else
                ItemKey = false;
            GameObject.Find("SurvivorController").GetComponent<SurvivorUICtrl>().DisKey(ItemKey);
        }
    }

    [PunRPC]
    public void HatEquip(int type, int level)
    {
        Hat.transform.Find("Item" + type.ToString() + level.ToString()).gameObject.SetActive(true);
    }

    [PunRPC]
    public void ClothesEquip(int type, int level)
    {
        Clothes.transform.Find("Item" + type.ToString() + level.ToString()).gameObject.SetActive(true);
    }

    [PunRPC]
    public void BagEquip(int type, int level)
    {
        Bag.transform.Find("Item" + type.ToString() + level.ToString()).gameObject.SetActive(true);
    }

    [PunRPC]
    public void HatTakeOff(int type, int level)
    {
        Hat.transform.Find("Item" + type.ToString() + level.ToString()).gameObject.SetActive(false);
        GameObject.Find("GameController").GetComponent<ItemsCtrl>().SetItem(this.transform.position, type, level);
    }

    [PunRPC]
    public void ClothesTakeOff(int type, int level)
    {
        Clothes.transform.Find("Item" + type.ToString() + level.ToString()).gameObject.SetActive(false);
        GameObject.Find("GameController").GetComponent<ItemsCtrl>().SetItem(this.transform.position, type, level);
    }

    [PunRPC]
    public void BagTakeOff(int type, int level)
    {
        Bag.transform.Find("Item" + type.ToString() + level.ToString()).gameObject.SetActive(false);
        GameObject.Find("GameController").GetComponent<ItemsCtrl>().SetItem(this.transform.position, type, level);
    }
}