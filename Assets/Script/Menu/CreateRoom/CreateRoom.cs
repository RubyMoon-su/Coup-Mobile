using System;
using Photon.Pun;
using UnityEngine;
using NetworkControl;
using UnityEngine.UI;
using Photon.Realtime;
using EventBus_System;
using ExitGames.Client.Photon;
using Coup_Mobile.Changescene;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace Coup_Mobile.Menu
{
    public class CreateRoom : MonoBehaviourPun, INetworkObserver 
    {
        [SerializeField] private InputField input_name, input_password, input_maxplayer, input_username;
        [SerializeField] private Button enter_button;
        [SerializeField] private Text text_exception;

        private Network_Observer_Notification networkobserver;

        #region  Starter Function And Ending Function
        public void Awake()
        {
            networkobserver = GameObject.Find("PhotonNetwork").gameObject.GetComponent<Network_Observer_Notification>();

            networkobserver.Attach_Network_Notification(this);

            if (!PhotonNetwork.InLobby) PhotonNetwork.JoinLobby();

        }

        public void OnDestroy()
        {
            Debug.Log("OnDestory CreateRoom");

            networkobserver.Detach_Network_Notification(this);
        }

        #endregion

        #region  Button Finction

        public void Check_ButtonInteractable()
        {
            bool Interactable_Button = true;

            // Check RoomName.
            string RoomName = input_name.text;

            if (!CheckCharacterRegex(RoomName))
            {
                Interactable_Button = false;
            }
            else
            {
                if (RoomName.Length < 4)
                {
                    Interactable_Button = false;
                }
            }

            string Password = input_password.text;

            if (Password.Length > 0)
            {
                if (!CheckCharacterRegex(Password)) Interactable_Button = false;
            }          

            string MaxPlayer = input_maxplayer.text;

            if (!CheckCharacterRegex_Number(MaxPlayer))
            {
                Interactable_Button = false;
            }
            else
            {
                int MaxPlayerToInt = Convert.ToInt32(MaxPlayer);

                if (MaxPlayerToInt <= 0 || MaxPlayerToInt > 6)
                {
                    Interactable_Button = false;
                }
            }

            string Username = input_username.text;

            if (!CheckCharacterRegex(Username))
            {
                Interactable_Button = false;
            }

            enter_button.interactable = Interactable_Button;
        }
        public void Enter_Function()
        {
            int? Password = input_password.text.Length > 0 ? Convert.ToInt32(input_password.text) : null;

            if (Password.HasValue) CreateGameRoom(Password.Value , input_username.text);
            else CreateGameRoom(null , input_username.text);

        }

         public void BackToMainMenu_Function()
        {
            PhotonNetwork.LeaveLobby();
        }

        #endregion

        #region  Local Function

        private void CreateGameRoom(int? Password , string RoomOwerName)
        {
            if (PhotonNetwork.CountOfRooms == 10)
            {
                Debug.LogWarning("Can't Create Room, Server is Full.");

                return;
            }

            RoomOptions newRoomOption = null;

            string RoomProperties_StarterInfo = JsonUtility.ToJson(new CustomRoomProperties_StarterInfo
            {
                PerCard  = 2,
                GoldStarter = 2,
                TotalCard = 15,
                TotalGold = 50,
            });

            string RoomProperties_CardInfo = JsonUtility.ToJson(new CustomRoomProperties_CardInfo
            {
                Duke = 3,
                Assassin = 3,
                Captain = 3,
                Ambassador = 3,
                Contessa = 3,
            });

            string RoomProperties_PlayerSlotInfo = JsonUtility.ToJson(new CustomRoomProperties_PlayerSlotInfo
            {
                Player_Slots = new List<Lobby_Slot>(),
                Player_Status = new Dictionary<Player, bool>(),
            });          

            if (Password.HasValue)
            {

                newRoomOption = new RoomOptions
                {
                    MaxPlayers = Convert.ToInt32(input_maxplayer.text),
                    CustomRoomProperties = new Hashtable 
                    { 
                        { "Password", Convert.ToInt32(Password.Value) } 
                        , { "Room Status" , true }
                        , { "StartInfo" , RoomProperties_StarterInfo}
                        , { "CardInfo" , RoomProperties_CardInfo}
                        , { "PlayerSlotInfo" , RoomProperties_PlayerSlotInfo}
                        
                    },
                    CustomRoomPropertiesForLobby = new string[] 
                    { 
                        "Password"
                        , "Lobby" 
                        , "Room Status" 
                        , "StartInfo" 
                        , "CardInfo" 
                        , "PlayerSlotInfo" 
                    }
                };
            }
            else
            {
                newRoomOption = new RoomOptions
                {
                    MaxPlayers = Convert.ToInt32(input_maxplayer.text),
                    CustomRoomProperties = new Hashtable 
                    {
                        {"Room Status" , false}
                        , { "StartInfo" , RoomProperties_StarterInfo}
                        , { "CardInfo" , RoomProperties_CardInfo}
                        , { "PlayerSlotInfo" , RoomProperties_PlayerSlotInfo}

                    },
                    CustomRoomPropertiesForLobby = new string[] 
                    { 
                        "Lobby" 
                        , "Password" 
                        , "Room Status" 
                        , "StartInfo" 
                        , "CardInfo" 
                        , "PlayerSlotInfo"
                    }
                };
            }

            if (newRoomOption.Equals(typeof(RoomOptions)))
            {
                Debug.LogError("No Setting RoomOption.");
                return;
            }

            PhotonNetwork.NickName = input_username.text;

            PhotonNetwork.JoinOrCreateRoom(input_name.text, newRoomOption, TypedLobby.Default);
        }

        private bool CheckCharacterRegex(string CharacterInput)
        {
            string ResultCharacter = CharacterInput;
            Regex regex = new Regex("^[a-zA-Z0-9]*$");

            if (ResultCharacter.Length > 0 && !string.IsNullOrEmpty(ResultCharacter) && !string.IsNullOrWhiteSpace(ResultCharacter))
            {
                if (regex.IsMatch(ResultCharacter))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private bool CheckCharacterRegex_Number(string CharacterInput)
        {
            string ResultCharacter = CharacterInput;
            Regex regex = new Regex("^[0-9]*$");

            if (ResultCharacter.Length > 0 && int.TryParse(ResultCharacter, out int number))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region  Observer Network Update.
        public void NetworkUpdate(object Network_Notification, Network_State State_Notification)
        {
            switch (State_Notification)
            {
                case Network_State.OnJoinedLobby:

                    Debug.Log("Join Lobby");
                    break;
                case Network_State.OnCreatedRoom:

                    Debug.Log("Create Room");
                    break;
                case Network_State.OnJoinedRoom:

                    EventBus_SceneManager<IEvent>.RaiseSceneManager_Event(ChangeScene.Lobby, true, null);

                    break;
                case Network_State.OnLeftLobby:

                    Debug.Log("LeftLobby");

                    EventBus_SceneManager<IEvent>.RaiseSceneManager_Event(ChangeScene.Mainmenu, false, null);

                    break;
            }
        }

        #endregion
    }

    #region CustomRoom Setting

    [Serializable]
    public struct CustomRoomProperties_StarterInfo
    {
        // Common Setting.
        public int PerCard , GoldStarter , TotalGold , TotalCard;
    }

    [Serializable]
    public struct CustomRoomProperties_CardInfo
    {
        public int Duke , Assassin , Captain , Ambassador , Contessa;
    }

    [Serializable]
    public class CustomRoomProperties_PlayerSlotInfo
    {
        public List<Lobby_Slot> Player_Slots;
        public Dictionary<Player , bool> Player_Status;
    }

    #endregion

}