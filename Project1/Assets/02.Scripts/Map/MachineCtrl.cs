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
    private AudioSource Audio;

    private GameObject HUD;
    private Image imgHUD;
    private Text txtHUD;
    private GameObject CompleteLight;
    private  GameObject Flare;

    public int MachineNum;
    private float MachineGauge = 0;
    private  bool Complete = false;

    private bool GadgetUse = false;
    private float GadgetGauge = 10f;
    private int GadgetNum = 0;

    void Start()
    {
        pv = GetComponent<PhotonView>();

        Ani = GetComponent<Animator>();

        Audio = GetComponent<AudioSource>();
        Audio.Stop();
        SoundManager.instance.SetEffect(false, Audio, "Install");

        HUD = transform.Find("HUDMachine").gameObject;
        imgHUD = HUD.transform.Find("imgHUDMachine").GetComponent<Image>();
        txtHUD = HUD.transform.Find("txtHUDMachine").GetComponent<Text>();
        CompleteLight = transform.Find("Completelight").gameObject;
        Flare = transform.Find("Flare").gameObject;
    }

    public bool Install(float work)
    {
        currWork = work;
        pv.RPC("MachineInstall", PhotonTargets.AllBuffered);

        if (GadgetGauge < 0)
        {
            pv.RPC("MachineOneComplete", PhotonTargets.All);

            if (MachineGauge >= 10f)    // Demo
            {
                pv.RPC("MachineComplete", PhotonTargets.All);
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

        if (!Audio.isPlaying)
            Audio.Play();
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
        Flare.SetActive(false);

        SoundManager.instance.SetEffect(true, Audio, "MachineComplete");
        Audio.Play();

        Ani.SetTrigger("Complete");
        GameCtrl.instance.MachineComplete();
    }

    [PunRPC]
    public void MachineInstallAnim()
    {
        Ani.SetTrigger("Install");
    }

    public void MachineStop()
    {
        pv.RPC("RPCMachineStop", PhotonTargets.All);
    }

    [PunRPC]
    void RPCMachineStop()
    {
        Audio.Stop();
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
