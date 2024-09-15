using System;
using Photon.Pun;
using UnityEngine;
using Photon.Realtime;
using System.Threading.Tasks;
using System.Collections.Generic;
using Coup_Mobile.InGame.GameManager.ReportData;
using Coup_Mobile.InGame.PlayerData;
using System.Linq;
using ExitGames.Client.Photon;

namespace Coup_Mobile.InGame.GameManager.GameState
{
    public class SetupPlayerProperties_GameState : GameState, IGameState_Strategy, IOtherPlayerResponsive_Oberver
    {
        private GameStateManager gameStateManager;

        private bool settingAllPlayerComplate = false;
        private OtherPlayerResponsive_StarterState starter_Data;

        public SetupPlayerProperties_GameState(GameStateManager gameStateManager) : base(gameStateManager)
        {
            Debug.Log($"{this.GetType().Name} Created.");
            this.gameStateManager = gameStateManager;

            // Register Network Observer.
            gameStateManager.Attach(this);
        }

        /// <summary>
        /// using IGameState_Strategy interface method.
        /// Initialize main process setupPlayerProperties state. 
        /// </summary>
        /// <returns>StateReport Data</returns>
        public async Task<object> ProcessGameState(object PacketData)
        {
            GameState_List EndPoint_Path = GameState_List.Setup_Player_Properties;

            try
            {
                bool isOnline = GameManager.isOnline;
                GameState_Report? ReturnResult = null;

                if (isOnline)
                {
                    ReturnResult = await OnProcess_Online();
                }
                else
                {
                    ReturnResult = await OnProcess_Offline();
                }

                if (!ReturnResult.HasValue)
                {
                    throw new NullReferenceException("GameState Report is null");
                }

                return ReturnResult;
                // Report the game state with the option to proceed to the next state.
                //return new GameState_Report(EndPoint_Path, null, false, "Only Test SetUp Player Properties.");
            }
            catch (NullReferenceException nre)
            {
                return new GameState_Report(EndPoint_Path, null, false, nre.Message);
            }
            catch (Exception ex)
            {
                // Report the game state with the option to proceed to the next state.
                return new GameState_Report(EndPoint_Path, null, false, ex.Message);
            }
        }

        #region Game Network Optional

