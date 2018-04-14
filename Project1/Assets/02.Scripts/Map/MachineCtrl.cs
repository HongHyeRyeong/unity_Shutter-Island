using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MachineCtrl : MonoBehaviour
{
    private Animator Ani;

    public GameObject HUD;
    public Image imgHUD;
    public Text txtHUD;

    public GameObject CompleteLight;

    public int MachineNum;
    float MachineGauge = 0;
    public bool Complete;

    public bool GadgetUse;
    float GadgetGauge = 10f;
    int GadgetNum = 0;

    void Start()
    {
        Ani = GetComponent<Animator>();

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
            GadgetNum++;

            MachineGauge = 10 * GadgetNum;

            if (MachineGauge > 50f)
            {
                Complete = true;
                CompleteLight.SetActive(true);
                Ani.SetTrigger("Complete");
            }
            else
                Ani.SetTrigger("Install");

            return true;
        }

        return false;
    }

    public void DisHUD(Vector3 pos)
    {
        HUD.SetActive(true);

        pos.y = HUD.transform.position.y;
        Vector3 vec = pos - HUD.transform.position;
        vec.Normalize();

        HUD.transform.rotation = Quaternion.LookRotation(vec);

        txtHUD.text = MachineGauge.ToString("N2");
        imgHUD.fillAmount = MachineGauge / 50;
    }
}
