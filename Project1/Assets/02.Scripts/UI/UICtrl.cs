using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICtrl : MonoBehaviour
{
    public GameObject Inven;
    bool isInven = false;

    public GameObject ItemInfor;
    public Text txtItemInfor;

    public GameObject HUDItem;
    public Text txtHUDItem;

    public Image imgLife;
    public Sprite spLife1;
    public Sprite spLife2;

    public Image imgHP;
    public Text txtHP;

    public Image imgStamina;
    public Text txtStamina;
    float maxStamina = 0;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (maxStamina == 0)
            maxStamina = GameObject.Find("Survivor").GetComponent<SurvivorCtrl>().maxStamina;

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (isInven)
            {
                isInven = false;
                Inven.SetActive(false);
                ItemInfor.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                isInven = true;
                Inven.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
            }
        }

        if (ItemInfor.activeSelf)
        {
            Vector3 pos = Input.mousePosition;
            pos.x += 150;
            pos.y -= 50;

            ItemInfor.transform.position = pos;
        }
    }

    public void OnClickInventory(int type)
    {
        GameObject.Find("Survivor").GetComponent<SurvivorItem>().ItemPut(type);
        UpdateItemInformation(type);
    }

    public void OnMouseInventory(int type)
    {
        ItemInfor.SetActive(true);
        UpdateItemInformation(type);
    }

    public void OffMouseInventory()
    {
        ItemInfor.SetActive(false);
    }

    public void UpdateItemInformation(int type)
    {
        int level = GameObject.Find("Survivor").GetComponent<SurvivorItem>().ItemGet(type);

        if (level == 0)
            txtItemInfor.text = "장착된 아이템이 없습니다.";
        else
        {
            if (type == 1)
            {
                if (level == 1)
                    txtItemInfor.text = "모자 1단계(장치 속도 2초 감소)";
                else if (level == 2)
                    txtItemInfor.text = "모자 2단계(장치 속도 3초 감소)";
                else if (level == 3)
                    txtItemInfor.text = "모자 3단계(장치 속도 4초 감소)";
            }
            else if (type == 2)
            {
                if (level == 1)
                    txtItemInfor.text = "옷 1단계(스테미너 50 증가)";
                else if (level == 2)
                    txtItemInfor.text = "옷 2단계(스테미너 100 증가)";
                else if (level == 3)
                    txtItemInfor.text = "옷 3단계(스테미너 200 증가)";
            }
            else if (type == 3)
            {
                if (level == 1)
                    txtItemInfor.text = "가방 1단계(장치 부품 1개 추가 가능)";
                else if (level == 2)
                    txtItemInfor.text = "가방 2단계(장치 부품 2개 추가 가능)";
            }
        }
    }

    public void DispItemHUD(Vector3 pos, int type, int level)
    {
        HUDItem.SetActive(true);
        pos.y += 1;
        HUDItem.transform.position = pos;

        if (type == 1)
        {
            if (level == 1)
                txtHUDItem.text = "모자 1단계";
            else if (level == 2)
                txtHUDItem.text = "모자 2단계";
            else if (level == 3)
                txtHUDItem.text = "모자 3단계";
        }
        else if (type == 2)
        {
            if (level == 1)
                txtHUDItem.text = "옷 1단계";
            else if (level == 2)
                txtHUDItem.text = "옷 2단계";
            else if (level == 3)
                txtHUDItem.text = "옷 3단계";
        }
        else if (type == 3)
        {
            if (level == 1)
                txtHUDItem.text = "가방 1단계";
            else if (level == 2)
                txtHUDItem.text = "가방 2단계";
        }
    }

    public void DispLife(int life)
    {
        if (life == 1)
        {
            imgLife.sprite = spLife1;
        }
        else if (life == 2)
        {
            imgLife.sprite = spLife2;
        }
    }

    public void DispHP(float hp)
    {
        txtHP.text = hp.ToString("N2");
        imgHP.fillAmount = hp / 100;
    }

    public void DispStamina(float stamina)
    {
        txtStamina.text = stamina.ToString("N2");
        imgStamina.fillAmount = stamina / maxStamina;
    }
}