        private async Task<GameState_Report> OnProcess_Online()
        {
            GameState_List EndPoint_Path = GameState_List.Setup_Player_Properties;

            try
            {
                var PlayerNetwork = PhotonNetwork.IsMasterClient;
                var PlayerInGame = PhotonNetwork.CurrentRoom.Players;

                // Process With Host Game.
                if (PlayerNetwork)
                {
                    var EventPath = GameManager_Event.GameResourceManager;
                    object EndPoint = ResourceManager_List.GetRandomPlayerCard;
                    var AllCharacter_Resource = new List<string[]>();

                    // Get Random Card for Owner and another player.
                    foreach (var kvp in PlayerInGame)
                    {
                        GameResource_Result StarterCardAnoumt = (GameResource_Result)Request_Event(EventPath, EndPoint, null);

                        AllCharacter_Resource.Add((string[])StarterCardAnoumt.return_Data);
                    }

                    EndPoint = ResourceManager_List.GetRandomPlayerCoin;
                    List<int> AllCoin_Resource = new List<int>();

                    // Get amount coin starter for Owner and another player.
                    foreach (var kvp in PlayerInGame)
                    {
                        GameResource_Result StarterCoinAnoumt = (GameResource_Result)Request_Event(EventPath, EndPoint, null);

                        AllCoin_Resource.Add((int)StarterCoinAnoumt.return_Data);
                    }

                    // Update Resource Properties To Another Player.
                    EventPath = GameManager_Event.GameNetworkManager;
                    EndPoint = GameNetworkManager_List.ShareResourceSetting;
                    int IndexPlayer = 0;

                    // Share a random character and coin with all players.
                    foreach (var vkp in PlayerInGame)
                    {
                        Player player = vkp.Value;

                        // Setting the network packet. 
                        var Setting_PlayerProperties = new OtherPlayerResponsive_StarterState
                        {
                            GetSetOnContineState = true,
                            GetSetCharacter_Setting = AllCharacter_Resource[IndexPlayer],
                            GetSetCoin_Setting = AllCoin_Resource[IndexPlayer],
                        };

                        // Convert the network packet to JSON format.
                        string SettingPP_Json = NetworkRequestment_ConvertToJson(Setting_PlayerProperties);

                        // Create the packet header. 
                        var Network_Request = new GameNetwork_Requestment
                        {
                            // Target player selection.
                            playerSelection = player,

                            // Send the network packet in JSON format.
                            packetData = SettingPP_Json,
                        };

                        // Send the network packet header to the GameNetworkManager.
                        // And share a character and coin with each player via RPC.
                        // Return status of the RPC.
                        var NetworkRequestment = (GameNetworkManager_Return)Request_Event(EventPath, EndPoint, Network_Request);

                        var NR_Result = NetworkRequestment.requestCommand_Reult;

                        // Check if the network packets were not successfully sent to each player.
                        if (!NR_Result)
                        {
                            Debug.LogError("Can't request network.");
                            return new GameState_Report(EndPoint_Path, null, false, "Can't request network.");
                        }

                        // Loop through and send the network packet to the next player.
                        IndexPlayer++;
                    }

                    // Wait for a notification that the network is updated. 
                    int Timer = 0;
                    int TimeCount = GameManager.NETWORK_TIMEOUT;

                    while (!settingAllPlayerComplate)
                    {
                        await Task.Delay(1000);

                        Timer++;

                        if (Timer >= TimeCount)
                        {
                            return new GameState_Report(EndPoint_Path, null, false, "Wait for Host Time Out.");
                        }
                    }
                    // Load Character and Coin from the server.
                    string[] CharacterSetting = starter_Data.GetSetCharacter_Setting;
                    int CoinSetting = starter_Data.GetSetCoin_Setting;

                    EventPath = GameManager_Event.PlayerManager;
                    EndPoint = PlayerManager_List.Get_SelectionPlayerData;
                    object PacketData = PhotonNetwork.LocalPlayer.NickName;

                    Player_Data LocalPlayer_Data = (Player_Data)((PlayerManager_Return)Request_Event(EventPath, EndPoint, PacketData)).return_Data;

                    if (LocalPlayer_Data.Equals(typeof(Player_Data)))
                    {
                        return new GameState_Report(EndPoint_Path, null, false, "Get Player Data is fail.");
                    }

                    LocalPlayer_Data.usedCharacter = CharacterSetting;
                    LocalPlayer_Data.usedCoin = CoinSetting;

                    EventPath = GameManager_Event.PlayerManager;
                    EndPoint = PlayerManager_List.Update_PlayerData;
                    PacketData = LocalPlayer_Data;

                    var UpdatePlayerResult = (PlayerManager_Return)Request_Event(EventPath, EndPoint, LocalPlayer_Data);

                    if (!(bool)UpdatePlayerResult.return_Data)
                    {
                        return new GameState_Report(EndPoint_Path, null, false, "Can't Update Player Data.");
                    }

                    // Update player resource.
                    SetUp_PlayerResource(CharacterSetting, CoinSetting);

                }
                // Wait for the host process to complete and then get the resource from the server.
                else
                {
                    int Timer = 0;
                    int TimeCount = GameManager.NETWORK_TIMEOUT;

                    while (!settingAllPlayerComplate)
                    {
                        await Task.Delay(1000);

                        Timer++;

                        if (Timer >= TimeCount)
                        {
                            return new GameState_Report(EndPoint_Path, null, false, "Wait for Host Time Out.");
                        }
                    }

                    // Wait for a notification that the network is updated. 
                    string[] CharacterSetting = starter_Data.GetSetCharacter_Setting;
                    int CoinSetting = starter_Data.GetSetCoin_Setting;

                    var EventPath = GameManager_Event.PlayerManager;
                    object EndPoint = PlayerManager_List.Get_SelectionPlayerData;
                    object PacketData = PhotonNetwork.LocalPlayer.NickName;

                    Player_Data LocalPlayer_Data = (Player_Data)((PlayerManager_Return)Request_Event(EventPath, EndPoint, PacketData)).return_Data;

                    if (LocalPlayer_Data.Equals(typeof(Player_Data)))
                    {
                        return new GameState_Report(EndPoint_Path, null, false, "Get Player Data is fail.");
                    }

                    LocalPlayer_Data.usedCharacter = CharacterSetting;
                    LocalPlayer_Data.usedCoin = CoinSetting;

                    EventPath = GameManager_Event.PlayerManager;
                    EndPoint = PlayerManager_List.Update_PlayerData;
                    PacketData = LocalPlayer_Data;

                    var UpdatePlayerResult = (PlayerManager_Return)Request_Event(EventPath, EndPoint, LocalPlayer_Data);

                    if (!(bool)UpdatePlayerResult.return_Data)
                    {
                        return new GameState_Report(EndPoint_Path, null, false, "Can't Update Player Data.");
                    }

                    // Update player resource.
                    SetUp_PlayerResource(CharacterSetting, CoinSetting);
                }

                return new GameState_Report(EndPoint_Path, null, true, null);
            }
            catch (Exception ex)
            {
                return new GameState_Report(EndPoint_Path, null, false, $"SPP_GameState Exception = {ex}");
            }
        }

