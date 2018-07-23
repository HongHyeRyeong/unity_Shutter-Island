using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelect : MonoBehaviour
{
    public static CharacterSelect instance;

    public GameObject survivor;
    public GameObject murderer;

    public GameObject Surbtn;

    public GameObject RoomPanel;
    public GameObject MapBtn;

    public GameObject SurJoin;
    public GameObject MurJoin;

    //
    public bool SelSur = false;
    public int SurStat = -1;
    public int Map = -1;

    void Start()
    {
        instance = this;
    }

    public void OnClickSurvivor()
    {
        SelSur = true;
        Map = -1;

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
        SurStat = -1;

        survivor.gameObject.SetActive(SelSur);
        Surbtn.gameObject.SetActive(SelSur);
        RoomPanel.SetActive(SelSur);
        SurJoin.SetActive(SelSur);

        murderer.gameObject.SetActive(!SelSur);
        MapBtn.SetActive(!SelSur);
        MurJoin.SetActive(!SelSur);
    }

    public void OnClickSurvivor(int i)
    {
        SurStat = i;
    }

    public void OnClickMap(int i)
    {
        Map = i;
    }
}
