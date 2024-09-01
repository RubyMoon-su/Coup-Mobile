using System;
using Photon.Pun;
using UnityEngine;
using Photon.Realtime;
using System.Threading.Tasks;
using System.Collections.Generic;
using Coup_Mobile.InGame.GameManager.ReportData;

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

                        // Update player resource.
                        SetUp_PlayerResource(CharacterSetting, CoinSetting);

                    }

                    // Report the game state with the option to proceed to the next state.
                    return new GameState_Report(EndPoint_Path, null, false, "Only Test SetUp Player Properties.");
                }
                catch (Exception ex)
                {
                    // Report the game state with the option to proceed to the next state.
                    return new GameState_Report(EndPoint_Path, null, false, ex.Message);
                }
        }

        #region Local Function

        /// <summary>
        /// Update the resource in the player properties.
        /// </summary>
        /// <param name="setupCharacter">Character names in Array format </param>
        /// <param name="setupCoin">Coin values in integer format</param>
        private void SetUp_PlayerResource(string[] setupCharacter, int setupCoin)
        {
            // Create a Hashtable using Photon. 
            var SetUp_Properties = new ExitGames.Client.Photon.Hashtable
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