        private async Task<GameState_Report> OnProcess_Offline()
        {
            GameState_List EndPoint_Path = GameState_List.Setup_Player_Properties;

            try
            {
                GameManager_Event EventPath = GameManager_Event.PlayerManager;
                object EndPoint = PlayerManager_List.Get_AllPlayerData;

                var PlayerInfo = (PlayerManager_Return)Request_Event(EventPath, EndPoint, null);

                List<Player_Data> PlayerInGame = ((Dictionary<int, Player_Data>)PlayerInfo.return_Data).Values.ToList();
                var AllCharacter_Resource = new List<string[]>();

                EventPath = GameManager_Event.GameResourceManager;
                EndPoint = ResourceManager_List.GetRandomPlayerCard;

                // Get Random Card for Owner and another player.
                foreach (var kvp in PlayerInGame)
                {
                    GameResource_Result StarterCardAnoumt = (GameResource_Result)Request_Event(EventPath, EndPoint, null);

                    AllCharacter_Resource.Add((string[])StarterCardAnoumt.return_Data);
                }

                EndPoint = ResourceManager_List.GetRandomPlayerCoin;
                List<int> AllCoin_Resource = new List<int>();

                // Get amount coin starter for Owner and another player.
                foreach (var kvp in PlayerInGame)
                {
                    GameResource_Result StarterCoinAnoumt = (GameResource_Result)Request_Event(EventPath, EndPoint, null);

                    AllCoin_Resource.Add((int)StarterCoinAnoumt.return_Data);
                }

                // Update a character and coin with each player
                EventPath = GameManager_Event.PlayerManager;
                EndPoint = PlayerManager_List.Update_PlayerData;

                int UpdateIndex = 0;

                foreach (var updateResoucre in PlayerInGame)
                {
                    string[] Character = AllCharacter_Resource[UpdateIndex];
                    int Coin = AllCoin_Resource[UpdateIndex];

                    Player_Data updatePlayer = updateResoucre;

                    updatePlayer.usedCharacter = Character;
                    updatePlayer.usedCoin = Coin;

                    PlayerManager_Return SettingResoucre = (PlayerManager_Return)Request_Event(EventPath, EndPoint, updatePlayer);
                }
                await Task.Delay(0);
                return new GameState_Report(EndPoint_Path, null, true, null);
            }
            catch (Exception ex)
            {
                return new GameState_Report(EndPoint_Path, null, false, ex.Message);
            }
        }

        #endregion

        #region Local Function

        /// <summary>
        /// Update the resource in the player properties.
        /// </summary>
        /// <param name="setupCharacter">Character names in Array format </param>
        /// <param name="setupCoin">Coin values in integer format</param>
        private void SetUp_PlayerResource(string[] setupCharacter, int setupCoin)
        {
            // Create a Hashtable using Photon. 
            var SetUp_Properties = new Hashtable
            {
                {"PlayerCard" , setupCharacter},
                {"PlayerCoin" , setupCoin}
            };

            // Update player Properties.
            PhotonNetwork.LocalPlayer.SetCustomProperties(SetUp_Properties);
        }

        #endregion

        #region  Update Form Network

        public void UpdateFormNetwork(object PacketData)
        {
            if (PacketData is string PlayerReponsive_Json)
            {
                try
                {
                    // Try convert jsonString from networkResponsive.
                    // process "Get Random Card And Coin From Server" topic.
                    var starter_Data = JsonUtility.FromJson<OtherPlayerResponsive_StarterState>(PlayerReponsive_Json);

                    // Check 'Starter Data' is null or not null.
                    if (starter_Data.Equals(typeof(OtherPlayerResponsive_StarterState)))
                    {
                        Debug.LogError("Network StarterState is Null");
                        return;
                    }

                    settingAllPlayerComplate = starter_Data.GetSetOnContineState;

                    // Set the character and coin from the server in a global variable.
                    this.starter_Data = starter_Data;
                }
                catch (InvalidCastException ICE)
                {
                    Debug.LogErrorFormat($"Network Update SetupPlayerProperties InvalidCastExcetion = {ICE}");
                    return;
                }
                catch (Exception Ex)
                {
                    Debug.LogErrorFormat($"Network Update SetupPlayerProperties Exception = {Ex}");
                    return;
                }
            }
        }

        #endregion
    }


}