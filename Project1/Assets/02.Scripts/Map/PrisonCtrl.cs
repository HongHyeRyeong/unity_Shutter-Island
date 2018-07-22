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
                if (!isSurvivor)
                    StartCoroutine(SurvivorInPrison());
                break;
            }

        GameObject.Find("GameController").GetComponent<GameCtrl>().DisSurPrison(1);
    }

    IEnumerator SurvivorInPrison()
    {
        isSurvivor = true;

        while (true)
        {
            GameObject.Find("GameController").GetComponent<GameCtrl>().DisPrison(transform.position, PrisonNum);
            yield return new WaitForEndOfFrame();
        }
    }

    public void SurvivorExit(GameObject survivor)
    {
        curSur = survivor;
        pv.RPC("RPCSurvivorExit", PhotonTargets.All);
    }

    [PunRPC]
    void RPCSurvivorExit()
    {
        for (int i = 0; i < 4; ++i)
            if (Survivors[i] == curSur)
            {
                Survivors[i] = null;
                break;
            }

        int num = 0;
        for (int i = 0; i < 4; ++i)
            if (Survivors[i] != null)
                num++;

        if (num == 0)
            isSurvivor = false;

        GameObject.Find("GameController").GetComponent<GameCtrl>().SetPrisons(PrisonNum, false, 1);
    }

    public void OpenDoor()
    {
        pv.RPC("RPCOpenDoor", PhotonTargets.All);
    }

    [PunRPC]
    void RPCOpenDoor()
    {
        Ani.SetTrigger("Open");
    }

    public void Open()
    {
        pv.RPC("RPCOpen", PhotonTargets.All);
    }

    [PunRPC]
    void RPCOpen()
    {
        isOpen = true;

        int num = 0;
        for (int i = 0; i < 4; ++i)
            if (Survivors[i] != null)
                num++;

        GameObject.Find("GameController").GetComponent<GameCtrl>().SetPrisons(PrisonNum, false, num);

        for (int i = 0; i < 4; ++i)
        {
            if (Survivors[i] != null)
            {
                Survivors[i].transform.position = SpawnPoint;
                Survivors[i].GetComponent<SurvivorCtrl>().PrisonFalse();
                Survivors[i] = null;
            }
        }
        isSurvivor = false;
    }

    public void Close()
    {
        pv.RPC("RPCClose", PhotonTargets.All);
    }

    [PunRPC]
    void RPCClose()
    {
        isOpen = false;
    }

    public bool GetOpen() { return isOpen; }
}