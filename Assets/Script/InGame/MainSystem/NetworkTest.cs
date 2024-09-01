using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Coup_Mobile.InGame.GameManager;
using Coup_Mobile.Menu.GameSetting_Data;
using Coup_Mobile.InGame.PlayerData;
using System.Threading.Tasks;

public class NetworkTest : MonoBehaviourPunCallbacks
{
    [SerializeField] public bool OnOwner = false;
    [SerializeField] public bool OnStart = false;
    [SerializeField] public string PlayerName = "player";

    public void Start()
    {
        Debug.Log("Start Test");
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected Test");
        if (OnOwner)
        {
            PhotonNetwork.JoinOrCreateRoom("Test Room", new RoomOptions(), TypedLobby.Default);
        }
        else
        {
            PhotonNetwork.JoinRandomRoom();
        }

    }

    public async override void OnJoinedRoom()
    {
        PhotonNetwork.LocalPlayer.NickName = PlayerName;

        if (OnOwner)
        {
            Debug.LogError("OnJoin");
            int Time = 0;
            int max = 30;

            while (!OnStart)
            {
                await Task.Delay(1000);

                Time++;

                Debug.Log("Wait For Player" + Time);

                if (max <= Time)
                {
                    Debug.LogError("Network Time out");
                    return;
                }
            }

            CharacterSetting Character = new CharacterSetting();
            Character.SetDefalult_Setting();

            PropertiesSetting Properties = new PropertiesSetting();
            Properties.SetDefalult_Setting();

            AdvanceSetting Advance = new AdvanceSetting();
            Advance.SetDefalult_Setting();

            var AllPlayerInGame = PhotonNetwork.CurrentRoom.Players;
            List<Player_Data> PlayerInfo = new List<Player_Data>();

            GameSetting NewGame = new GameSetting()
            {
                gameMode = GameMode.NormalGame,

                allPlayerInGame = 4,
                characterSetting = Character,
                propertiesSetting = Properties,
                advanceSetting = Advance,
            };

            foreach (var kvp in AllPlayerInGame)
            {
                Player player = kvp.Value;

                Player_Data NewID = new Player_Data
                {
                    playerName = player.NickName,
                };

                PlayerInfo.Add(NewID);
            }

            NewGame.allPlayerData = PlayerInfo;

            string Gamesetting_Json = JsonUtility.ToJson(NewGame);

            ExitGames.Client.Photon.Hashtable NewProperties = new ExitGames.Client.Photon.Hashtable()
            {
            {"GameSetting" , Gamesetting_Json}
            };

            PhotonNetwork.CurrentRoom.SetCustomProperties(NewProperties);
        }
        else 
        {
            Debug.LogError("OnJoin");
        }
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        Debug.Log("Update Properties");

        var Test = gameObject.GetComponent<GameManager>();

        Test.OnStarterGameManager();
    }
}
