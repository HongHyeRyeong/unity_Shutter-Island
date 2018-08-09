using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MurdererUICtrl : MonoBehaviour
{
    public static MurdererUICtrl instance;

    public Text txtFPS;

    public Image imgLife;
    public Sprite spriteLife1;

    public Image imgHP;

    public Text txtSurPrisonNum;
    public Text txtTrapNum;

    public Image[] imgMachine = new Image[5];

    public RectTransform rtPrison;
    public GameObject[] Prisons = new GameObject[3];
    public Text[] txtPrisons = new Text[3];

    private void Awake()
    {
        instance = this;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void DisFPS(float fps)
    {
        txtFPS.text = fps.ToString("F1");
    }

    void DispLife(int life)
    {
        if (life == 1)
        {
            imgLife.sprite = spriteLife1;
        }
    }

    public void DispHP(float hp)
    {
        imgHP.fillAmount = hp / 200;

        if (hp < 100 && imgLife.sprite != spriteLife1)
            DispLife(1);
    }

    public void DisSurPrison(int num)
    {
        txtSurPrisonNum.text = num.ToString();
    }

    public void DisTrap(int num)
    {
        txtTrapNum.text = num.ToString();
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

    public void DisPrison(Vector3 pos, int num)
    {
        if (!Prisons[num].activeSelf)
            Prisons[num].SetActive(true);

        float dist = Vector3.Distance(pos, GameCtrl.instance.Murderer.transform.position) - 5.5f;

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
        int MaXy = Screen.height / 2 - 250;

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
}
