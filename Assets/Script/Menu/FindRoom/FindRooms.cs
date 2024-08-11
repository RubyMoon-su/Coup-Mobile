using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using NetworkControl;
using Coup_Mobile.EventBus;
using Photon.Realtime;
using Coup_Mobile.Changescene;
using System.Collections.Generic;

namespace Coup_Mobile.Menu
{

    public class FindRooms : MonoBehaviourPun, INetworkObserver
    {
        #region Pool RoomItem Gameobject Field

        [HideInInspector] private List<RoomItem> allroomlist = new List<RoomItem>();

        [SerializeField] private List<RoomItem> all_room_active = new List<RoomItem>();

        #endregion

        #region Update Room Field

        private List<RoomInfo> update_room_list = new List<RoomInfo>();

        #endregion

        #region Prefab Field

        [SerializeField] private GameObject RoomTemplate;

        #endregion

        #region Ref Component

        [HideInInspector] private Network_Observer_Notification network_observer;
        [SerializeField] private InsertPassword insert_form;
        [SerializeField] private Transform room_contex;
        [SerializeField] private Text refresh_text;

        #endregion

        #region Starter Function And Ending Function

        public void Start()
        {
            GameObject.Find("PhotonNetwork").gameObject.TryGetComponent(out Network_Observer_Notification network_observer);

            if (network_observer == null)
            {
                Debug.LogError("Can't Connect Network Observer Notification.");
            }

            insert_form.gameObject.SetActive(false);

            network_observer.Attach_Network_Notification(this);

            CreateObjectPool();

            if (!PhotonNetwork.InLobby) PhotonNetwork.JoinLobby();

            PhotonNetwork.NickName = "NewPlayer" + Random.Range(0 , 1000);

        }

        private void CreateObjectPool()
        {
            // Room Collection Pool.
            for (int i = 0; i < 20; i++)
            {
                if (all_room_active.Count > 20) return;

                GameObject RoomSolt = Instantiate(RoomTemplate, room_contex);

                RoomItem NewRoomSolt = RoomSolt.AddComponent<RoomItem>();

                NewRoomSolt.insert_form = insert_form;

                all_room_active.Add(NewRoomSolt);

                NewRoomSolt.gameObject.SetActive(false);
            }
        }

        public void Destroy()
        {
            network_observer.Detach_Network_Notification(this);

            Debug.LogWarning("Destory 'FindRoom'");
        }

        #endregion

        #region  Local Function
        public void UpdateRoom_List(List<RoomInfo> RoomInfo)
        {
            if (all_room_active.Count <= 0)
            {
                Debug.LogWarning("Pool List Is Full");

                return;
            }
            else
            {
                for (int i = 0; i < allroomlist.Count; i++)
                {
                    RoomItem ItemRoom = allroomlist[i];

                    ItemRoom.RemoveRoomItem();

                    ItemRoom.gameObject.SetActive(false);

                    allroomlist.Remove(ItemRoom);

                    all_room_active.Add(ItemRoom);
                }
            }

            for (int i = 0; i < RoomInfo.Count; i++)
            {
                RoomInfo NewRoomList = RoomInfo[i];

                RoomItem ItemRoom = all_room_active[i];

                ItemRoom.AddRoomInfo(NewRoomList);

                ItemRoom.gameObject.SetActive(true);

                allroomlist.Add(ItemRoom);

                all_room_active.Remove(ItemRoom);
            }
        }

        #endregion

        #region Button Function

        public void Refresh_Function()
        {
            if (allroomlist.Count > 0)
            {
                refresh_text.text = "Refresh";

                UpdateRoom_List(update_room_list);
            }
            else
            {
                refresh_text.text = "Refreshed";
            }
        }

        public void CreateRoom_Function()
        {
            EventBus_SceneManager<IEvent>.RaiseSceneManager_Event(ChangeScene.CreateRoom, false, null);
        }

        public void BackToMainMenu_Function()
        {
            if (PhotonNetwork.InLobby)
            {
                PhotonNetwork.LeaveLobby();
            }
        }

        #endregion

        #region Network Observer Update.

        public void NetworkUpdate(object Network_Notification, Network_State State_Notification)
        {
            switch (State_Notification)
            {
                case Network_State.OnRoomListUpdate:

                    Debug.LogWarning("OnRoomListUpdate 'FindRoom'");

                    if (Network_Notification is List<RoomInfo> UpdateRoom)
                    {
                        update_room_list = UpdateRoom;

                        if (update_room_list.Count > 0) refresh_text.text = "Refresh(New)";
                        else refresh_text.text = "Refresh";

                        if (allroomlist.Count == 0) UpdateRoom_List(UpdateRoom);
                    }

                    break;
                case Network_State.OnJoinedRoom:

                    Debug.Log("Join Room 'FindRoom'");

                    EventBus_SceneManager<IEvent>.RaiseSceneManager_Event(ChangeScene.Lobby, true, null);

                    break;
                case Network_State.OnDisconnected:

                    Debug.LogError($"Disconnected server 'FindRoom'");

                    EventBus_SceneManager<IEvent>.RaiseSceneManager_Event(ChangeScene.Mainmenu, false, Network_Notification);

                    break;
                case Network_State.OnLeftLobby:

                    Debug.Log("LeftLobby 'FindRoom'");

                    EventBus_SceneManager<IEvent>.RaiseSceneManager_Event(ChangeScene.Mainmenu, false, null);

                    break;
            }
        }

        #endregion
    }
}