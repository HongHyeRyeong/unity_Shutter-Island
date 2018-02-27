using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using NetworkModule;

namespace NetworkUnity
{
    public class NetworkUnityService : MonoBehaviour
    {
        NetworkEventManager event_manager;

        // 연결된 게임 서버 객체
        IPeer gameserver;

        // TCP 통신을 위한 서비스 객체
        NetworkService service;

        // 네트워크 상태 변경 시 호출되는 델리게이트
        // 애플리케이션에서 콜백 메서드를 설정하여 사용한다
        public delegate void StatusChangeHandler(NETWORK_EVENT status);
        public StatusChangeHandler appcallback_on_status_changed;

        // 네트워크 메시지 수신 시 호출되는 델리게이트
        // 애플리케이션에서 콜백 메서드를 설정하여 사용한다
        public delegate void MessageHandler(Packet msg);
        public MessageHandler appcallback_on_message;

        void Awake()
        {
            PacketBufferManager.initialize(10);
            this.event_manager = new NetworkEventManager();
        }

        public void connect(string host, int port)
        {
            // NetworkService 객체는 메시지의 비동기 송,수신 처리를 수행한다.
            this.service = new NetworkService();

            // endpoint 정보를 갖고 있는 Connector 생성
            // 만들어둔 NetworkService 객체를 넣어준다.
            Connector connector = new Connector(service);

            // 접속 성공 시 호출될 콜백 메서드 지정
            connector.connected_callback += on_connected_gameserver;
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Parse(host), port);
            connector.connect(endpoint);
        }

        public bool is_connected()
        {
            return this.gameserver != null;
        }

        /// <summary> 
        /// 접속 성공 시 호출될 콜백 메서드.
        /// </summary> 
        /// <param name="server_token"></param> 
        void on_connected_gameserver(UserToken server_token)
        {
            this.gameserver = new RemoteServerPeer(server_token);

            ((RemoteServerPeer)this.gameserver).set_eventmanager(this.event_manager);

            // 유니티 애플리케이션으로 이벤트를 넘겨주기 위해서 매니저에 큐잉시켜준다.
            this.event_manager.enqueue_network_event(NETWORK_EVENT.connected);
        }

        /// <summary> 
        /// 네트워크에서 발생하는 모든 이벤트를 클라이언트에 알려주는 역할을 Update에서 진행한다. 
        /// NetworkModule 엔진의 메시지 송, 수신 처리는 워커 스레드에서 수행되지만 유니티의 로직 처리는 메인 스레드에서 수행되므로 큐잉 처리를 통하여 메인 스레드에서 모든 로직 처리가 이루어지도록 구성하였다.
        /// </summary> 
        void Update()
        {
            // 수신된 메시지에 대한 콜백
            if (this.event_manager.has_message())
            {
                Packet msg = this.event_manager.dequeue_network_message();
                if (this.appcallback_on_message != null)
                {
                    this.appcallback_on_message(msg);
                }
            }

            // 네트워크 발생 이벤트에 대한 콜백
            if (this.event_manager.has_event())
            {
                NETWORK_EVENT status = this.event_manager.dequeue_network_event();

                if (this.appcallback_on_status_changed != null)
                {
                    this.appcallback_on_status_changed(status);
                }
            }
        }

        public void send(Packet msg)
        {
            try
            {
                this.gameserver.send(msg);
                Packet.destroy(msg);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        /// <summary>
        /// 정상적인 종료시에는 OnApplicationQuit매소드에서 disconnect를 호출해 줘야 유니티가 hang되지 않는다.
        /// </summary>
        void OnApplicationQuit()
        {
            if (this.gameserver != null)
            {
                ((RemoteServerPeer)this.gameserver).token.disconnect();
            }
        }
    }
}