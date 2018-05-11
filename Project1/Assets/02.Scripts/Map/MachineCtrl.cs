using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MachineCtrl : MonoBehaviour
{
    private PhotonView pv = null;

    float currWork = 0;
    bool currGadgetUse = false;

    //
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
        pv = GetComponent<PhotonView>();

        Ani = GetComponent<Animator>();
        HUD = this.gameObject.transform.Find("HUDMachine").gameObject;
        imgHUD = HUD.transform.Find("imgHUDMachine").GetComponent<Image>();
        txtHUD = HUD.transform.Find("txtHUDMachine").GetComponent<Text>();
        CompleteLight = this.gameObject.transform.Find("Completelight").gameObject;
    }

    public bool Install(float work)
    {
        currWork = work;
        pv.RPC("MachineInstall", PhotonTargets.AllBuffered);

        if (GadgetGauge < 0)
        {
            pv.RPC("MachineOneComplete", PhotonTargets.All);

            if (MachineGauge == 20f)    // Demo
            {
                pv.RPC("MachineComplete", PhotonTargets.All);
                GameObject.Find("GameController").GetComponent<GameCtrl>().MachineComplete();
            }
            else
                pv.RPC("MachineInstallAnim", PhotonTargets.All);

            return true;
        }

        return false;
    }

    [PunRPC]
    public void MachineInstall()
    {
        MachineGauge += currWork;
        GadgetGauge -= currWork;
    }

    [PunRPC]
    public void MachineInstallAnim()
    {
        Ani.SetTrigger("Install");
    }

    [PunRPC]
    public void MachineOneComplete()
    {
        GadgetUse = false;
        GadgetGauge = 10f;
        GadgetNum++;

        MachineGauge = 10 * GadgetNum;
    }

    [PunRPC]
    public void MachineComplete()
    {
        Complete = true;
        HUD.SetActive(false);
        CompleteLight.SetActive(true);

        Ani.SetTrigger("Complete");
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
        currGadgetUse = b;
        pv.RPC("GadgetUseSet", PhotonTargets.All);
    }

    [PunRPC]
    public void GadgetUseSet()
    {
        GadgetUse = currGadgetUse;
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
