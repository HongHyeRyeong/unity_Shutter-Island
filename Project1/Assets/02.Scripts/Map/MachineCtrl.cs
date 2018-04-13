using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineCtrl : MonoBehaviour
{
    private Animator ani;

    public int MachineNum;
    float MachineGauge = 0;
    public bool Complete;

    public bool GadgetUse;
    float GadgetGauge = 10f;

    void Start()
    {
        ani = GetComponent<Animator>();

        Complete = false;
        GadgetUse = false;
    }

    public bool Install(float work)
    {
        GadgetGauge -= work;
        MachineGauge += work;

        if (GadgetGauge < 0)
        {
            GadgetUse = false;
            GadgetGauge = 10f;

            if (MachineGauge > 50f)
            {
                Complete = true;
                ani.SetTrigger("Complete");
            }
            else
                ani.SetTrigger("Install");

            return true;
        }

        return false;
    }
}
