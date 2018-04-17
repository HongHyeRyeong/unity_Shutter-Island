using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrisonCtrl : MonoBehaviour
{
    private Animator Ani;
    Vector3 SpawnPoint;

    public int PrisonNum;
    bool isOpen = false;

    GameObject[] Survivors = new GameObject[4];
    int SurvivorNum = 0;

    void Start()
    {
        Ani = GetComponent<Animator>();
        SpawnPoint = this.gameObject.transform.Find("PrisonSpawnPoint").transform.position;
    }

    void Update()
    {
        if (SurvivorNum != 0)
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
        if (other.tag == "Survivor" && SurvivorNum != 0)
        {
            other.gameObject.GetComponent<SurvivorCtrl>().PrisonStay(this.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Survivor" && SurvivorNum != 0)
        {
            other.gameObject.GetComponent<SurvivorCtrl>().PrisonExit();
        }
    }

    public void SurvivorEnter(GameObject survivor)
    {
        Survivors[SurvivorNum++] = survivor;
    }

    public void OpenDoor()
    {
        Ani.SetTrigger("Open");
        isOpen = true;

        for (int i = 0; i < SurvivorNum; ++i)
        {
            Survivors[i].transform.position = SpawnPoint;
            Survivors[i].GetComponent<SurvivorCtrl>().PrisonFalse();
            Survivors[i] = null;
        }
        SurvivorNum = 0;

        GameObject.Find("GameController").GetComponent<GameCtrl>().SetPrisons(PrisonNum, false);
    }

    public bool GetOpen()
    {
        return isOpen;
    }
}