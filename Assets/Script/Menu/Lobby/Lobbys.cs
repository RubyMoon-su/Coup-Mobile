using System;
using Photon.Pun;
using UnityEngine;
using NetworkControl;
using EventBus_System;
using ExitGames.Client.Photon;
using Coup_Mobile.Changescene;



namespace Coup_Mobile.Menu
{
    public class Lobbys : MonoBehaviourPun, INetworkObserver
    {
        #region Common Field

        // PhotonView.
        [HideInInspector] private static PhotonView photon_observer;

        // Network Observer Notification System.
        [HideInInspector] private Network_Observer_Notification network_observer_notification;

        // Lobby Observer Notification System.
        [HideInInspector] private Lobby_Observer lobby_observer;

        // Ref Component.
        [Header("Ref Component")]
        [SerializeField] private Lobby_SlotControl lobby_Slotcontrol;
        [SerializeField] private Lobby_UiControl lobby_uicontrol;

        // Time.
        [HideInInspector] private static readonly int SYSTEM_TIMEOUT = 6;
        [HideInInspector] private static readonly int NETWORK_TIMEOUT = 10;

        // Toggle Ready.
        [Header("Install Component Status")]
        [SerializeField] private static bool install_Complate = false;

        [Header("IsMasterClient")]
        [SerializeField] private static bool isMasterClient = false;

        #endregion

        #region Starter Function And Ending Function

        public void Awake()
        {
            // Starter Functon.
            StartCoroutine(StarterFunction());
        }

        public void OnDestory()
        {
            Debug.Log("Destory");
        }

        /// <summary>
        /// This Method is Process about StarterFunction.
        /// </summary>
        /// <returns> IEnumerator Deray For Install Component And Ui. </returns>
        private System.Collections.IEnumerator StarterFunction()
        {
            // Wait For 'Install Component' Function is Return 'True' Result.
            yield return new WaitUntil(InstallComponent);

            // Wait For 'Set up New Properties And Wait For 'Install Custom Properties' Process Is finished'.
            yield return StartCoroutine(Install_CustormProperties());

            // Process Function For First Time.
            Starter_HostSlot();
        }

        private bool InstallComponent()
        {
            try
            {
                // Find PhotonView.
                if (photon_observer == null)
                {
                    gameObject.TryGetComponent<PhotonView>(out photon_observer);
                }

                // Install Network Observer Notification.
                if (network_observer_notification == null)
                {
                    GameObject.Find("PhotonNetwork").gameObject.TryGetComponent<Network_Observer_Notification>(out network_observer_notification);
                }

                // Create Observer Patten.
                lobby_observer = new Lobby_Observer();

                // Create Lobby SubSystem Object.
                lobby_Slotcontrol = GetComponent<Lobby_SlotControl>();
                lobby_uicontrol = GetComponent<Lobby_UiControl>();

                // Register Observer System.
                lobby_observer.Attach_LobbyObserver(lobby_Slotcontrol, Observer_Lobby_Type.Observer_Slot);
                lobby_observer.Attach_LobbyObserver(lobby_uicontrol, Observer_Lobby_Type.Observer_Ui);

                // Attach Network Observer.
                network_observer_notification.Attach_Network_Notification(this);

                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                return false;
            }
        }

        private System.Collections.IEnumerator Install_CustormProperties()
        {
            // New Properties Player 'Ready' And Set Starter Status is False.
            Hashtable PlayerProperties = new Hashtable
            {
                {"Ready" , false},
            };

            // Update New Player Properties To PhotonNetworkServer.
            PhotonNetwork.LocalPlayer.SetCustomProperties(PlayerProperties);

            yield return new WaitForSeconds(1);

            // Check New Properties is Have "Ready"?.
            if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Ready"))
            {
                // Check And Set Value About Is Power Room.
                isMasterClient = PhotonNetwork.IsMasterClient;

                // All Process Is Complate.
                install_Complate = true;
                Debug.Log("Install Properties Complate");
            }
            else
            {
                Debug.LogError("Fail To SetCustomProperties Player");
            }
        }

        /// <summary>
        /// This Method is Update Ui Game First Time.
        /// </summary>
        private void Starter_HostSlot()
        {
            NetworkUpdate(null, Network_State.OnPlayerEnteredRoom);
        }

