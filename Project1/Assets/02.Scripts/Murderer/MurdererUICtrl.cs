using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MurdererUICtrl : MonoBehaviour
{
    GameObject Murderer;

    public Image imgHP;
    public Text txtHP;

    public Image[] imgAttack = new Image[2];
    public Image[] imgAttackR = new Image[2];

    public Text txtMachine;

    public Camera Cam;
    public RectTransform rtPrison;
    public GameObject[] Prisons = new GameObject[3];
    public Text[] txtPrisons = new Text[3];

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void DispHP(float hp)
    {
        txtHP.text = hp.ToString("N2");
        imgHP.fillAmount = hp / 100;
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

    public void DisMachine(int num)
    {
        txtMachine.text = num.ToString();
    }

    public void DisPrison(Vector3 pos, int num)
    {
        if (!Prisons[num].activeSelf)
            Prisons[num].SetActive(true);

        float dist = Vector3.Distance(pos, Murderer.transform.position) - 5.5f;

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

    public void SetMurderer(GameObject murderer)
    {
        Murderer = murderer;
    }
}
