using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MurdererTrapCtrl : MonoBehaviour
{
    private PhotonView pv = null;

    private Animator Anim;

    public int SetNum = -1;

    private bool Use = false;       // 트랩이 발동중인지
    private GameObject Survivor;    // 트랩에 걸린 생존자

    private void Start()
    {
        // PhotonView 컴포넌트 할당
        pv = GetComponent<PhotonView>();
        // 데이터 전송 타입 설정
        pv.synchronization = ViewSynchronization.UnreliableOnChange;

        // PhotonView Observed Components 속성에 Ctrl 스크립트 연결
        pv.ObservedComponents[0] = this;

        Anim = gameObject.GetComponent<Animator>();
        Survivor = null;
    }

    private void OnTriggerStay(Collider other)
    {
        if (Use)
            return;

        if (other.gameObject.tag == "Survivor")
        {
            int state = other.gameObject.GetComponent<SurvivorCtrl>().GetState();

            if (state == 0 || state == 1 || state == 2)
            {
                Use = true;
                Survivor = other.gameObject;

                Anim.SetTrigger("trCaught");
                other.gameObject.GetComponent<SurvivorCtrl>().TrapOn();

                StartCoroutine(TrapOn());
            }
        }
    }

    IEnumerator TrapOn()
    {
        SurvivorCtrl surCtrl = Survivor.GetComponent<SurvivorCtrl>();

        while(true)
        {
            if(!surCtrl.Trap)
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
    }
}