        #endregion

        #region Network Observer Notification System

        /// <summary>
        /// This Method Interface is implement from INetwork_Observer , Update New Properties Or Player EnterRoom , ExitRoom Other From NetworkObserver.
        /// </summary>
        /// <param name="PacketData">Import Data Argument about Packet Data anyType And Way Path Network State. </param>
        public void NetworkUpdate(object Network_Notification, Network_State State_Notification)
        {
            switch (State_Notification)
            {
                // This Network_State Data Is Union Process.
                case Network_State.OnPlayerPropertiesUpdate:
                case Network_State.OnRoomPropertiesUpdate:
                case Network_State.OnMasterClientSwitched:
                case Network_State.OnPlayerEnteredRoom:
                case Network_State.OnPlayerLeftRoom:

                    Debug.Log($"{State_Notification} 'Lobby'");

                    // RoomPropertiesUpdate Option.
                    if (State_Notification == Network_State.OnRoomPropertiesUpdate)
                    {
                        Photon.Realtime.Room RoomInfo_Update = PhotonNetwork.CurrentRoom;

                        UpdateRoomInfo RoomInfo = new UpdateRoomInfo()
                        {
                            roomName = RoomInfo_Update.Name,
                            roomPassword = (bool)RoomInfo_Update.CustomProperties["Room Status"],
                            playerInRoom = RoomInfo_Update.Players.Count,
                            playerMaxnium = RoomInfo_Update.MaxPlayers,
                            roomower = isMasterClient,
                        };

                        lobby_observer.Update_LobbyObserver(RoomInfo, Observer_Lobby_Type.Observer_Ui);

                        CustomRoomProperties_StarterInfo GameProperties_Data = JsonUtility.FromJson<CustomRoomProperties_StarterInfo>((string)RoomInfo_Update.CustomProperties["StartInfo"]);

                        UpdateGameProperties GameProperties = new UpdateGameProperties()
                        {
                            coinStarter = GameProperties_Data.GoldStarter,
                            coinMaxnium = GameProperties_Data.TotalGold,
                            cardStarter = GameProperties_Data.PerCard,
                            CardMaxnium = GameProperties_Data.TotalCard,
                        };

                        lobby_observer.Update_LobbyObserver(GameProperties, Observer_Lobby_Type.Observer_Ui);

                        CustomRoomProperties_CardInfo GameRule_Data = JsonUtility.FromJson<CustomRoomProperties_CardInfo>((string)RoomInfo_Update.CustomProperties["CardInfo"]);

                        UpdateGameRule GameRule = new UpdateGameRule()
                        {
                            dukeCard = GameRule_Data.Duke,
                            assassinCard = GameRule_Data.Assassin,
                            captainCard = GameRule_Data.Captain,
                            ambassdorCard = GameRule_Data.Ambassador,
                            contessaCard = GameRule_Data.Contessa,
                        };

                        lobby_observer.Update_LobbyObserver(GameRule, Observer_Lobby_Type.Observer_Ui);

                    }

                    // OnMasterClientSwitched Option.
                    if (State_Notification == Network_State.OnMasterClientSwitched)
                    {
                        isMasterClient = PhotonNetwork.IsMasterClient;
                    }

                    lobby_observer.Update_LobbyObserver(null, Observer_Lobby_Type.Observer_Slot);

                    break;
                case Network_State.OnDisconnected:

                    EventBus_SceneManager<IEvent>.RaiseSceneManager_Event(ChangeScene.Mainmenu, false, null);

                    break;
                case Network_State.OnLeftRoom:

                    EventBus_SceneManager<IEvent>.RaiseSceneManager_Event(ChangeScene.Mainmenu, false, null);

                    break;
            }
        }

        #endregion

        #region  Static Property 

        public static bool GetInstall_Complate
        {
            get => install_Complate;
        }

        public static int GetSystem_TimeOut
        {
            get => SYSTEM_TIMEOUT;
        }

        public static int GetNetwork_TimeOut
        {
            get => NETWORK_TIMEOUT;
        }

        public static PhotonView GetPhoton_Observer
        {
            get => photon_observer;
        }

        public static bool GetIsMaster_Client
        {
            get => isMasterClient;
        }

        #endregion
    }
}
