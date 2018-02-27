using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NetworkModule;
using NetworkUnity;
using GameServer;

public class NetworkManager : MonoBehaviour
{
    NetworkUnityService gameserver;

    void Awake()
    {
        // 네트워크 통신을 위해 NetworkUnityService 객체를 추가합니다.
        this.gameserver = gameObject.AddComponent<NetworkUnityService>();

        // 상태 변화(접속, 끊김 등)를 통보 받을 델리게이트 설정
        this.gameserver.appcallback_on_status_changed += on_status_changed;

        // 패킷 수신 델리게이트 설정
        this.gameserver.appcallback_on_message += on_message;
    }

    // Use this for initialization
    void Start()
    {
        connect();
    }

    void connect()
    {
        this.gameserver.connect("192.168.180.240", 7979);
    }

    /// <summary>
    /// 네트워크 상태 변경 시 호출될 콜백 메서드.
    /// </summary>
    /// <param name="server_token"></param> 
    void on_status_changed(NETWORK_EVENT status)
    {
        switch (status)
        {
            // 접속 성공
            case NETWORK_EVENT.connected:
                {
                    Debug.Log("on connected");

                    Packet msg = Packet.create((short)PROTOCOL.MOVE);

                    this.gameserver.send(msg);
                }
                break;

            // 연결 끊김
            case NETWORK_EVENT.disconnected:
                Debug.Log("disconnected");
                break;
        }
    }

    void on_message(Packet msg)
    {
        // 제일 먼저 프로토콜 아이디를 꺼내온다.
        PROTOCOL protocol_id = (PROTOCOL)msg.pop_protocol_id();

        // 프로토콜에 따른 분기 처리
        switch (protocol_id)
        {
            case PROTOCOL.MOVE:
                {
                    string text = msg.pop_string();
                    GameObject.Find("PlayerCtrl").GetComponent<PlayerCtrl>().InputGet();
                }
                break;
        }
    }

    public void send(Packet msg)
    {
        this.gameserver.send(msg);
    }
}
