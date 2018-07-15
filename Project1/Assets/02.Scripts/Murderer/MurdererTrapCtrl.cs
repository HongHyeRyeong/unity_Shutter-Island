using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MurdererTrapCtrl : MonoBehaviour
{
    private Animator Anim;

    public int SetNum = -1;

    private bool Use = false;       // 트랩이 발동중인지
    private GameObject Survivor;    // 트랩에 걸린 생존자

    private void Start()
    {
        Anim = gameObject.GetComponent<Animator>();
        Survivor = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (Use)
            return;

        if (other.gameObject.tag == "Survivor")
        {
            print("dd");
            Use = true;
            Survivor = other.gameObject;

            Anim.SetTrigger("trCaught");
            other.gameObject.GetComponent<SurvivorCtrl>().TrapOn();

            StartCoroutine(TrapOn());
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
        Use = false;
        gameObject.SetActive(false);
    }
}
