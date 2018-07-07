using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurvivorAnimCtrl : MonoBehaviour
{
    private SurvivorCtrl parent;

    void Start()
    {
        parent = transform.parent.GetComponent<SurvivorCtrl>();
    }

    public void SetState(int s)
    {
        parent.SetState(s);
    }
}
