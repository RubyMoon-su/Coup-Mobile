using System;
using Photon.Pun;
using UnityEngine;
using NetworkControl;
using System.Collections;
using Coup_Mobile.EventBus;
using Coup_Mobile.Menu.GameSetting_Data;
using Coup_Mobile.InGame.GameManager.ReportData;


namespace Coup_Mobile.InGame.GameManager
{
    public enum GameMode
    {
        None,
        NormalGame,
        RefomationGame,
        CustormGame,
    }

    public enum GameManager_Event
    {
        GameResourceManager,
        GameStateManager,
        PlayerManager,
        GameNetworkManager,
        GameUiManager,
        GameAssistManager,

    }

    public class GameManager : MonoBehaviour, INetworkObserver
    {
        [Header("All Component Status")]
        [SerializeField] private static bool install_complate = false;

        [Header("Component Manager")]
        [SerializeField] private PlayerManager playerManager;
        [SerializeField] private GameStateManager gameStateManager;
        [SerializeField] private GameResourceManager gameResourceManager;
        [SerializeField] private GameNetworkManager gameNetworkManager;
        [SerializeField] private GameUiManager gameUiManager;
        [SerializeField] private GameAssistManager gameAssistManager;

        [SerializeField] private GameSetting gameSetting;

        [HideInInspector] private GameMode gameMode = GameMode.None;

        [HideInInspector] private Network_Observer_Notification networkObserver;

        [SerializeField] public static readonly int SYSTEM_TIMEOUT = 10;
        [SerializeField] public static readonly int NETWORK_TIMEOUT = 10;

        public void Start()
        {
            // Test System.
            //SetUp_GameEvent();

            //StartCoroutine(InstallConponent());
        }

        public void OnDestory()
        {
            
        }

        public void OnStarterGameManager()
        {
            SetUp_GameEvent();

            StartCoroutine(InstallConponent());
        }

        private void SetUp_GameEvent()
        {
            try
            {
                if (!PhotonNetwork.IsConnected)
                {
                    Debug.LogError("Player Disconnect.");
                    return;
                }
                else
                {

                    string GameSetting_Json = (string)PhotonNetwork.CurrentRoom.CustomProperties["GameSetting"];

                    GameSetting GameSetting_Data = JsonUtility.FromJson<GameSetting>(GameSetting_Json);

                    if (!GameSetting_Data.Equals(typeof(GameSetting)))
                    {
                        Debug.LogWarning("GameSetting Complate");
                        gameSetting = GameSetting_Data;
                    }
                    else
                    {
                        Debug.LogError("GameSetting Is Null");
                    }

                    // Register Network Observer Notification.
                    //networkObserver = GameObject.Find("PhotonNetwork").gameObject.GetComponent<Network_Observer_Notification>();
                    //networkObserver.Attach_Network_Notification(this);

                    // Register EventBus GameManager.
                    InGameManager<IInGameEvent> @event = new InGameManager<IInGameEvent>((_) => Request_Command(_));
                    EventBus_InGameManager<IInGameEvent>.EventCommand_Control(@event, "GameManager");

                    Debug.Log("GameManager Install GameEvent Complate.");

                    install_complate = true;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                install_complate = false;
            }
        }

        public T Request_Command<T>(T PacketData) where T : IInGameEvent
        {
            IInGameEvent Return_Data = null;

            if (PacketData is GameManager_Data gameManager_Data)
            {
                switch (gameManager_Data.gameManager_Event)
                {
                    case GameManager_Event.GameResourceManager:

                        GameResource_Result Return_GameResource = gameResourceManager.GameResource_Control(gameManager_Data); 

                        Return_Data = Return_GameResource;

                        break;
                    case GameManager_Event.GameStateManager:

                        GameStateManager_Return Return_StateManager = gameStateManager.GameStateManager_Control(gameManager_Data); 

                        Return_Data = Return_StateManager;

                        break;
                    case GameManager_Event.PlayerManager:

                        PlayerManager_Return Return_PlayerManager = playerManager.PlayerManager_Control(gameManager_Data);

                        Return_Data = Return_PlayerManager;

                        break;
                    case GameManager_Event.GameUiManager:
                        
                        GameUIManager_Return Return_UIManager = gameUiManager.GameUIManager_Control(gameManager_Data);

                        Return_Data = Return_UIManager;

                        break;
                    case GameManager_Event.GameNetworkManager:

                        GameNetworkManager_Return Return_NetworkManager = gameNetworkManager.GameNetworkManager_Control(gameManager_Data);

                        Return_Data = Return_NetworkManager;

                        break;
                    case GameManager_Event.GameAssistManager:

                        GameAssistManager_Return Return_AssistManager = gameAssistManager.GameAssistManager_Control(gameManager_Data);

                        Return_Data = Return_AssistManager;

                    break;
                }
            }
            
            if (Return_Data != null)
            {
                return (T)Return_Data;
            }
             
            return default;
        }

        private IEnumerator InstallConponent()
        {
            int Timer = 0;

            while (!install_complate)
            {
                yield return new WaitForSeconds(1);

                Timer++;

                if (Timer == SYSTEM_TIMEOUT)
                {
                    Debug.LogError("Install Component GameManager Is TimeOut.");
                    yield break;
                }
            }

            gameAssistManager = new GameAssistManager(this);
            playerManager = new PlayerManager(this, gameSetting);
            gameStateManager = new GameStateManager(this);
            gameResourceManager = new GameResourceManager(this, gameSetting);
            gameNetworkManager = GetComponent<GameNetworkManager>();
            gameNetworkManager.Install_System();
            gameUiManager = new GameUiManager(this);

            yield break;
        }

        public void NetworkUpdate(object Network_Notification, Network_State State_Notification)
        {


            switch (State_Notification)
            {
                case Network_State.OnDisconnected:
                    break;
                case Network_State.OnPlayerPropertiesUpdate:
                    break;
            }
        }

        public static bool GetInstall_ComponentStatus
        {
            get => install_complate;
        }

        public GameMode GetGameMode
        {
            get => gameMode;
        }

        public static int GetSystemTimeout
        {
            get => SYSTEM_TIMEOUT;
        }

        public static int GetNetworkTimeOut
        {
            get => NETWORK_TIMEOUT;
        }

    }
}