using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurvivorAnimCtrl : MonoBehaviour
{
    private SurvivorCtrl parentCtrl;
    private SurvivorItem parentItem;

    void Start()
    {
        parentCtrl = transform.parent.GetComponent<SurvivorCtrl>();
        parentItem = transform.parent.GetComponent<SurvivorItem>();
    }

    public void SetState(int s)
    {
        parentCtrl.SetState(s);
    }

    public void TrapOff()
    {
        parentCtrl.Trap = false;
    }

    public void Item()
    {
        parentItem.PickAnimEnd();
    }
}
