using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NetworkControl
{
    public enum Network_State
    {
        OnConnectedToMaster,
        OnDisconnected,
        OnJoinedLobby,
        OnJoinedRoom,
        OnRoomListUpdate,
        OnCreatedRoom,
        OnCreateRoomFailed,
        OnJoinRandomFailed,
        OnJoinRoomFailed,
        OnLeftLobby,
        OnLeftRoom,
        OnPlayerEnteredRoom,
        OnPlayerLeftRoom,
        OnPlayerPropertiesUpdate,
        OnRoomPropertiesUpdate,
        OnMasterClientSwitched,
        
    }
    public interface INetworkObserver
    {
        void NetworkUpdate(object Network_Notification , Network_State State_Notification);
    }

    public interface INetworkOSubject
    {
        void Attach_Network_Notification(INetworkObserver Network_Notification);
        void Detach_Network_Notification(INetworkObserver Network_Notification);
        void Notify_Network_notification(object Data_Notification , Network_State State_Notification);
    }

    public class Network_Observer_Notification : MonoBehaviour , INetworkOSubject
    {
        private Network_Observer_Notification instance;

        #region Observer Collection

        private List<INetworkObserver> request_network_notification = new List<INetworkObserver>();
        #endregion

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

        #region  Network Subject Interface

        public void Attach_Network_Notification(INetworkObserver Network_Notification)
        {
            if (!request_network_notification.Contains(Network_Notification))
            {
                request_network_notification.Add(Network_Notification);
            }
        }

        public void Detach_Network_Notification(INetworkObserver Network_Notification)
        {
            if (request_network_notification.Contains(Network_Notification))
            {
                request_network_notification.Remove(Network_Notification);
            }
        }

        // Send Packet Form NetworkPunCallback To sub system observer.
        public void Notify_Network_notification(object Data_Notification , Network_State State_Notification)
        {
            foreach (INetworkObserver Observer_Excute in request_network_notification)
            {
                Observer_Excute.NetworkUpdate(Data_Notification , State_Notification);
            }
        }

        #endregion
    }
}
