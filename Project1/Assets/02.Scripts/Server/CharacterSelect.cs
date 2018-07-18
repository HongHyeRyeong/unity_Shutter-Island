using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelect : MonoBehaviour {

    public static CharacterSelect instance;

    public GameObject survivor;
    public GameObject murderer;

    public GameObject Surbtn;

    public GameObject SurStamina;
    public GameObject SurQuick;
    public GameObject SurDamage;

    public GameObject RoomPanel;
    public GameObject MapBtn;

    public GameObject Map1Btn;
    public GameObject Map2Btn;
    public GameObject MapRandBtn;

    public GameObject SurJoin;
    public GameObject MurJoin;

    //
    public bool SelSur = false;
    public int SurStat = 0;
    public int Map = 0;

    //
    void Start()
    {
        instance = this;
    }

    //
	public void OnClickSurvivor()
    {
        SelSur = true;
        Map = 0;

        Map1Btn.GetComponent<Image>().color = new Color(255, 255, 255);
        Map2Btn.GetComponent<Image>().color = new Color(255, 255, 255);
        MapRandBtn.GetComponent<Image>().color = new Color(255, 255, 255);

        survivor.gameObject.SetActive(SelSur);
        Surbtn.gameObject.SetActive(SelSur);
        RoomPanel.SetActive(SelSur);
        SurJoin.SetActive(SelSur);

        murderer.gameObject.SetActive(!SelSur);
        MapBtn.SetActive(!SelSur);
        MurJoin.SetActive(!SelSur);
    }

    public void OnClickMurderer()
    {
        SelSur = false;
        SurStat = 0;

        SurStamina.GetComponent<Image>().color = new Color(255, 255, 255);
        SurQuick.GetComponent<Image>().color = new Color(255, 255, 255);
        SurDamage.GetComponent<Image>().color = new Color(255, 255, 255);

        survivor.gameObject.SetActive(SelSur);
        Surbtn.gameObject.SetActive(SelSur);
        RoomPanel.SetActive(SelSur);
        SurJoin.SetActive(SelSur);

        murderer.gameObject.SetActive(!SelSur);
        MapBtn.SetActive(!SelSur);
        MurJoin.SetActive(!SelSur);
    }

    //
    public void OnClickSurStamina()
    {
        SurStat = 1;

        SurStamina.GetComponent<Image>().color = new Color(0, 125, 125);
        SurQuick.GetComponent<Image>().color = new Color(255, 255, 255);
        SurDamage.GetComponent<Image>().color = new Color(255, 255, 255);
    }

    public void OnClickSurQuick()
    {
        SurStat = 2;

        SurStamina.GetComponent<Image>().color = new Color(255, 255, 255);
        SurQuick.GetComponent<Image>().color = new Color(0, 125, 125);
        SurDamage.GetComponent<Image>().color = new Color(255, 255, 255);
    }

    public void OnClickSurDamage()
    {
        SurStat = 3;

        SurStamina.GetComponent<Image>().color = new Color(255, 255, 255);
        SurQuick.GetComponent<Image>().color = new Color(255, 255, 255);
        SurDamage.GetComponent<Image>().color = new Color(0, 125, 125);
    }

    //
    public void OnClickMap1()
    {
        Map = 1;

        Map1Btn.GetComponent<Image>().color = new Color(0, 125, 125);
        Map2Btn.GetComponent<Image>().color = new Color(255, 255, 255);
        MapRandBtn.GetComponent<Image>().color = new Color(255, 255, 255);
    }

    public void OnClickMap2()
    {
        Map = 2;

        Map1Btn.GetComponent<Image>().color = new Color(255, 255, 255);
        Map2Btn.GetComponent<Image>().color = new Color(0, 125, 125);
        MapRandBtn.GetComponent<Image>().color = new Color(255, 255, 255);
    }

    public void OnClickMapRand()
    {
        Map = 3;

        Map1Btn.GetComponent<Image>().color = new Color(255, 255, 255);
        Map2Btn.GetComponent<Image>().color = new Color(255, 255, 255);
        MapRandBtn.GetComponent<Image>().color = new Color(0, 125, 125);
    }
}
