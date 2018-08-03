using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurvivorAnimCtrl : MonoBehaviour
{
    [SerializeField]
    private SurvivorCtrl parentCtrl;
    [SerializeField]
    private SurvivorItem parentItem;

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
