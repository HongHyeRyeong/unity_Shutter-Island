using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCtrl : MonoBehaviour
{
    public int ItemType;    // 1: 모자 2: 옷 3: 가방
    public int ItemLevel;
    bool SurvivorEnter = false;

    void Update()
    {
        if (SurvivorEnter)
            GameObject.Find("Survivor").GetComponent<SurvivorItem>().SurvivorEnterItem(this.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Survivor")
        {
            SurvivorEnter = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Survivor")
        {
            SurvivorEnter = false;
        }
    }
}
