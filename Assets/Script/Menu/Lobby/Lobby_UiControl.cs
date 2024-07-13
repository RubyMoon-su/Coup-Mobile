using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using ExitGames.Client.Photon;
using Photon.Realtime;
using System.Linq;

namespace Coup_Mobile.Menu
{

    public class Lobby_UiControl : MonoBehaviourPun, ILobby_Observer
    {
        #region Field Common

        [Header("Install Complate Status")]
        [SerializeField] private bool install_complate = false;

        [Header("Display RoomInfo")]
        [SerializeField] private Text room_name_display;
        [SerializeField] private Text room_password_display;
        [SerializeField] private Text room_playerinroom_display;
        [SerializeField] private Text room_maxnium_display;

        [Header("Display Game Propertis")]
        [SerializeField] private Text game_coinstater_display;
        [SerializeField] private Text game_coinmaxnium_display;
        [SerializeField] private Text game_cardstarter_display;
        [SerializeField] private Text game_cardmaxnium_display;

        [Header("Display Game Rule")]
        [SerializeField] private Text duke_display;
        [SerializeField] private Text assassin_display;
        [SerializeField] private Text captain_display;
        [SerializeField] private Text ambassador_display;
        [SerializeField] private Text contessa_display;

        [Header("Button Display And Interactive")]
        [SerializeField] private Text readyOrStartGame_display;
        [SerializeField] private Button readyOrStartGame_Interactive;
        [SerializeField] private bool isReady = false;

        #endregion

        #region Start Function And Ending Function

        public void Awake()
        {
            Install_DisplayComponent();
        }

        /// <summary>
        /// This Method is GetComponent Text And Button, Verify All Component is Install Complete.
        /// </summary>
        private void Install_DisplayComponent()
        {
            try
            {
                GameObject Display_Properties = transform.Find("Display_Properties").gameObject;
                // RoomInfo Display Zone.
                room_name_display = Display_Properties.transform.Find("RoomName_Display").gameObject.GetComponent<Text>();
                room_password_display = Display_Properties.transform.Find("RoomPassword_Display").gameObject.GetComponent<Text>();
                room_playerinroom_display = Display_Properties.transform.Find("PlayerInRoom_Display").gameObject.GetComponent<Text>();
                room_maxnium_display = Display_Properties.transform.Find("PlayerMaxnium_Display").gameObject.GetComponent<Text>();

                // Game Properties Zone.
                game_coinstater_display = Display_Properties.transform.Find("CoinStarter_Displayer").gameObject.GetComponent<Text>();
                game_coinmaxnium_display = Display_Properties.transform.Find("CoinMaxnium_Display").gameObject.GetComponent<Text>();
                game_cardstarter_display = Display_Properties.transform.Find("CardStarter_Display").gameObject.GetComponent<Text>();
                game_cardmaxnium_display = Display_Properties.transform.Find("CardMaxnium_Display").gameObject.GetComponent<Text>();

                // Game Rule Zone.
                duke_display = Display_Properties.transform.Find("Duke_Display").gameObject.GetComponent<Text>();
                assassin_display = Display_Properties.transform.Find("Assassins_Display").gameObject.GetComponent<Text>();
                captain_display = Display_Properties.transform.Find("Captain_Display").gameObject.GetComponent<Text>();
                ambassador_display = Display_Properties.transform.Find("Ambassdor_Display").gameObject.GetComponent<Text>();
                contessa_display = Display_Properties.transform.Find("Contessa_Display").gameObject.GetComponent<Text>();

                readyOrStartGame_display = GameObject.Find("Button").gameObject.transform.Find("Ready").gameObject.GetComponentInChildren<Text>();
                readyOrStartGame_Interactive = GameObject.Find("Button").gameObject.transform.Find("Ready").gameObject.GetComponent<Button>();

                install_complate = true;
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex);
                install_complate = false;
            }
        }

        #endregion

        #region Observer System

        /// <summary>
        /// This Method Interface is implement from ILobby_Observer , Update RoomInfo or GameRule From NetworkObserver.
        /// </summary>
        /// <param name="PacketData">Import DataArgument about RoomInfo or GamePropertis or GameRule DataType.</param>
        public void Update_Notification(object PacketData)
        {
            while (!install_complate)
            {
                Debug.Log("Wait Install UI.");
            }

            if (PacketData is UpdateRoomInfo RoomInfo)
            {
                // Call Update_RoomInfo With RoomInfo Argument.
                Update_RoomInfo(RoomInfo);

                ReadyOrStart_Interctable();
            }
            else if (PacketData is UpdateGameProperties GameProperties)
            {
                Update_GameProperties(GameProperties);
            }
            else if (PacketData is UpdateGameRule GameRule)
            {
                Update_GameRule(GameRule);
            }
        }

        #endregion

        #region Button Event

