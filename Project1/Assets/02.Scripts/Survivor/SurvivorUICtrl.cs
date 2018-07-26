using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SurvivorUICtrl : MonoBehaviour
{
    public static SurvivorUICtrl instance;

    public Text txtFPS;

    public GameObject Message;
    public Text txtMessage;
    public Text txtKeyInput;

    public GameObject Time;
    public Image imgTime;
    public Text txtTime;

    public Image imgLife;
    public Sprite spriteLife1;

    public Image imgHP;
    public Image imgMurHP;
    public Image imgStamina;

    public GameObject Inven;
    public Image imgInvenHat;
    public Image imgInvenCoat;
    public Image imgInvenBag;
    public Sprite[] spriteInvenHat = new Sprite[4];
    public Sprite[] spriteInvenCoat = new Sprite[4];
    public Sprite[] spriteInvenBag = new Sprite[3];

    public GameObject ItemInfor;
    public Text txtItemInfor;

    public Image imgKey;
    public Sprite spriteKeyOn;
    public Sprite spriteKeyOff;

    public Text txtGadget;

    public Image[] imgMachine = new Image[5];

    public RectTransform rtPrison;
    public GameObject[] Prisons = new GameObject[3];
    public Text[] txtPrisons = new Text[3];

    GameObject[] Items = new GameObject[100];
    int itemNum = 0;

    public GameObject HUDItem;
    public Text txtHUDItem;

    private void Start()
    {
        instance = this;

        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Inven
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (Inven.activeSelf)
            {
                Inven.SetActive(false);
                ItemInfor.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                Inven.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
            }
        }

        if (ItemInfor.activeSelf)
        {
            Vector3 pos = Input.mousePosition;
            pos.x += 200;
            pos.y -= 50;

            ItemInfor.transform.position = pos;
        }

        // Item HUD
        if (itemNum > 0)
        {
            for (int i = 0; i < 100; ++i)
                if (Items[i] != null)
                {
                    DispItemHUD(
                        Items[i].transform.position,
                        Items[i].GetComponent<ItemCtrl>().ItemType,
                        Items[i].GetComponent<ItemCtrl>().ItemLevel);
                    GameCtrl.instance.Survivor.GetComponent<SurvivorItem>().SurvivorEnterItem(Items[i]);
                    break;
                }
        }
        else if (itemNum == 0)
        {
            if (HUDItem.activeSelf)
                HUDItem.SetActive(false);
        }
    }

    public void DisFPS(float fps)
    {
        txtFPS.text = fps.ToString("F1");
    }

    public void DisMessage(int type)
    {
        Message.SetActive(true);

        if (type == 1)
        {
            txtKeyInput.text = "T";
            txtMessage.text = "Installing the parts";
        }
        else if (type == 2)
        {
            txtKeyInput.text = "R";
            txtMessage.text = "Repairing devices";
        }
        else if (type == 3)
        {
            txtKeyInput.text = "R";
            txtMessage.text = "Open prison door";
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
    }

    public void DispHP(float hp)
    {
        imgHP.fillAmount = hp / 100;
    }

    public void DispStamina(float stamina, float maxStamina)
    {
        imgStamina.fillAmount = stamina / maxStamina;
    }

    public void OnClickInventory(int type)
    {
        int state = GameCtrl.instance.Survivor.GetComponent<SurvivorCtrl>().GetState();

        if (state == 0 || state == 1 || state == 2)
        {
            GameCtrl.instance.Survivor.GetComponent<SurvivorItem>().ItemPut(type);
            UpdateItemInformation(type);
        }
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
        int level = GameCtrl.instance.Survivor.GetComponent<SurvivorItem>().ItemGet(type);

        if (level == 0)
        {
            txtItemInfor.text = "No Item";

            if (type == 1)
                imgInvenHat.sprite = spriteInvenHat[0];
            else if (type == 2)
                imgInvenCoat.sprite = spriteInvenCoat[0];
            else if (type == 3)
                imgInvenBag.sprite = spriteInvenBag[0];
        }
        else
        {
            if (type == 1)
            {
                txtItemInfor.text = "Hat Level " + level.ToString();
                imgInvenHat.sprite = spriteInvenHat[level];
            }
            else if (type == 2)
            {
                txtItemInfor.text = "Coat Level " + level.ToString();
                imgInvenCoat.sprite = spriteInvenCoat[level];
            }
            else if (type == 3)
            {
                txtItemInfor.text = "Bag Level " + level.ToString();
                imgInvenBag.sprite = spriteInvenBag[level];
            }
        }
    }

    public void DisGadget(int num)
    {
        txtGadget.text = num.ToString();
    }

    public void DisKey(bool b)
    {
        if (b == true)
            imgKey.sprite = spriteKeyOn;
        else
            imgKey.sprite = spriteKeyOff;
    }

    public void DisPrison(Vector3 pos, int num)
    {
        if (!Prisons[num].activeSelf)
            Prisons[num].SetActive(true);

        float dist = Vector3.Distance(pos, GameCtrl.instance.Survivor.transform.position) - 5.5f;

        if (dist < 0)
            dist = 0;

        pos.y += 5;

        Vector3 view = CameraCtrl.instance.MainCam.WorldToViewportPoint(pos);

        if (!(-0.5 < view.x && view.x < 1.5) || view.z < 0)
        {
            Prisons[num].SetActive(false);
            return;
        }

        Vector2 screen = new Vector2(
            ((view.x * rtPrison.sizeDelta.x) - (rtPrison.sizeDelta.x * 0.5f)),
            ((view.y * rtPrison.sizeDelta.y) - (rtPrison.sizeDelta.y * 0.5f)));

        int Minx = Screen.width / 2 - 80;
        int MaXx = Screen.width / 2 - 80;
        int Miny = Screen.height / 2 - 150;
        int MaXy = Screen.height / 2 - 150;

        if (screen.x > Minx)
            screen.x = Minx;
        else if (screen.x < -MaXx)
            screen.x = -MaXx;

        if (screen.y > Miny)
            screen.y = Miny;
        else if (screen.y < -MaXy)
            screen.y = -MaXy;

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

        Vector3 survivorPos = GameCtrl.instance.Survivor.transform.position;

        survivorPos.y = HUDItem.transform.position.y;
        Vector3 vec = survivorPos - HUDItem.transform.position;
        vec.Normalize();

        HUDItem.transform.rotation = Quaternion.LookRotation(vec);

        if (type == 1)
            txtHUDItem.text = "Hat Level " + level.ToString();
        else if (type == 2)
            txtHUDItem.text = "Coat Level " + level.ToString();
        else if (type == 3)
            txtHUDItem.text = "Bag Level " + level.ToString();
        else if (type == 4)
            txtHUDItem.text = "Machine Parts";
        else if (type == 5)
            txtHUDItem.text = "Key";
    }

    public int SurvivorEnterItems(GameObject item)
    {
        for (int i = 0; i < 100; ++i)
            if (Items[i] == null)
            {
                Items[i] = item;
                itemNum++;
                return i;
            }
        return -1;
    }

    public void SurvivorExitItems(int num)
    {
        Items[num] = null;
        itemNum--;
    }

    public void DisMachine(int num)
    {
        for (int i = 0; i < 5; ++i)
        {
            Color c = imgMachine[i].color;

            if (i < num)
                c.a = 1.0f;
            else
                c.a = 0.2f;

            imgMachine[i].color = c;
        }
    }

    public void DisMurHP(float hp)
    {
        imgMurHP.fillAmount = hp / 200;
    }
}