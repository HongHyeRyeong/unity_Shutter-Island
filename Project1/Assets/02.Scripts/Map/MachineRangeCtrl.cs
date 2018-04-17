using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineRangeCtrl : MonoBehaviour
{
    public GameObject Machine;
    bool Use = false;

    public void SetMachineUse(bool b)
    {
        Use = b;
    }

    public bool GetMachineUse()
    {
        return Use;
    }
}