        public void Ready_Function()
        {
            // Toggle Ready Status.
            bool ToggleReady = !isReady;

            // New Properties Data Update About 'Ready' for Player.
            Hashtable UpdateReady_Properties = new Hashtable();

            if (ToggleReady)
            {
                // Change ReadyOrStartGame Text Name to 'UnReady'.
                readyOrStartGame_display.text = "UnReady";

                // Add New Status Ready To Properties.
                UpdateReady_Properties.Add("Ready", true);
            }
            else
            {
                // Change ReadyOrStartGame Text Name to 'Ready'.
                readyOrStartGame_display.text = "Ready";

                // Add New Status Ready To Properties.
                UpdateReady_Properties.Add("Ready", false);
            }

            // Check New Properties is Have 'Ready'?
            if (UpdateReady_Properties.ContainsKey("Ready"))
            {
                Debug.Log($"{PhotonNetwork.LocalPlayer.NickName} is {ToggleReady}");

                // Update New Player Properties To PhotonNetworkServer.
                PhotonNetwork.LocalPlayer.SetCustomProperties(UpdateReady_Properties);
            }
            else
            {
                Debug.LogError("Can't Set 'Ready' Properties.");
                return;
            }
        }

        public void StartGame_Function()
        {
            // Wait For InGame Scene.
        }

        public void BackToMainMenu()
        {
            // LeaveRoom To Go BackToMainMenu Scene.
            PhotonNetwork.LeaveRoom();
        }

        #endregion

        #region Local Function

        /// <summary>
        /// This Method is Process With Button , if User is Ower Room Convert Ready Button To StartGame Button Function if not Ower Room Convert To Ready Functon.
        /// </summary>
        /// <param name="isMasterClient">Import Ower Room Status.</param>
        private void ChangeReadyOrStart_Function(bool isMasterClient)
        {
            Button button = readyOrStartGame_Interactive;

            // Player is Ower Room.
            if (isMasterClient)
            {
                // Remove All Method in Ready Function.
                button.onClick.RemoveAllListeners();

                // Replace Ready Function Change Start Game Function.
                button.onClick.AddListener(StartGame_Function);

                Debug.Log("Button Change To StartGame");
            }
            else // Player is not Ower Room.
            {
                // Remove All Method in StartGame Function.
                button.onClick.RemoveAllListeners();

                // Replace StartGame Function Change Ready Function.
                button.onClick.AddListener(Ready_Function);

                Debug.Log("Button Change To Ready");
            }
        }

        /// <summary>
        /// This Method is Control Interctable ReadyORStartGame Button.
        /// </summary>
        private void ReadyOrStart_Interctable()
        {
            // Get CurrentRoom Data.
            Room CurrentRoom = UpdateRoom();
 
            if (Lobbys.GetIsMaster_Client)
            {
                bool AllPlayerIsReady = true;

                // Don't Process  if All Player in Current Room Have One Player Alone.
                if (CurrentRoom.PlayerCount <= 1)
                {
                    // Set Button is Not Interactive With User.
                    readyOrStartGame_Interactive.interactable = false;
                }

                // Loop All Player In Current Room And Find SomePlayer is Not Ready.
                foreach (Player Status in CurrentRoom.Players.Values.ToList())
                {
                    // Process With Player Is Not Ready.
                    if (!(bool)Status.CustomProperties["Ready"])
                    {
                        // Set Button is Not Interactive With User.
                        AllPlayerIsReady = false;
                        break; // Get out From Loop.
                    }
                }

                // Update Last Result For Interactable Button.
                readyOrStartGame_Interactive.interactable = AllPlayerIsReady;

                ChangeReadyOrStart_Function(Lobbys.GetIsMaster_Client);
            }
            else // User Is Not Ower Player.
            {
                // User Can Interact Ready Button.
                readyOrStartGame_Interactive.interactable = true;
                ChangeReadyOrStart_Function(Lobbys.GetIsMaster_Client);
            }
        }

        #endregion

        #region Local Function

        private Room UpdateRoom()
        {
            return PhotonNetwork.CurrentRoom;
        }

        #endregion

        #region Update UI Function

        private void Update_RoomInfo(UpdateRoomInfo RoomInfo)
        {
            room_name_display.text = RoomInfo.roomName;
            room_password_display.text = RoomInfo.roomPassword.ToString();
            room_playerinroom_display.text = RoomInfo.playerInRoom.ToString();
            room_maxnium_display.text = RoomInfo.playerMaxnium.ToString();

            readyOrStartGame_display.text = RoomInfo.roomower == true ? "StartGame" : "Ready";
        }

        private void Update_GameProperties(UpdateGameProperties GameProperties)
        {
            game_coinstater_display.text = GameProperties.coinStarter.ToString();
            game_coinmaxnium_display.text = GameProperties.coinMaxnium.ToString();
            game_cardstarter_display.text = GameProperties.cardStarter.ToString();
            game_cardmaxnium_display.text = GameProperties.CardMaxnium.ToString();
        }

        private void Update_GameRule(UpdateGameRule GameRule)
        {
            duke_display.text = GameRule.dukeCard.ToString();
            assassin_display.text = GameRule.assassinCard.ToString();
            captain_display.text = GameRule.captainCard.ToString();
            ambassador_display.text = GameRule.ambassdorCard.ToString();
            contessa_display.text = GameRule.contessaCard.ToString();
        }

        #endregion
    }

    #region Update Data Struct

    public struct UpdateRoomInfo
    {
        public string roomName;
        public bool roomPassword;
        public int playerInRoom;
        public int playerMaxnium;
        public bool roomower;
    }

    public struct UpdateGameProperties
    {
        public int coinStarter;
        public int coinMaxnium;
        public int cardStarter;
        public int CardMaxnium;
    }

    public struct UpdateGameRule
    {
        public int dukeCard;
        public int assassinCard;
        public int captainCard;
        public int ambassdorCard;
        public int contessaCard;
    }

    #endregion
}