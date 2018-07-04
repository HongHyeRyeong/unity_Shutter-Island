using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PhotonInit : MonoBehaviour {

    // App의 버전 정보
    public string version = "v1.0";

    // 룸 이름을 입력받을 UI 항목 연결 변수
    public InputField roomName;

    static public int Map = 0;

    // Use this for initialization
    void Awake () {
        // 포톤 클라우드에 접속
        PhotonNetwork.ConnectUsingSettings(version);
        // 룸 이름을 무작위로 설정
        roomName.text = "Room_" + Random.Range(0, 999).ToString("000");
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
            ao = Application.LoadLevelAsync("inGame1");
        else
            ao = Application.LoadLevelAsync("inGame2");

        yield return ao;
    }

    public void OnClickJoinRandomRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public void OnClickCreateRoom()
    {
        Map = Random.Range(1, 3);
        string _roomName = roomName.text;

        if(string.IsNullOrEmpty(roomName.text))
        {
            _roomName = "Room_" + Random.Range(0, 999).ToString("000");
        }

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsOpen = true;
        roomOptions.IsVisible = true;
        roomOptions.MaxPlayers = 5;
        //roomOptions.customRoomPropertiesForLobby = new string[1];
        //roomOptions.customRoomPropertiesForLobby[0] = "map";
        //roomOptions.customRoomProperties.Add("map", Map);

        PhotonNetwork.CreateRoom(_roomName, roomOptions, TypedLobby.Default);
    }

    void OnPhotonCreateRoomFailed(object[] codeAndMsg)
    {
        Debug.Log("Create Room Failed = " + codeAndMsg[1]);
    }

    void OnReceiveRoomListUpdate()
    {
        foreach(RoomInfo _room in PhotonNetwork.GetRoomList())
        {
            Debug.Log(_room.name);
        }
    }

    void OnGUI () {
        // 화면 좌측 상단에 접속 과정에 대한 로그를 출력
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
	}
}
