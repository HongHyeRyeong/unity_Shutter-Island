using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MachineCtrl : MonoBehaviour
{
    private PhotonView pv = null;
    private Animator Ani;
    private AudioSource Audio;

    private GameObject HUD;
    private Image imgHUD;
    private Text txtHUD;
    private GameObject CompleteLight;
    private GameObject Flare;

    private bool GadgetUse = false;     // 부품 설치 됐는지
    private int GadgetNum = 0;          // 설치한 부품 수
    private float MachineGauge = 0;     // 기계 게이지
    private bool Complete = false;      // 모든 설치가 완료 됐는 지

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
        pv.RPC("MachineInstall", PhotonTargets.AllBuffered, work);

        if (MachineGauge >= 10 + 10 * GadgetNum)
        {
            pv.RPC("MachineOneComplete", PhotonTargets.AllBuffered);

            if (MachineGauge >= 20f)    // 50
                pv.RPC("MachineComplete", PhotonTargets.AllBuffered);
            else
                pv.RPC("MachineInstallAnim", PhotonTargets.AllBuffered);

            return true;
        }
        return false;
    }

    [PunRPC]
    public void MachineInstall(float work)
    {
        MachineGauge += work;

        if (!Audio.isPlaying)
            Audio.Play();
    }

    [PunRPC]
    public void MachineOneComplete()
    {
        if(GadgetUse)
        {
            GadgetUse = false;
            GadgetNum++;
        }

        MachineGauge = 10 * GadgetNum;  // 10단위로 끊어지게
    }

    [PunRPC]
    public void MachineComplete()
    {
        Complete = true;
        GadgetUse = false;

        Ani.SetTrigger("Complete");
        SoundManager.instance.SetEffect(true, Audio, "MachineComplete");
        Audio.Play();

        HUD.SetActive(false);
        Flare.SetActive(false);
        CompleteLight.SetActive(true);

        GameCtrl.instance.MachineComplete();
    }

    [PunRPC]
    public void MachineInstallAnim()
    {
        Ani.SetTrigger("Install");
    }

    public void MachineStop()
    {
        pv.RPC("RPCMachineStop", PhotonTargets.AllBuffered);
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

    public bool GetGadgetUse() { return GadgetUse; }
    public void SetGadgetUse(bool use) { pv.RPC("GadgetUseSet", PhotonTargets.AllBuffered, use); }
    [PunRPC]
    void GadgetUseSet(bool use) { GadgetUse = use; }

    public bool GetComplete() { return Complete; }
    public void SetHUD(bool b) { HUD.SetActive(b); }
}