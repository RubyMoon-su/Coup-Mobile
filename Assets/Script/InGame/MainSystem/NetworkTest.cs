using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Coup_Mobile.InGame.GameManager;
using Coup_Mobile.Menu.GameSetting_Data;
using ExitGames.Client.Photon;

public class NetworkTest : MonoBehaviourPunCallbacks
{
    public void Start()
    {
        Debug.Log("Start Test");
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected Test");
        PhotonNetwork.JoinOrCreateRoom("Test Room" , new RoomOptions() , TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Join Test");
        CharacterSetting Character = new CharacterSetting();
        Character.SetDefalult_Setting();

        PropertiesSetting Properties = new PropertiesSetting();
        Properties.SetDefalult_Setting();

        AdvanceSetting Advance = new AdvanceSetting();
        Advance.SetDefalult_Setting();

        GameSetting NewGame = new GameSetting()
        {
            gameMode = GameMode.NormalGame,
            allPlayerData = new List<Coup_Mobile.InGame.PlayerData.Player_Data>
            {
                new Coup_Mobile.InGame.PlayerData.Player_Data(){playerName = "Jo"},
                new Coup_Mobile.InGame.PlayerData.Player_Data(){playerName = "Ice"},
                new Coup_Mobile.InGame.PlayerData.Player_Data(){playerName = "Boom"},
                new Coup_Mobile.InGame.PlayerData.Player_Data(){playerName = "Hee"},
            },
            allPlayerInGame = 4,
            characterSetting = Character,
            propertiesSetting = Properties,
            advanceSetting = Advance,
        };

        string Gamesetting_Json = JsonUtility.ToJson(NewGame);

        ExitGames.Client.Photon.Hashtable NewProperties = new ExitGames.Client.Photon.Hashtable()
        {
            {"GameSetting" , Gamesetting_Json}
        };

        PhotonNetwork.CurrentRoom.SetCustomProperties(NewProperties);
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        Debug.Log("Update Properties");

        var Test = gameObject.GetComponent<GameManager>();

        Test.OnStarterGameManager();
    }
}
