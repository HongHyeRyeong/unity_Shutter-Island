using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrisonCtrl : MonoBehaviour
{
    private PhotonView pv = null;

    private Animator Ani;
    private AudioSource Audio;
    private Vector3 SpawnPoint;

    public int PrisonNum;
    private bool isOpen = false;

    private GameObject[] Survivors = new GameObject[4];
    private int SurvivorNum = 0;

    void Start()
    {
        pv = GetComponent<PhotonView>();

        Ani = GetComponent<Animator>();
        SpawnPoint = this.gameObject.transform.Find("PrisonSpawnPoint").transform.position;

        Audio = GetComponent<AudioSource>();
        Audio.Stop();
        SoundManager.instance.SetEffect(false, Audio, "DoorOpen");
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Survivor"))
        {
            SurvivorCtrl ctrl = other.gameObject.GetComponent<SurvivorCtrl>();

            if (SurvivorNum != 0)    // 감옥에 생존자가 있다면
                ctrl.PrisonStay(this.gameObject);
            else
                ctrl.PrisonExit();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Survivor") && SurvivorNum != 0)
            other.gameObject.GetComponent<SurvivorCtrl>().PrisonExit();
    }

    public void SurvivorEnter(GameObject survivor)
    {
        for (int i = 0; i < 4; ++i)
            if (Survivors[i] == null)
            {
                SurvivorNum++;
                Survivors[i] = survivor;
                pv.RPC("RPCSurvivorEnter", PhotonTargets.AllBuffered);
                break;
            }
    }

    [PunRPC]
    void RPCSurvivorEnter()
    {
        if (SurvivorNum == 0)    // 감옥에 갇힌 생존자를 제외한 나머지 플레이어
            StartCoroutine(SurvivorInPrison());

        SurvivorNum++;
        GameCtrl.instance.DisSurPrison(1);
    }

    IEnumerator SurvivorInPrison()
    {
        yield return new WaitForEndOfFrame();

        while (SurvivorNum != 0)
        {
            GameCtrl.instance.DisPrison(transform.position, PrisonNum);
            yield return new WaitForEndOfFrame();
        }
    }

    public void SurvivorExit(GameObject survivor)
    {
        for (int i = 0; i < 4; ++i)
            if (Survivors[i] == survivor)
            {
                Survivors[i] = null;
                break;
            }

        pv.RPC("RPCSurvivorExit", PhotonTargets.AllBuffered);
    }

    [PunRPC]
    void RPCSurvivorExit()
    {
        SurvivorNum--;
        GameCtrl.instance.SetPrisons(PrisonNum, false, 1);
    }

    public void OpenDoor()
    {
        pv.RPC("RPCOpenDoor", PhotonTargets.All);
    }

    [PunRPC]
    void RPCOpenDoor()
    {
        Audio.Play();
        Ani.SetTrigger("Open");
    }

    public void Open()
    {
        isOpen = true;
        GameCtrl.instance.SetPrisons(PrisonNum, false, SurvivorNum);
        SurvivorNum = 0;

        for (int i = 0; i < 4; ++i)
            if (Survivors[i] != null)
            {
                Survivors[i].transform.position = SpawnPoint;
                Survivors[i].GetComponent<SurvivorCtrl>().SetprisonPos(SpawnPoint);
                Survivors[i].GetComponent<SurvivorCtrl>().PrisonFalse();
                Survivors[i] = null;
            }
    }

    public void Close() { isOpen = false; }

    public bool GetOpen() { return isOpen; }
}