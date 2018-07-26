using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCtrl : MonoBehaviour
{
    private PhotonView pv = null;

    public int ItemType;
    public int ItemLevel;

    int includeNum;

    private void Start()
    {
        pv = GetComponent<PhotonView>();

        includeNum = -1;

        GameObject items;
        items = GameObject.Find("Items");
        this.transform.parent = items.transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Survivor")
        {
            includeNum = SurvivorUICtrl.instance.SurvivorEnterItems(this.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Survivor")
        {
            SurvivorUICtrl.instance.SurvivorExitItems(includeNum);
            includeNum = -1;
        }
    }

    public void SetUse(bool b)
    {
        pv.RPC("RPCSetUse", PhotonTargets.AllBuffered, b);
    }

    [PunRPC]
    public void RPCSetUse(bool b)
    {
        this.gameObject.SetActive(b);
    }

    public int GetincludeNum()
    {
        return includeNum;
    }
}
