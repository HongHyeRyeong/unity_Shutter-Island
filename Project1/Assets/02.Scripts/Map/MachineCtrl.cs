using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MachineCtrl : MonoBehaviour
{
    private Animator Ani;

    GameObject HUD;
    Image imgHUD;
    Text txtHUD;

    GameObject CompleteLight;

    public int MachineNum;
    float MachineGauge = 0;
    bool Complete = false;

    bool GadgetUse = false;
    float GadgetGauge = 10f;
    int GadgetNum = 0;

    void Start()
    {
        Ani = GetComponent<Animator>();
        HUD = this.gameObject.transform.Find("HUDMachine").gameObject;
        imgHUD = HUD.transform.Find("imgHUDMachine").GetComponent<Image>();
        txtHUD = HUD.transform.Find("txtHUDMachine").GetComponent<Text>();
        CompleteLight = this.gameObject.transform.Find("Completelight").gameObject;
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

            if (MachineGauge == 50f)
            {
                Complete = true;
                CompleteLight.SetActive(true);
                HUD.SetActive(false);
                Ani.SetTrigger("Complete");
                GameObject.Find("GameController").GetComponent<GameCtrl>().MachineComplete();
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

    public bool GetComplete()
    {
        return Complete;
    }

    public void SetGadgetUse(bool b)
    {
        GadgetUse = b;
    }

    public bool GetGadgetUse()
    {
        return GadgetUse;
    }

    public void SetHUD(bool b)
    {
        HUD.SetActive(b);
    }
}
