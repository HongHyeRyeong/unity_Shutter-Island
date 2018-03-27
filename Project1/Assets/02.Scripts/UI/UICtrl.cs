using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICtrl : MonoBehaviour
{
    public GameObject Inven;
    bool isInven;

    public Image imgStamina;
    public Text txtStamina;
    float maxStamina;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        isInven = false;
        maxStamina = GameObject.Find("Survivor").GetComponent<SurvivorCtrl>().maxStamina;
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

    public void DispStamina(float s)
    {
        txtStamina.text = s.ToString("N2");
        imgStamina.fillAmount = s / maxStamina;
    }
}