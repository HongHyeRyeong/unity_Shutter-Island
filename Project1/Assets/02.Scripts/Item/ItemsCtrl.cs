using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsCtrl : MonoBehaviour
{
    public GameObject ItemHat1;
    public GameObject ItemBag1;

    public void CreateItem(Vector3 position, int type, int level)
    {
        if (type == 1)
        {
            if (level == 1)
                Instantiate(ItemHat1, position, Quaternion.identity);
        }
        else if (type == 2)
        {

        }
        else if (type == 3)
        {
            if (level == 1)
                Instantiate(ItemBag1, position, Quaternion.identity);
        }
    }
}
