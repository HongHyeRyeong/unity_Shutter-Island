using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrisonCtrl : MonoBehaviour
{
    private PhotonView pv = null;
    GameObject curSur;

    private Animator Ani;
    Vector3 SpawnPoint;

    public int PrisonNum;
    bool isOpen = false;

    GameObject[] Survivors = new GameObject[4];
    bool isSurvivor = false;

    void Start()
    {
        pv = GetComponent<PhotonView>();
        Ani = GetComponent<Animator>();
        SpawnPoint = this.gameObject.transform.Find("PrisonSpawnPoint").transform.position;
    }

    void Update()
    {
        if (isSurvivor)
            GameObject.Find("GameController").GetComponent<GameCtrl>().DisPrison(transform.position, PrisonNum);

        if (isOpen)
        {
            if (Ani.GetCurrentAnimatorStateInfo(0).IsName("PrisonOpen") &&
                Ani.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f)
                isOpen = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Survivor" && isSurvivor)
        {
            other.gameObject.GetComponent<SurvivorCtrl>().PrisonStay(this.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Survivor" && isSurvivor)
        {
            other.gameObject.GetComponent<SurvivorCtrl>().PrisonExit();
        }
    }

    public void SurvivorEnter(GameObject survivor)
    {
        for (int i = 0; i < 4; ++i)
            if (Survivors[i] == null)
            {
                Survivors[i] = survivor;
                isSurvivor = true;
                break;
            }
        GameObject.Find("GameController").GetComponent<GameCtrl>().DisSurPrison(1);
    }

    public void SurvivorExit(GameObject survivor)
    {
        curSur = survivor;
        pv.RPC("RPCSurvivorExit", PhotonTargets.All);
    }

    [PunRPC]
    public void RPCSurvivorExit()
    {
        for (int i = 0; i < 4; ++i)
            if (Survivors[i] == curSur)
            {
                Survivors[i] = null;
                break;
            }

        int num = GetSurvivorNum();

        if (num == 0)
            isSurvivor = false;

        GameObject.Find("GameController").GetComponent<GameCtrl>().SetPrisons(PrisonNum, false, 1);
    }

    public void OpenDoor()
    {
        pv.RPC("RPCOpenDoor", PhotonTargets.All);
    }

    [PunRPC]
    public void RPCOpenDoor()
    {
        Ani.SetTrigger("Open");
        isOpen = true;
        GameObject.Find("GameController").GetComponent<GameCtrl>().SetPrisons(PrisonNum, false, GetSurvivorNum());

        for (int i = 0; i < 4; ++i)
        {
            if (Survivors[i] != null)
            {
                Survivors[i].transform.position = SpawnPoint;
                Survivors[i].GetComponent<DemoSurvivorCtrl>().PrisonFalse();    // Demo
                Survivors[i] = null;
            }
        }
        isSurvivor = false;
    }

    public bool GetOpen()
    {
        return isOpen;
    }

    int GetSurvivorNum()
    {
        int num = 0;

        for (int i = 0; i < 4; ++i)
            if (Survivors[i] != null)
                num++;

        return num;
    }
}