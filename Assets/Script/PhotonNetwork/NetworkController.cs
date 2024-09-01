using System;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using ExitGames.Client.Photon;

namespace NetworkControl
{

    public class NetworkController : MonoBehaviourPunCallbacks
    {
        private NetworkController instance;
        private Network_Observer_Notification networkobserver;

        

        void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                instance = this;
                DontDestroyOnLoad(this);
            }
        }


        void Start()
        {
            if (!PhotonNetwork.IsConnected)
            {
                PhotonNetwork.ConnectUsingSettings();

                Debug.Log("Connection");
            }
            else
            {
                Debug.Log("On Connection");
            }

            if (networkobserver == null)
            {
                networkobserver = gameObject.GetComponent<Network_Observer_Notification>();
            }
        }

        #region  Connection Method

        public override void OnConnectedToMaster() => SendNetwork_Packet_Notification(null, Network_State.OnConnectedToMaster);
        public override void OnDisconnected(DisconnectCause cause) => SendNetwork_Packet_Notification(cause, Network_State.OnDisconnected);
        public override void OnJoinedLobby() => SendNetwork_Packet_Notification(null, Network_State.OnJoinedLobby);
        public override void OnJoinedRoom() => SendNetwork_Packet_Notification(null, Network_State.OnJoinedRoom);
        public override void OnRoomListUpdate(List<RoomInfo> roomList) => SendNetwork_Packet_Notification(roomList, Network_State.OnRoomListUpdate);
        public override void OnCreatedRoom() => SendNetwork_Packet_Notification(null, Network_State.OnCreatedRoom);

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            string Failed_MessageJson = JsonUtility.ToJson(new Failded_Network_Entry{ returncode = returnCode , message = message});

            SendNetwork_Packet_Notification(Failed_MessageJson, Network_State.OnCreateRoomFailed);
        }
        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            string Failed_MessageJson = JsonUtility.ToJson(new Failded_Network_Entry{ returncode = returnCode , message = message});

            SendNetwork_Packet_Notification( Failed_MessageJson , Network_State.OnJoinRandomFailed);
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            string Failed_MessageJson = JsonUtility.ToJson(new Failded_Network_Entry { returncode = returnCode , message = message});

            SendNetwork_Packet_Notification( Failed_MessageJson, Network_State.OnJoinRoomFailed);
        }

        public override void OnLeftLobby() => SendNetwork_Packet_Notification(null, Network_State.OnLeftLobby);
        public override void OnLeftRoom() => SendNetwork_Packet_Notification(null, Network_State.OnLeftRoom);

        public override void OnPlayerEnteredRoom(Player newPlayer) => SendNetwork_Packet_Notification(newPlayer, Network_State.OnPlayerEnteredRoom);
        public override void OnPlayerLeftRoom(Player otherPlayer) => SendNetwork_Packet_Notification(otherPlayer, Network_State.OnPlayerLeftRoom);

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            string PlayerPropertiesToJson = JsonUtility.ToJson(new Player_HashtableEntry { playerInfo = targetPlayer, PropertiesInfo = changedProps });

            SendNetwork_Packet_Notification(PlayerPropertiesToJson, Network_State.OnConnectedToMaster);
        }
        public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged) => SendNetwork_Packet_Notification(propertiesThatChanged, Network_State.OnRoomPropertiesUpdate);

        public override void OnMasterClientSwitched(Player newMasterClient) => SendNetwork_Packet_Notification(newMasterClient, Network_State.OnMasterClientSwitched);

        #endregion

        private void SendNetwork_Packet_Notification(object NetworkPacket, Network_State Sender_Networkstate)
        {
            networkobserver.Notify_Network_notification(NetworkPacket, Sender_Networkstate);
        }

    }

    [Serializable]
    public class Player_HashtableEntry
    {
        public Player playerInfo;
        public Hashtable PropertiesInfo;
    }

    public class Failded_Network_Entry
    {
        public short returncode;
        public string message;
    }

}
