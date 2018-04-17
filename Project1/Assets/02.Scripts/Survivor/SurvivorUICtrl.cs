using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SurvivorUICtrl : MonoBehaviour
{
    GameObject Survivor;

    // ui
    public GameObject Message;
    public Text txtMessage;
    public Text txtKeyInput;

    public GameObject Time;
    public Image imgTime;
    public Text txtTime;

    public Image imgLife;
    public Sprite spriteLife1;
    public Sprite spriteLife2;

    public Image imgHP;
    public Text txtHP;

    public Image imgStamina;
    public Text txtStamina;

    public Image[] imgAttack = new Image[2];
    public Image[] imgAttackR = new Image[2];

    public GameObject Inven;
    bool isInven = false;

    public GameObject ItemInfor;
    public Text txtItemInfor;

    public GameObject Key;
    public Text txtGadget;

    public Text txtMachine;

    public Camera Cam;
    public RectTransform rtPrison;
    public GameObject[] Prisons = new GameObject[3];
    public Text[] txtPrisons = new Text[3];

    // world ui
    public GameObject HUDItem;
    public Text txtHUDItem;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
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

    public void DisMessage(int type)
    {
        Message.SetActive(true);

        if (type == 1)
        {
            txtKeyInput.text = "T";
            txtMessage.text = "장치에 부품 설치하기";
        }
        else if (type == 2)
        {
            txtKeyInput.text = "R";
            txtMessage.text = "장치 고치기";
        }
        else if (type == 3)
        {
            txtKeyInput.text = "R";
            txtMessage.text = "감옥 문 열기";
        }
    }

    public void DisTime(float time, float totalTime)
    {
        Time.SetActive(true);

        txtTime.text = time.ToString("N2");
        imgTime.fillAmount = time / totalTime;
    }

    public void DispLife(int life)
    {
        if (life == 1)
        {
            imgLife.sprite = spriteLife1;
        }
        else if (life == 2)
        {
            imgLife.sprite = spriteLife2;
        }
    }

    public void DispHP(float hp)
    {
        txtHP.text = hp.ToString("N2");
        imgHP.fillAmount = hp / 100;
    }

    public void DispStamina(float stamina, float maxStamina)
    {
        txtStamina.text = stamina.ToString("N2");
        imgStamina.fillAmount = stamina / maxStamina;
    }

    public void DisAttackBack(int i, bool attack)
    {
        Color c = imgAttack[i].color;

        if (attack)
            c.a = 0.2f;
        else
            c.a = 0.8f;

        imgAttack[i].color = c;
        imgAttackR[i].color = c;
    }

    public void OnClickInventory(int type)
    {
        Survivor.GetComponent<SurvivorItem>().ItemPut(type);
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
        int level = Survivor.GetComponent<SurvivorItem>().ItemGet(type);

        if (level == 0)
            txtItemInfor.text = "장착된 아이템이 없습니다.";
        else
        {
            if (type == 1)
            {
                if (level == 1)
                    txtItemInfor.text = "모자 1단계";
                else if (level == 2)
                    txtItemInfor.text = "모자 2단계";
                else if (level == 3)
                    txtItemInfor.text = "모자 3단계";
            }
            else if (type == 2)
            {
                if (level == 1)
                    txtItemInfor.text = "옷 1단계";
                else if (level == 2)
                    txtItemInfor.text = "옷 2단계";
                else if (level == 3)
                    txtItemInfor.text = "옷 3단계";
            }
            else if (type == 3)
            {
                if (level == 1)
                    txtItemInfor.text = "가방 1단계";
                else if (level == 2)
                    txtItemInfor.text = "가방 2단계";
            }
        }
    }

    public void DisGadget(int num)
    {
        txtGadget.text = num.ToString();
    }

    public void DisKey(bool b)
    {
        Key.SetActive(b);
    }

    public void DisMachine(int num)
    {
        txtMachine.text = num.ToString();
    }

    public void DisPrison(Vector3 pos, int num)
    {
        if (!Prisons[num].activeSelf)
            Prisons[num].SetActive(true);

        float dist = Vector3.Distance(pos, Survivor.transform.position) - 5.5f;

        if (dist < 0)
            dist = 0;

        pos.y += 5;

        Vector3 view = Cam.WorldToViewportPoint(pos);

        if (!(-0.5 < view.x && view.x < 1.5) || view.z < 0)
        {
            Prisons[num].SetActive(false);
            return;
        }

        Vector2 screen = new Vector2(
            ((view.x * rtPrison.sizeDelta.x) - (rtPrison.sizeDelta.x * 0.5f)),
            ((view.y * rtPrison.sizeDelta.y) - (rtPrison.sizeDelta.y * 0.5f)));

        if (screen.x > 850)
            screen.x = 850;
        else if (screen.x < -850)
            screen.x = -850;

        if (screen.y > 350)
            screen.y = 350;
        else if (screen.y < -350)
            screen.y = -350;

        Prisons[num].GetComponent<RectTransform>().anchoredPosition = screen;
        txtPrisons[num].text = dist.ToString("N1") + " m";
    }

    public void SetPrisons(int num, bool b)
    {
        Prisons[num].SetActive(b);
    }

    public void DispItemHUD(Vector3 pos, int type, int level)
    {
        HUDItem.SetActive(true);
        pos.y += 2.5f;
        HUDItem.transform.position = pos;

        Vector3 survivorPos = Survivor.transform.position;

        survivorPos.y = HUDItem.transform.position.y;
        Vector3 vec = survivorPos - HUDItem.transform.position;
        vec.Normalize();

        HUDItem.transform.rotation = Quaternion.LookRotation(vec);

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
        else if (type == 4)
        {
            txtHUDItem.text = "장치 부품";
        }
        else if (type == 5)
        {
            txtHUDItem.text = "열쇠";
        }
    }

    public void SetSurvivor(GameObject survivor)
    {
        Survivor = survivor;
    }
}