using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PhotonInit : MonoBehaviour
{
    // App의 버전 정보
    public string version = "v1.0";

    // 룸 이름을 입력받을 UI 항목 연결 변수
    //public InputField roomName;

    public GameObject scrollContents;
    public GameObject roomItem;

    public static PhotonInit instance;

    public int Map = 0;
    int[] itemrand = new int[46];
    int[] gadgetrand = new int[35];
    int[] keyrand = new int[10];

    //int[] tmp = new int[92];
    float[] random = new float[182];

    // Use this for initialization
    void Awake()
    {
        // 포톤 클라우드에 접속
        PhotonNetwork.ConnectUsingSettings(version);
        // 룸 이름을 무작위로 설정
        //roomName.text = "Room_" + Random.Range(0, 999).ToString("000");

        instance = this;
    }

    // 포톤 클라우드에 정상적으로 접속한 후 로비에 입장하면 호출되는 콜백 함수
    void OnJoinedLobby()
    {
        Debug.Log("Entered Lobby !");
    }

    // 무작위 룸 접속에 실패한 경우 호출되는 콜백 함수
    void OnPhotonRandomJoinFailed()
    {
        Debug.Log("No Rooms !");
        // 룸 생성
        PhotonNetwork.CreateRoom("MyRoom");
    }

    // 룸에 입장하면 호출되는 콜백 함수
    void OnJoinedRoom()
    {
        Debug.Log("Enter Room !");

        // 생존자를 생성하는 함수 호출
        // CreateSurvivor();

        StartCoroutine(this.LoadBattleField());
    }

    IEnumerator LoadBattleField()
    {
        PhotonNetwork.isMessageQueueRunning = false;

        AsyncOperation ao;

        if (Map == 1)
        {
            ao = Application.LoadLevelAsync("2. inGame1");

            for (int i = 0; i < 46; ++i)
                PlayerPrefs.SetInt("itemrand" + (i + 1), itemrand[i]);
            for (int i = 0; i < 35; ++i)
                PlayerPrefs.SetInt("gadgetrand" + (i + 47), gadgetrand[i]);
            for (int i = 0; i < 10; ++i)
                PlayerPrefs.SetInt("keyrand" + (i + 82), keyrand[i]);
            for (int i = 0; i < 182; ++i)
                PlayerPrefs.SetFloat("random" + (i + 1), random[i]);
        }
        else
        {
            ao = Application.LoadLevelAsync("2. inGame2");

            for (int i = 0; i < 46; ++i)
                PlayerPrefs.SetInt("itemrand" + (i + 1), itemrand[i]);
            for (int i = 0; i < 35; ++i)
                PlayerPrefs.SetInt("gadgetrand" + (i + 47), gadgetrand[i]);
            for (int i = 0; i < 10; ++i)
                PlayerPrefs.SetInt("keyrand" + (i + 82), keyrand[i]);
            for (int i = 0; i < 182; ++i)
                PlayerPrefs.SetFloat("random" + (i + 1), random[i]);
        }

        yield return ao;
    }

    //public void OnClickJoinRandomRoom()
    //{
    //    PhotonNetwork.JoinRandomRoom();
    //}

    public void OnClickCreateRoom()
    {
        if (CharacterSelect.instance.Map != 0)
        {
            if (CharacterSelect.instance.Map == 1)
                Map = 1;
            else if (CharacterSelect.instance.Map == 2)
                Map = 2;
            else if (CharacterSelect.instance.Map == 3)
                Map = Random.Range(1, 3);

            string _roomName = "Room_" + Random.Range(0, 999).ToString("000");

            //if(string.IsNullOrEmpty(roomName.text))
            //{
            //    _roomName = "Room_" + Random.Range(0, 999).ToString("000");
            //}

            RoomOptions roomOptions = new RoomOptions();
            roomOptions.IsOpen = true;
            roomOptions.IsVisible = true;
            roomOptions.MaxPlayers = 5;

            roomOptions.customRoomProperties = new ExitGames.Client.Photon.Hashtable();
            roomOptions.customRoomPropertiesForLobby = new string[274];
            roomOptions.customRoomPropertiesForLobby[0] = "map";
            roomOptions.customRoomProperties.Add("map", Map);

            for (int i = 0; i < 46; ++i)
            {
                itemrand[i] = Random.Range(0, 12);
                roomOptions.customRoomPropertiesForLobby[i + 1] = "itemrand" + (i + 1);
                roomOptions.customRoomProperties.Add("itemrand" + (i + 1), itemrand[i]);
            }
            for (int i = 0; i < 35; ++i)
            {
                gadgetrand[i] = Random.Range(0, 12);
                roomOptions.customRoomPropertiesForLobby[i + 47] = "gadgetrand" + (i + 47);
                roomOptions.customRoomProperties.Add("gadgetrand" + (i + 47), gadgetrand[i]);
            }
            for (int i = 0; i < 10; ++i)
            {
                keyrand[i] = Random.Range(0, 12);
                roomOptions.customRoomPropertiesForLobby[i + 82] = "keyrand" + (i + 82);
                roomOptions.customRoomProperties.Add("keyrand" + (i + 82), keyrand[i]);
            }
            for (int i = 0; i < 182; ++i)
            {
                random[i] = Random.Range(-3.0f, 3.0f);
                roomOptions.customRoomPropertiesForLobby[i + 92] = "random" + (i + 1);
                roomOptions.customRoomProperties.Add("random" + (i + 1), random[i]);
            }

            PhotonNetwork.CreateRoom(_roomName, roomOptions, TypedLobby.Default);
        }
    }

    void OnPhotonCreateRoomFailed(object[] codeAndMsg)
    {
        Debug.Log("Create Room Failed = " + codeAndMsg[1]);
    }

    void OnReceivedRoomListUpdate()
    {
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("ROOM_ITEM"))
        {
            Destroy(obj);
        }

        int rowCount = 0;

        foreach (RoomInfo _room in PhotonNetwork.GetRoomList())
        {
            Debug.Log(_room.Name);
            GameObject room = (GameObject)Instantiate(roomItem);
            room.transform.SetParent(scrollContents.transform, false);

            RoomData roomData = room.GetComponent<RoomData>();
            roomData.roomName = _room.Name;
            roomData.connectPlayer = _room.PlayerCount;
            roomData.maxPlayers = _room.MaxPlayers;
            roomData.cp = _room.customProperties;

            Map = (int)roomData.cp["map"];

            for (int i = 0; i < 46; ++i)
                itemrand[i] = (int)roomData.cp["itemrand" + (i + 1)];
            for (int i = 0; i < 35; ++i)
                gadgetrand[i] = (int)roomData.cp["gadgetrand" + (i + 47)];
            for (int i = 0; i < 10; ++i)
                keyrand[i] = (int)roomData.cp["keyrand" + (i + 82)];
            for (int i = 0; i < 182; ++i)
                random[i] = (float)roomData.cp["random" + (i + 1)];

            roomData.DispRoomData();

            scrollContents.GetComponent<GridLayoutGroup>().constraintCount = ++rowCount;

            roomData.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate { OnClickRoomItem(roomData.roomName); });
        }
    }

    string roomname;

    void OnClickRoomItem(string roomName)
    {
        roomname = roomName;
        print("room check");
    }

    public void OnClickRoomBtn()
    {
        if (CharacterSelect.instance.SurStat != 0)
            PhotonNetwork.JoinRoom(roomname);
    }

    void OnGUI()
    {
        // 화면 좌측 상단에 접속 과정에 대한 로그를 출력
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
    }

    //public void OnClickCreateRoomMap1()
    //{
    //    Map = 1;

    //    string _roomName = "Room_" + Random.Range(0, 999).ToString("000");

    //    //if (string.IsNullOrEmpty(roomName.text))
    //    //{
    //    //    _roomName = "Room_" + Random.Range(0, 999).ToString("000");
    //    //}

    //    RoomOptions roomOptions = new RoomOptions();
    //    roomOptions.IsOpen = true;
    //    roomOptions.IsVisible = true;
    //    roomOptions.MaxPlayers = 5;

    //    roomOptions.customRoomProperties = new ExitGames.Client.Photon.Hashtable();
    //    roomOptions.customRoomPropertiesForLobby = new string[274];
    //    roomOptions.customRoomPropertiesForLobby[0] = "map";
    //    roomOptions.customRoomProperties.Add("map", Map);

    //    for (int i = 0; i < 46; ++i)
    //    {
    //        itemrand[i] = Random.Range(0, 12);
    //        roomOptions.customRoomPropertiesForLobby[i + 1] = "itemrand" + (i + 1);
    //        roomOptions.customRoomProperties.Add("itemrand" + (i + 1), itemrand[i]);
    //    }
    //    for (int i = 0; i < 35; ++i)
    //    {
    //        gadgetrand[i] = Random.Range(0, 12);
    //        roomOptions.customRoomPropertiesForLobby[i + 47] = "gadgetrand" + (i + 47);
    //        roomOptions.customRoomProperties.Add("gadgetrand" + (i + 47), gadgetrand[i]);
    //    }
    //    for (int i = 0; i < 10; ++i)
    //    {
    //        keyrand[i] = Random.Range(0, 12);
    //        roomOptions.customRoomPropertiesForLobby[i + 82] = "keyrand" + (i + 82);
    //        roomOptions.customRoomProperties.Add("keyrand" + (i + 82), keyrand[i]);
    //    }
    //    for (int i = 0; i < 182; ++i)
    //    {
    //        random[i] = Random.Range(-3.0f, 3.0f);
    //        roomOptions.customRoomPropertiesForLobby[i + 92] = "random" + (i + 1);
    //        roomOptions.customRoomProperties.Add("random" + (i + 1), random[i]);
    //    }

    //    PhotonNetwork.CreateRoom(_roomName, roomOptions, TypedLobby.Default);
    //}

    //public void OnClickCreateRoomMap2()
    //{
    //    Map = 2;

    //    string _roomName = "Room_" + Random.Range(0, 999).ToString("000");

    //    //if (string.IsNullOrEmpty(roomName.text))
    //    //{
    //    //    _roomName = "Room_" + Random.Range(0, 999).ToString("000");
    //    //}

    //    RoomOptions roomOptions = new RoomOptions();
    //    roomOptions.IsOpen = true;
    //    roomOptions.IsVisible = true;
    //    roomOptions.MaxPlayers = 5;

    //    roomOptions.customRoomProperties = new ExitGames.Client.Photon.Hashtable();
    //    roomOptions.customRoomPropertiesForLobby = new string[274];
    //    roomOptions.customRoomPropertiesForLobby[0] = "map";
    //    roomOptions.customRoomProperties.Add("map", Map);

    //    for (int i = 0; i < 46; ++i)
    //    {
    //        itemrand[i] = Random.Range(0, 12);
    //        roomOptions.customRoomPropertiesForLobby[i + 1] = "itemrand" + (i + 1);
    //        roomOptions.customRoomProperties.Add("itemrand" + (i + 1), itemrand[i]);
    //    }
    //    for (int i = 0; i < 35; ++i)
    //    {
    //        gadgetrand[i] = Random.Range(0, 12);
    //        roomOptions.customRoomPropertiesForLobby[i + 47] = "gadgetrand" + (i + 47);
    //        roomOptions.customRoomProperties.Add("gadgetrand" + (i + 47), gadgetrand[i]);
    //    }
    //    for (int i = 0; i < 10; ++i)
    //    {
    //        keyrand[i] = Random.Range(0, 12);
    //        roomOptions.customRoomPropertiesForLobby[i + 82] = "keyrand" + (i + 82);
    //        roomOptions.customRoomProperties.Add("keyrand" + (i + 82), keyrand[i]);
    //    }
    //    for (int i = 0; i < 182; ++i)
    //    {
    //        random[i] = Random.Range(-3.0f, 3.0f);
    //        roomOptions.customRoomPropertiesForLobby[i + 92] = "random" + (i + 1);
    //        roomOptions.customRoomProperties.Add("random" + (i + 1), random[i]);
    //    }

    //    PhotonNetwork.CreateRoom(_roomName, roomOptions, TypedLobby.Default);
    //}
}
