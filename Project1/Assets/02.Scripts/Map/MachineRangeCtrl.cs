using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MachineRangeCtrl : MonoBehaviour
{
    private PhotonView pv = null;

    public MachineCtrl Machine;

    [HideInInspector]
    public int MachineNum;          // 자리 넘버
    private bool Use = false;       // 사용 중인 자리인지

    void Start()
    {
        pv = GetComponent<PhotonView>();

        MachineNum = Convert.ToInt32(transform.gameObject.name);
    }

    public bool GetMachineUse() { return Use; }
    public void SetMachineUse(bool use) { pv.RPC("SpaceUse", PhotonTargets.All, use); }

    [PunRPC]
    void SpaceUse(bool use) { Use = use; }
}
