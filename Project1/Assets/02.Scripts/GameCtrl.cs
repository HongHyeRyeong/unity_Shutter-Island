using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCtrl : MonoBehaviour
{
    bool Loading = false;
    int Character;

    GameObject Survivor;
    GameObject Murderer;

    public GameObject Camera;
    public GameObject GameController;
    public GameObject UI;
    public GameObject Fog;

    public Material SkyboxDay;
    public Material SkyboxNight;

    int MachineCompleteNum = 0;
    int PrisonSurNum = 0;

    //
    float deltaTime = 0.0f;
    float fps;

    private void Start()
    {
        PhotonNetwork.isMessageQueueRunning = true;
        Application.targetFrameRate = 60;

        if (CharacterSelect.SelSur == true)
            CreateSurvivor();
        else
            CreateMurderer();

        int map = Random.Range(1, 3);

        if (map == 1)
            Camera.AddComponent<Skybox>().material = SkyboxDay;
        else
            Camera.AddComponent<Skybox>().material = SkyboxDay;

        Loading = true;
    }

    void Update()
    {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        fps = 1.0f / deltaTime;

        DisFPS();
    }

    void DisFPS()
    {
        if (Character == 1)
            GameObject.Find("SurvivorController").GetComponent<SurvivorUICtrl>().DisFPS(fps);
        else if (Character == 2)
            GameObject.Find("MurdererController").GetComponent<MurdererUICtrl>().DisFPS(fps);
    }

    void CreateSurvivor()
    {
        Survivor = PhotonNetwork.Instantiate("Survivor", new Vector3(-37, 0.0f, 36), Quaternion.identity, 0);

        SetGame(1, Survivor, null);
    }

    void CreateMurderer()
    {
        Murderer = PhotonNetwork.Instantiate("Murderer", new Vector3(10, 0.0f, 67), Quaternion.identity, 0);

        SetGame(2, null, Murderer);
    }

    public void SetGame(int character, GameObject survivor, GameObject murderer)
    {
        Character = character;

        GameObject.Find("MainCamera").GetComponent<CameraCtrl>().SetCharacter(Character);

        if (Character == 1)
        {
            Survivor = survivor;

            GameController.transform.Find("SurvivorController").gameObject.SetActive(true);
            GameController.transform.Find("SurvivorController").GetComponent<ItemsCtrl>().SetSurvivor(Survivor);
            GameController.transform.Find("SurvivorController").GetComponent<SurvivorUICtrl>().SetSurvivor(Survivor);
            UI.transform.Find("SurvivorUI").gameObject.SetActive(true);
        }
        else if (Character == 2)
        {
            Murderer = murderer;

            GameController.transform.Find("MurdererController").gameObject.SetActive(true);
            GameController.transform.Find("MurdererController").GetComponent<MurdererUICtrl>().SetMurderer(Murderer);
            UI.transform.Find("MurdererUI").gameObject.SetActive(true);
        }
    }

    public void MachineComplete()
    {
        MachineCompleteNum++;

        if (Character == 1)
            GameObject.Find("SurvivorController").GetComponent<SurvivorUICtrl>().DisMachine(MachineCompleteNum);
        else if (Character == 2)
        {
            print("장치 가동");
            Murderer.GetComponent<MurdererCtrl>().DamageByMachine(40);
            GameObject.Find("MurdererController").GetComponent<MurdererUICtrl>().DisMachine(MachineCompleteNum);
        }
    }

    public void DisPrison(Vector3 pos, int num)
    {
        if (Character == 1)
            GameObject.Find("SurvivorController").GetComponent<SurvivorUICtrl>().DisPrison(pos, num);
        else if (Character == 2)
            GameObject.Find("MurdererController").GetComponent<MurdererUICtrl>().DisPrison(pos, num);
    }

    public void DisSurPrison(int num)
    {
        PrisonSurNum += num;

        if (Character == 2)
            GameObject.Find("MurdererController").GetComponent<MurdererUICtrl>().DisSurPrison(PrisonSurNum);
    }

    public void SetPrisons(int num, bool b, int Surnum)
    {
        if (Character == 1)
            GameObject.Find("SurvivorController").GetComponent<SurvivorUICtrl>().SetPrisons(num, b);
        else if (Character == 2)
        {
            PrisonSurNum -= Surnum;
            GameObject.Find("MurdererController").GetComponent<MurdererUICtrl>().DisSurPrison(PrisonSurNum);
            GameObject.Find("MurdererController").GetComponent<MurdererUICtrl>().SetPrisons(num, b);
        }
    }

    public void DisMurHP(float hp)
    {
        if (Character == 1)
        {
            GameObject.Find("SurvivorController").GetComponent<SurvivorUICtrl>().DisMurHP(hp);

            print("gamectrl " + hp);
        }
    }
}
