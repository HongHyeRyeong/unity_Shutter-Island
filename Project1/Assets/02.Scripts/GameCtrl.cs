using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameCtrl : MonoBehaviour
{
    public static GameCtrl instance;

    // inGame
    [HideInInspector]
    public bool isStart = false;
    [HideInInspector]
    public int Character;
    
    private int MachineCompleteNum = 0;
    public GameObject[] Prisons = new GameObject[3];
    private int PrisonSurNum = 0;

    // 1이면 생존자 승리, 2이면 생존자 패배, 3이면 살인마 승리, 4이면 살인마 패배
    [HideInInspector]
    public int Result = 0;

    // Survivor
    [HideInInspector]
    public GameObject Survivor;
    [SerializeField]
    private GameObject SurSpawns;
    public GameObject SurvivorUI;
    public Material[] Msurvivor = new Material[3];
    public Material[] MsurvivorItem = new Material[3];

    public GameObject SurFootPrints;
    [SerializeField]
    private GameObject FootPrint;
    private GameObject[] FootPrints = new GameObject[3000];
    private int FootPrintsNum = -1;
    private float delay = 0.05f;
    private float savedelay;

    // Murderer
    [HideInInspector]
    public GameObject Murderer;
    public GameObject MurdererUI;

    public GameObject MurdererTrap;
    private int TrapNum = 5;

    [HideInInspector]
    public int SurvivorNum = 0;
    [HideInInspector]
    public int AllSurNum = 0;

    // item
    [SerializeField]
    private GameObject ItemSpawns;
    private int[] hat = new int[] { 10, 4, 2 };
    private int[] Clothes = new int[] { 10, 4, 2 };
    private int[] Bag = new int[] { 10, 4 };
    private int GadgetNum = 35;
    private int keyNum = 10;

    // score
    [HideInInspector]
    public int[] SurvivorScore = new int[5];
    [HideInInspector]
    public int[] MurdererScore = new int[5];

    [SerializeField]
    private GameObject Fade;
    [SerializeField]
    private Image Hit;

    private void Awake()
    {
        instance = this;

        PhotonNetwork.isMessageQueueRunning = true;
        Application.targetFrameRate = 60;

        if (LobbyCtrl.instance.SelSur == true)
        {
            Character = 1;
            SurvivorUI.SetActive(true);

            // 랜덤 스폰
            Transform[] spawns = SurSpawns.GetComponentsInChildren<Transform>();

            // 맵에 따라 다른 생성 위치
            if (PhotonInit.instance.Map == 1)
                Survivor = PhotonNetwork.Instantiate("Survivor", spawns[Random.Range(1, spawns.Length)].position, Quaternion.identity, 0);
            else
                Survivor = PhotonNetwork.Instantiate("Survivor", spawns[Random.Range(1, spawns.Length)].position, Quaternion.identity, 0);

            StartCoroutine(FindMurderer());
        }
        else
        {
            Character = 2;
            MurdererUI.SetActive(true);
            MurdererUICtrl.instance.DisTrap(TrapNum);

            // 맵에 따라 다른 생성 위치
            if (PhotonInit.instance.Map == 1)
                Murderer = PhotonNetwork.Instantiate("Murderer", new Vector3(3.5f, 5.5f, 83), Quaternion.Euler(0, 180, 0), 0);
            else
                Murderer = PhotonNetwork.Instantiate("Murderer", new Vector3(36, 0.0f, 51), Quaternion.Euler(0, 180, 0), 0);

            // 아이템 생성
            Transform[] spawns = ItemSpawns.GetComponentsInChildren<Transform>();

            int hatNum = hat[0] + hat[1] + hat[2];
            int clothesNum = Clothes[0] + Clothes[1] + Clothes[2];
            int bagNum = Bag[0] + Bag[1];
            int itemnum = hatNum + clothesNum + bagNum;

            for (int i = 0; i < itemnum; ++i)
            {
                Vector3 pos = new Vector3(
                    spawns[PlayerPrefs.GetInt("itemrand" + (i + 1)) + 1].position.x + PlayerPrefs.GetFloat("random" + (i + 1)),
                    spawns[PlayerPrefs.GetInt("itemrand" + (i + 1)) + 1].position.y,
                    spawns[PlayerPrefs.GetInt("itemrand" + (i + 1)) + 1].position.z + PlayerPrefs.GetFloat("random" + (i + 47)));

                if (i < hatNum)
                {
                    if (i < hat[0])
                        PhotonNetwork.Instantiate("ItemHat1", pos, Quaternion.Euler(-90, 0, 0), 0);
                    else if (hat[0] <= i && i < hat[0] + hat[1])
                        PhotonNetwork.Instantiate("ItemHat2", pos, Quaternion.Euler(-90, 0, 0), 0);
                    else
                        PhotonNetwork.Instantiate("ItemHat3", pos, Quaternion.Euler(-90, 0, 0), 0);
                }
                else if (hatNum <= i && i < hatNum + clothesNum)
                {
                    if (i < hatNum + Clothes[0])
                        PhotonNetwork.Instantiate("ItemClothes1", pos, Quaternion.identity, 0);
                    else if (hatNum + Clothes[0] <= i && i < hatNum + Clothes[0] + Clothes[1])
                        PhotonNetwork.Instantiate("ItemClothes2", pos, Quaternion.identity, 0);
                    else
                        PhotonNetwork.Instantiate("ItemClothes3", pos, Quaternion.identity, 0);
                }
                else
                {
                    if (i < hatNum + clothesNum + Bag[0])
                        PhotonNetwork.Instantiate("ItemBag1", pos, Quaternion.identity, 0);
                    else
                        PhotonNetwork.Instantiate("ItemBag2", pos, Quaternion.identity, 0);
                }
            }

            for (int i = 0; i < GadgetNum; ++i)
            {
                Vector3 pos = new Vector3(
                    spawns[PlayerPrefs.GetInt("gadgetrand" + (i + 47)) + 1].position.x + PlayerPrefs.GetFloat("random" + (i + 93)),
                    spawns[PlayerPrefs.GetInt("gadgetrand" + (i + 47)) + 1].position.y,
                    spawns[PlayerPrefs.GetInt("gadgetrand" + (i + 47)) + 1].position.z + PlayerPrefs.GetFloat("random" + (i + 128)));

                PhotonNetwork.Instantiate("ItemGadget", pos, Quaternion.Euler(-90, 0, 0), 0);
            }

            for (int i = 0; i < keyNum; ++i)
            {
                Vector3 pos = new Vector3(
                    spawns[PlayerPrefs.GetInt("keyrand" + (i + 82)) + 1].position.x + PlayerPrefs.GetFloat("random1" + (i + 163)),
                    spawns[PlayerPrefs.GetInt("keyrand" + (i + 82)) + 1].position.y,
                    spawns[PlayerPrefs.GetInt("keyrand" + (i + 82)) + 1].position.z + PlayerPrefs.GetFloat("random2" + (i + 173)));

                PhotonNetwork.Instantiate("ItemKey", pos, Quaternion.identity, 0);
            }
        }

        savedelay = delay;
        StartCoroutine(StartFade(true));

        GetConnectPlayerCount();

        //StartCoroutine(DisFPS());
    }

    IEnumerator FindMurderer()
    {
        while (Murderer == null)
        {
            if (GameObject.Find("Murderer(Clone)") != null)
                Murderer = GameObject.Find("Murderer(Clone)");

            yield return null;
        }
    }

    void GetConnectPlayerCount()
    {
        Room currRoom = PhotonNetwork.room;
    }

    void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        AllSurNum += 1;
        SurvivorNum += 1;

        GetConnectPlayerCount();
    }

    void OnPhotonPlayerDisconnected(PhotonPlayer outPlayer)
    {
        SurvivorNum -= 1;

        if (Character == 2 && PlayerPrefs.GetInt("Result") == 4)
        {
            //
        }
        else if (Character == 2 && AllSurNum > 0 && SurvivorNum == 0)
        {
            Murderer.GetComponent<MurdererCtrl>().MurdererWin();
        }

        GetConnectPlayerCount();
    }

    void OnMasterClientSwitched()
    {
        PlayerPrefs.SetInt("Result", 1);
        StartCoroutine(GameCtrl.instance.StartFade(false));
    }

    public void MachineComplete()
    {
        MachineCompleteNum++;

        if (Character == 1)
            SurvivorUICtrl.instance.DisMachine(MachineCompleteNum);
        else if (Character == 2)
            MurdererUICtrl.instance.DisMachine(MachineCompleteNum);

        Murderer.GetComponent<MurdererCtrl>().DamageByMachine(40);
    }

    public void DisPrison(Vector3 pos, int num)
    {
        if (Character == 1 && Survivor != null)
            SurvivorUICtrl.instance.DisPrison(pos, num);
        else if (Character == 2 && Murderer != null)
            MurdererUICtrl.instance.DisPrison(pos, num);
    }

    public void DisSurPrison(int num)
    {
        PrisonSurNum += num;

        if (Character == 2)
            MurdererUICtrl.instance.DisSurPrison(PrisonSurNum);
    }

    public void DisTrap(int num)
    {
        if (Character == 2)
        {
            TrapNum += num;
            MurdererUICtrl.instance.DisTrap(TrapNum);
        }
    }

    public void SetPrisons(int num, bool b, int Surnum)
    {
        if (Character == 1)
            SurvivorUICtrl.instance.SetPrisons(num, b);
        else if (Character == 2)
        {
            PrisonSurNum -= Surnum;
            MurdererUICtrl.instance.DisSurPrison(PrisonSurNum);
            MurdererUICtrl.instance.SetPrisons(num, b);
        }
    }

    public void DisMurHP(float hp)
    {
        if (Character == 1)
            SurvivorUICtrl.instance.DisMurHP(hp);
    }

    public void UseFootPrint(Vector3 SurPos)
    {
        if (delay != savedelay)
            return;
        StartCoroutine(DelayTime());

        bool isNew = true;

        float randomX = Random.Range(-1.2f, 1.2f);
        Vector3 Pos = new Vector3(
                randomX + SurPos.x, SurPos.y + 1.5f, SurPos.z);

        for (int i = 0; i <= FootPrintsNum; ++i)
        {
            if (!FootPrints[i].activeSelf)
            {
                isNew = false;

                FootPrints[i].SetActive(true);
                FootPrints[i].transform.position = Pos;
                StartCoroutine(FootPrints[i].GetComponent<FootPrintCtrl>().Use());
                break;
            }
        }

        if (isNew)
        {
            FootPrints[++FootPrintsNum] = PhotonNetwork.Instantiate("FootPrintProjector", Pos,
                Quaternion.Euler(90, 0, Random.Range(0, 360)), 0);
            StartCoroutine(FootPrints[FootPrintsNum].GetComponent<FootPrintCtrl>().Use());
        }
    }

    IEnumerator DelayTime()
    {
        while (true)
        {
            delay -= Time.deltaTime;

            if (delay <= 0)
                break;

            yield return null;
        }
        delay = savedelay;
    }

    public void SetSurvivorScore(int score)
    {
        if(Character == 1)
        {
            SurvivorScore[4] += score;
            print("Survivor : " + SurvivorScore);
        }
    }

    public void SetMurdererScore(int score)
    {
        if (Character == 2)
        {
            MurdererScore[4] += score;
            print("Murderer : " + MurdererScore);
        }
    }

    public IEnumerator StartHit(float delay)
    {
        Color c = Hit.color;
        c.a = 0;
        Hit.color = c;

        while (Hit.color.a <= 1)
        {
            c.a += Time.deltaTime * 0.5f;
            Hit.color = c;
            yield return null;
        }

        yield return new WaitForSeconds(delay);

        c.a = 1;
        Hit.color = c;

        while (Hit.color.a >= 0)
        {
            c.a -= Time.deltaTime * 0.5f;
            Hit.color = c;
            yield return null;
        }
    }

    public IEnumerator StartFade(bool start)
    {
        Fade.SetActive(true);
        Image imgFade = Fade.GetComponent<Image>();

        Color color = imgFade.color;
        float time = 0;

        if (start)
        {
            color.a = 1;

            while (color.a > 0)
            {
                time += Time.deltaTime * 0.8f;

                color.a = Mathf.Lerp(1, 0, time);
                imgFade.color = color;

                yield return null;
            }

            isStart = true;
            Fade.SetActive(false);
        }
        else
        {
            color.a = 0;
            while (color.a < 1)
            {
                time += Time.deltaTime * 0.8f;
                color.a = Mathf.Lerp(0, 1, time);
                imgFade.color = color;

                yield return null;
            }
            ExitRoom();
        }
    }

    IEnumerator DisFPS()
    {
        float deltaTime = 0.0f;
        float fps;

        while (true)
        {
            deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
            fps = 1.0f / deltaTime;

            print(fps);

            if (Character == 1)
                SurvivorUICtrl.instance.DisFPS(fps);
            else if (Character == 2)
                MurdererUICtrl.instance.DisFPS(fps);

            yield return null;
        }
    }

    public void ExitRoom()
    {
        if (Character == 1)
        {
            for(int i = 0; i < 5; ++i)
                PlayerPrefs.SetInt("SurScore" + i, SurvivorScore[i]);
        }
        else if (Character == 2)
        {
            for (int i = 0; i < 5; ++i)
                PlayerPrefs.SetInt("MurScore" + i, MurdererScore[i]);
        }

        PhotonNetwork.LeaveRoom();
    }

    void OnLeftRoom()
    {
        SceneManager.LoadScene("3. Result");
    }
}
