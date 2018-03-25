using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICtrl : MonoBehaviour
{
    public GameObject Inven;
    public bool isInven;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        isInven = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (isInven)
            {
                isInven = false;
                Inven.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                isInven = true;
                Inven.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
            }
        }
    }

    public void OnClickInventory(int type)
    {
        GameObject.Find("Survivor").GetComponent<SurvivorItem>().ItemPut(type);
    }
}