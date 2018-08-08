using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCtrl : MonoBehaviour
{
    private PhotonView pv = null;

    public int ItemType;
    public int ItemLevel;

    private int includeNum = -1;

    private void Start()
    {
        pv = GetComponent<PhotonView>();

        transform.parent = ItemsCtrl.instance.Items.transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Survivor") && other.gameObject.GetComponent<SurvivorCtrl>().pv.isMine)
            includeNum = SurvivorUICtrl.instance.SurvivorEnterItems(this.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Survivor") && other.gameObject.GetComponent<SurvivorCtrl>().pv.isMine)
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
