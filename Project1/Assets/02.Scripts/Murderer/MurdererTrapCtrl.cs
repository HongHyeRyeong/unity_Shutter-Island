using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

public class MurdererTrapCtrl : Photon.MonoBehaviour
{
    [SerializeField]
    private PhotonView pv = null;
    [SerializeField]
    private Animator Anim;

    [HideInInspector]
    public int SetNum = -1;

    private bool Use = false;              // 트랩이 발동중인지
    private GameObject Survivor = null;    // 트랩에 걸린 생존자

    void Awake()
    {
        // 데이터 전송 타입 설정
        pv.synchronization = ViewSynchronization.UnreliableOnChange;
        // PhotonView Observed Components 속성에 Ctrl 스크립트 연결
        pv.ObservedComponents[0] = this;
    }

    private void OnTriggerStay(Collider other)
    {
        if (Use)
            return;

        if (other.CompareTag("Survivor"))
        {
            int state = other.gameObject.GetComponent<SurvivorCtrl>().GetState();

            if (state == 0 || state == 1 || state == 2)
            {
                Use = true;
                Survivor = other.gameObject;

                GameCtrl.instance.MurdererScore[2] += 200;
                GameCtrl.instance.SetMurdererScore(200);

                Anim.SetTrigger("trCaught");
                other.gameObject.GetComponent<SurvivorCtrl>().TrapOn();

                StartCoroutine(TrapOn());
            }
        }
    }

    IEnumerator TrapOn()
    {
        SurvivorCtrl surCtrl = Survivor.GetComponent<SurvivorCtrl>();

        while (true)
        {
            if (!surCtrl.Trap)
            {
                Survivor = null;
                Anim.SetTrigger("trIdle");
                break;
            }

            yield return null;
        }
    }

    public void TrapDisabled()
    {
        pv.RPC("TrapAcitveFalse", PhotonTargets.AllBuffered);
    }

    [PunRPC]
    public void TrapAcitveFalse()
    {
        Use = false;
        gameObject.SetActive(false);

        if (Survivor == GameCtrl.instance.Survivor)
            GameCtrl.instance.DisTrap(1);
    }
}
