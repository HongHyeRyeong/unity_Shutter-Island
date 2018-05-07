using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineRangeCtrl : MonoBehaviour
{
    private PhotonView pv = null;

    bool currUse = false;

    //
    public GameObject Machine;
    bool Use = false;

    void Start()
    {
        pv = GetComponent<PhotonView>();
    }

    public void SetMachineUse(bool b)
    {
        currUse = b;
        pv.RPC("SpaceUse", PhotonTargets.All);
    }

    [PunRPC]
    void SpaceUse()
    {
        Use = currUse;
    }

    public bool GetMachineUse()
    {
        return Use;
    }
}
