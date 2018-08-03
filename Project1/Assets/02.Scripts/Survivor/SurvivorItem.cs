using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurvivorItem : MonoBehaviour
{
    [SerializeField]
    private PhotonView pv = null;
    [SerializeField]
    private SurvivorCtrl ctrl;

    [SerializeField]
    private GameObject Hat;
    [SerializeField]
    private GameObject Clothes;
    [SerializeField]
    private GameObject Bag;

    // type 1~5 순서
    private bool[] ItemHat = new bool[3] { false, false, false };
    private bool[] ItemClothes = new bool[3] { false, false, false };
    private bool[] ItemBag = new bool[2] { false, false };
    private int ItemGadget = 0;
    private int ItemMaxGadget = 2;
    private bool ItemKey = false;

    private GameObject saveItem;
    private int saveType = -1;
    private int saveLevel = -1;

    void SurvivorByItem(int type, int level)
    {
        if (type == 1)
        {
            if (level == 0)
                ctrl.SetStatus("WorkSpeed", 0);
            else if (level == 1)
                ctrl.SetStatus("WorkSpeed", 0.1f);
            else if (level == 2)
                ctrl.SetStatus("WorkSpeed", 0.2f);
            else if (level == 3)
                ctrl.SetStatus("WorkSpeed", 0.3f);
        }
        else if (type == 2)
        {
            if (level == 0)
                ctrl.SetStatus("Stamina", 0);
            else if (level == 1)
                ctrl.SetStatus("Stamina", 0.5f);
            else if (level == 2)
                ctrl.SetStatus("Stamina", 1);
            else if (level == 3)
                ctrl.SetStatus("Stamina", 2);
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
                    ItemsCtrl.instance.SetItem(transform.position, 4, 0);
                }
            }

            SurvivorUICtrl.instance.DisGadget(ItemGadget);
        }
    }

    public void SurvivorEnterItem(GameObject Item)
    {
        int state = ctrl.GetState();

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

            ctrl.SetState(4);
        }
    }

    public void PickAnimEnd()
    {
        if (saveItem == null)
            return;

        ItemPick(saveType, saveLevel);
        SurvivorUICtrl.instance.SurvivorExitItems(saveItem.GetComponent<ItemCtrl>().GetincludeNum());
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
            SurvivorUICtrl.instance.UpdateItemInformation(type);
        }
        else if (type == 2)
        {
            for (int i = 0; i < 3; ++i)
                if (ItemClothes[i])
                    ItemPut(type);

            ItemClothes[level - 1] = true;
            SurvivorByItem(type, level);

            pv.RPC("ClothesEquip", PhotonTargets.AllBuffered, type, level);
            SurvivorUICtrl.instance.UpdateItemInformation(type);
        }
        else if (type == 3)
        {
            for (int i = 0; i < 2; ++i)
                if (ItemBag[i])
                    ItemPut(type);

            ItemBag[level - 1] = true;
            SurvivorByItem(type, level);

            pv.RPC("BagEquip", PhotonTargets.AllBuffered, type, level);
            SurvivorUICtrl.instance.UpdateItemInformation(type);
        }
        else if (type == 4)
        {
            ItemGadget++;
            SurvivorUICtrl.instance.DisGadget(ItemGadget);
        }
        else if (type == 5)
        {
            ItemKey = true;
            SurvivorUICtrl.instance.DisKey(true);
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
                    ctrl.SetState(4);

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
                    ctrl.SetState(4);

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
                    ctrl.SetState(4);

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
            SurvivorUICtrl.instance.DisGadget(ItemGadget);
        }
        else if (type == 5)
        {
            if (num == 1)
                ItemKey = true;
            else
                ItemKey = false;
            SurvivorUICtrl.instance.DisKey(ItemKey);
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
        ItemsCtrl.instance.SetItem(this.transform.position, type, level);
    }

    [PunRPC]
    public void ClothesTakeOff(int type, int level)
    {
        Clothes.transform.Find("Item" + type.ToString() + level.ToString()).gameObject.SetActive(false);
        ItemsCtrl.instance.SetItem(this.transform.position, type, level);
    }

    [PunRPC]
    public void BagTakeOff(int type, int level)
    {
        Bag.transform.Find("Item" + type.ToString() + level.ToString()).gameObject.SetActive(false);
        ItemsCtrl.instance.SetItem(this.transform.position, type, level);
    }
}