using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrisonCtrl : MonoBehaviour
{
    private Animator ani;
    public Transform SpawnPoint;

    public int PrisonNum;
    public bool isOpen;

    GameObject[] Survivors = new GameObject[4];
    int SurvivorNum = 1;

    void Start()
    {
        ani = GetComponent<Animator>();
        isOpen = false;
    }

    void Update()
    {
        if (SurvivorNum != 0)
        {
            GameObject.Find("GameController").GetComponent<SurvivorUICtrl>().DisPrison(transform.position, PrisonNum);
        }

        if (isOpen)
        {
            if (ani.GetCurrentAnimatorStateInfo(0).IsName("PrisonOpen") && ani.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f)
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
        if (other.tag == "Survivor")
        {
            if (GameObject.Find("GameController").GetComponent<SurvivorUICtrl>().Message.activeSelf)
                GameObject.Find("GameController").GetComponent<SurvivorUICtrl>().Message.SetActive(false);

            if (GameObject.Find("GameController").GetComponent<SurvivorUICtrl>().Time.activeSelf)
                GameObject.Find("GameController").GetComponent<SurvivorUICtrl>().Time.SetActive(false);
        }
    }

    public void SurvivorEnter(GameObject survivor)
    {
        Survivors[SurvivorNum++] = survivor;
    }

    public void OpenDoor()
    {
        ani.SetTrigger("Open");
        isOpen = true;

        for (int i = 0; i < SurvivorNum; ++i)
        {
            Survivors[i].transform.position = SpawnPoint.position;
            Survivors[i].GetComponent<SurvivorCtrl>().PrisonFalse();
            Survivors[i] = null;
        }

        SurvivorNum = 0;

        GameObject.Find("GameController").GetComponent<SurvivorUICtrl>().Prisons[PrisonNum].SetActive(false);
    }
}