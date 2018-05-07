using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PhotonInit : MonoBehaviour {

    // App의 버전 정보
    public string version = "v1.0";

    // Use this for initialization
    void Awake () {
        // 포톤 클라우드에 접속
        PhotonNetwork.ConnectUsingSettings(version);
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

        AsyncOperation ao = Application.LoadLevelAsync("Shutter Island");
        yield return ao;
    }

    public void OnClickJoinRandomRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    void OnGUI () {
        // 화면 좌측 상단에 접속 과정에 대한 로그를 출력
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
	}
}
