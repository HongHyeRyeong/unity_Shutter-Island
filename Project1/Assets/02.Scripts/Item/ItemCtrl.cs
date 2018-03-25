using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCtrl : MonoBehaviour
{
    public int ItemType;
    public int ItemLevel;
    bool SurvivorEnter = false;

    private void Start()
    {
        GameObject items;

        items = GameObject.Find("Items");
        this.transform.parent = items.transform;
    }

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
