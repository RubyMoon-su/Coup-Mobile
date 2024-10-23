using System;
using System.Linq;
using UnityEngine;
using Coup_Mobile.EventBus;
using System.Collections.Generic;
using Coup_Mobile.InGame.PlayerData;
using Coup_Mobile.Menu.GameSetting_Data;
using Coup_Mobile.InGame.GameManager.ReportData;

namespace Coup_Mobile.InGame.GameManager
{
    public enum PlayerManager_List
    {
        // PlayerManager Control
        Add_Player,
        Leave_Player,
        Update_PlayerData,
        Get_AllPlayerData,
        Get_AllPlayerWithout_MineData,
        Get_SelectionPlayerData,

        // Contine Player Sort.
        Update_Wave,

        // Fix Player Sort.
        Change_FirstPlayerSort,
        Change_NextPlayerSort,
        Change_PreviousPlayerSort,


        // Common Getter
        Get_FirstPlayerSort,
        Get_NextPlayerSort,
        Get_PreviousPlayerSort,

        //GetInstall Complate.
        Get_Install_Complate,

    }

    public class PlayerManager
    {

        #region Universal Variable Global Field

        // Player In Game Collection.
        // Process Wave With Numeric.
        private readonly List<Player_Data> allPlayer_InGame = new List<Player_Data>();
        private readonly Dictionary<int, Player_Data> allPlayerSort = new Dictionary<int, Player_Data>();
        private Player_Data myPlayer_Data;

        // Player Position In Game Wave.
        private Player_Data current_PlayerTurn;
        private Player_Data next_PlayerTurn;
        private Player_Data previous_PlayerTurn;

        // Component Manager.
        private GameManager gameManager;

        // Player Manager Control Process Successful Status.
        private static bool install_Complate = false;

        #endregion

        #region Online Variable Global Field



        #endregion

        #region Offline Variabel Global Field



        #endregion



        public PlayerManager(GameManager gameManager, GameSetting gameSetting)
        {
            Debug.Log("PlayerManager Created.");

            this.gameManager = gameManager;
            allPlayer_InGame = gameSetting.allPlayerData;

            Setup_PlayerInManager();

        }

        #region PlayerManager Control

        public PlayerManager_Return PlayerManager_Control(GameManager_Data Request_Data)
        {
            PlayerManager_Return Return_PlayerManager = new PlayerManager_Return();
            object ReturnData = null;

            if (Request_Data.EndPoint is PlayerManager_List PM_List)
            {
                switch (PM_List)
                {
                    case PlayerManager_List.Add_Player:

                        // No Player Enter Room Between Playing Game.

                        break;
                    case PlayerManager_List.Leave_Player:

                        if (Request_Data.PacketData is Player_Data Player_Leave)
                        {
                            bool PlayerLeave_Result = PlayerLeaveGame(Player_Leave);

                            ReturnData = PlayerLeave_Result;
                        }

                        break;
                    case PlayerManager_List.Update_PlayerData:

                        if (Request_Data.PacketData is Player_Data UpdatePlayer)
                        {
                            bool UpdatePlayer_Result = Update_PlayerData(UpdatePlayer);

                            ReturnData = UpdatePlayer_Result;
                        }

                        break;
                    case PlayerManager_List.Change_FirstPlayerSort:

                        if (Request_Data.PacketData is Player_Data Change_FirstPlayer)
                        {

                        }

                        break;
                    case PlayerManager_List.Change_NextPlayerSort:

                        if (Request_Data.PacketData is Player_Data Change_NextPlayer)
                        {

                        }

                        break;
                    case PlayerManager_List.Change_PreviousPlayerSort:

                        if (Request_Data.PacketData is Player_Data Change_PreviousPlayer)
                        {

                        }

                        break;
                    case PlayerManager_List.Get_NextPlayerSort: ReturnData = next_PlayerTurn; break;
                    case PlayerManager_List.Get_FirstPlayerSort: ReturnData = current_PlayerTurn; break;
                    case PlayerManager_List.Get_PreviousPlayerSort: ReturnData = previous_PlayerTurn; break;
                    case PlayerManager_List.Get_AllPlayerData: ReturnData = allPlayerSort; break;
                    case PlayerManager_List.Get_AllPlayerWithout_MineData:
                        foreach (var playerName in allPlayerSort.Values.ToList())
                        {

                        }
                        break;
                    case PlayerManager_List.Get_SelectionPlayerData:

                        if (Request_Data.PacketData != null && Request_Data.PacketData is not string)
                        {
                            Debug.LogError($"{PM_List} PacketData is {Request_Data.PacketData} || PacketData is not String{Request_Data.PacketData is not string}");
                            break;
                        }

                        foreach (var PlayerCard in allPlayerSort)
                        {
                            if (PlayerCard.Value.playerName == (string)Request_Data.PacketData)
                            {
                                ReturnData = PlayerCard.Value;
                            }
                        }

                        break;
                    case PlayerManager_List.Get_Install_Complate: ReturnData = install_Complate; break;
                    default:
                        Debug.LogError($"PlayerManager No PM_List {PM_List}");
                        break;
                }
            }

            if (ReturnData != null)
            {
                Return_PlayerManager = new PlayerManager_Return
                {
                    requestCommand_Reult = true,
                    requestType = (PlayerManager_List)Request_Data.EndPoint,
                    return_Data = ReturnData,
                };

                return Return_PlayerManager;
            }
            else
            {
                Return_PlayerManager = new PlayerManager_Return
                {
                    requestCommand_Reult = false,
                    requestType = (PlayerManager_List)Request_Data.EndPoint,
                    return_Data = null,
                };

                return Return_PlayerManager;
            }

        }

        #endregion

        #region Local Function

        /// <summary>
        /// Sets up the players in the game manager by randomly assigning a unique order to each player.
        /// It ensures that all players have a distinct position and determines the current, next, and previous players.
        /// If the setup is successful, it returns true; otherwise, it returns false if an error occurs during the setup process.
        /// </summary>
        /// <returns>Returns true if the player setup is successful, false otherwise.</returns>
        private bool Setup_PlayerInManager()
        {
            try
            {
                // Random Player System.
                int MaxPlayer = allPlayer_InGame.Count;
                HashSet<int> usedSort = new HashSet<int>();

                // Random Player Sort.
                for (int i = 0; i < allPlayer_InGame.Count; i++)
                {
                    Player_Data player = allPlayer_InGame[i];

                    int RandomSort;

                    do
                    {
                        RandomSort = UnityEngine.Random.Range(1, MaxPlayer + 1);
                    }
                    while (usedSort.Contains(RandomSort));

                    usedSort.Add(RandomSort);
                    allPlayerSort.Add(RandomSort, player);
                }

                // Setup Field player Sort.

                // First Player Starter
                allPlayerSort.TryGetValue(1, out Player_Data FirstPlayer);
                ChangePlayerWave_Control("Current Position", FirstPlayer);

                // Next Player.
                allPlayerSort.TryGetValue(2, out Player_Data NextPlayer);
                ChangePlayerWave_Control("Next Position", NextPlayer);

                // Previous_Player.
                allPlayerSort.TryGetValue(usedSort.Max(), out Player_Data PreviousPlayer);
                ChangePlayerWave_Control("Previous Position", PreviousPlayer);

                // Display.
                Displayer_AllPlayerWave();

                install_Complate = true;
                // Process Complate.
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
                // Not Complate.
                return false;
            }
        }

        /// <summary>
        /// This Method is Remove Player From System.
        /// if Player Disconnect Server.
        /// </summary>
        /// <param name="LeaveGame"></param>
        /// <returns></returns>
        private bool PlayerLeaveGame(Player_Data LeaveGame)
        {
            try
            {
                if (!allPlayerSort.ContainsValue(LeaveGame))
                {
                    Debug.LogError($"No Player {LeaveGame.GetPlayerName} In GameSort.");
                    return false;
                }

                int LeaveKey = -1;

                // Find Player Disconnect in System Collection.
                foreach (var kvp in allPlayerSort)
                {
                    if (kvp.Value.GetPlayerName == LeaveGame.GetPlayerName)
                    {
                        LeaveKey = kvp.Key;
                        break;
                    }
                }

                if (LeaveKey == -1)
                {
                    Debug.LogError("No Player has Remove From System.");
                    return false;
                }

                allPlayerSort.Remove(LeaveKey);

                // ReSort Player In List.
                var Backup_Data = allPlayerSort.ToDictionary(entry => entry.Key, entry => entry.Value); ;
                allPlayerSort.Clear();

                Dictionary<int, Player_Data> NewPlayerData = new Dictionary<int, Player_Data>();

                for (int i = LeaveKey; i <= Backup_Data.Count; i++)
                {
                    if (allPlayerSort.ContainsKey(i + 1))
                    {
                        allPlayer_InGame[i] = allPlayer_InGame[i + 1];
                    }
                }

                allPlayerSort.Remove(allPlayerSort.Count);

                // Adjust current player turn if needed.
                if (LeaveKey <= allPlayerSort.Keys.Max() && current_PlayerTurn.GetPlayerName == allPlayerSort[LeaveKey].GetPlayerName)
                {
                    current_PlayerTurn = allPlayerSort.ContainsKey(LeaveKey) ? allPlayerSort[LeaveKey] : allPlayerSort[allPlayerSort.Keys.Min()];
                }

                // Adjust next player turn if needed.
                if (LeaveKey <= allPlayerSort.Keys.Max() && next_PlayerTurn.GetPlayerName == allPlayerSort[LeaveKey].GetPlayerName)
                {
                    next_PlayerTurn = allPlayerSort.ContainsKey(LeaveKey + 1) ? allPlayerSort[LeaveKey + 1] : allPlayerSort[allPlayerSort.Keys.Min()];
                }

                // Adjust previous player turn if needed.
                if (LeaveKey <= allPlayerSort.Keys.Max() && previous_PlayerTurn.GetPlayerName == allPlayerSort[LeaveKey].GetPlayerName)
                {
                    previous_PlayerTurn = allPlayerSort.ContainsKey(LeaveKey - 1) ? allPlayerSort[LeaveKey - 1] : allPlayerSort[allPlayerSort.Keys.Max()];
                }


                /* // Check Player In Game Wave.
                 if (LeaveGame.GetPlayerName == current_PlayerTurn.GetPlayerName)
                 {
                     if (++PlayerSort > allPlayerSort.Keys.Max())
                     {
                         PlayerSort = allPlayerSort.Keys.Min();
                     }

                     current_PlayerTurn = allPlayerSort[PlayerSort];
                 }

                 if (LeaveGame.GetPlayerName == next_PlayerTurn.GetPlayerName || current_PlayerTurn.GetPlayerName == next_PlayerTurn.GetPlayerName)
                 {
                     if (++PlayerSort > allPlayerSort.Keys.Max())
                     {
                         PlayerSort = allPlayerSort.Keys.Min();
                     }

                     next_PlayerTurn = allPlayer_InGame[PlayerSort];
                 }

                 if (LeaveGame.GetPlayerName == previous_PlayerTurn.GetPlayerName || next_PlayerTurn.GetPlayerName == previous_PlayerTurn.GetPlayerName)
                 {
                     if (--PlayerSort < allPlayerSort.Keys.Min())
                     {
                         PlayerSort = allPlayerSort.Keys.Max();
                     }

                     previous_PlayerTurn = allPlayer_InGame[PlayerSort];
                 }
                 */

                // Remove Player in Collection Complate.
                return true;
            }
            catch
            {
                // Remove Player in Collection Not Complate.
                return false;
            }
        }

        private void ChangePlayerWave_Control(string ChangePosition, Player_Data NewPlayerPosition)
        {

            Player_Data Old_Player;

            switch (ChangePosition)
            {
                case "Current Position":

                    Old_Player = current_PlayerTurn;

                    current_PlayerTurn = NewPlayerPosition;

                    break;
                case "Next Position":

                    Old_Player = next_PlayerTurn;

                    next_PlayerTurn = NewPlayerPosition;

                    break;
                case "Previous Position":

                    Old_Player = previous_PlayerTurn;

                    previous_PlayerTurn = NewPlayerPosition;

                    break;
                default:
                    Debug.LogError($"{ChangePosition} Is Not Correct.");
                    return;
            }

            Debug.Log($"Game System Player {Old_Player.GetPlayerName} Change Wave To {ChangePosition}");

        }

        private void Displayer_AllPlayerWave()
        {
            Debug.Log($"First Wave is {current_PlayerTurn.GetPlayerName}");
            Debug.Log($"Secon Wave is {next_PlayerTurn.GetPlayerName}");
            Debug.Log($"Previous Wave is {previous_PlayerTurn.GetPlayerName}");
        }

        private bool Update_PlayerData(Player_Data UpdatePlayer)
        {
            try
            {
                foreach (var SelectPlayer in allPlayerSort)
                {
                    if (SelectPlayer.Value.GetPlayerName == UpdatePlayer.GetPlayerName)
                    {
                        allPlayerSort[SelectPlayer.Key] = UpdatePlayer;

                        return true;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                Debug.LogError("PlayerManager Exception Message 'Update_PlayerData' " + ex);
                return false;
            }
        }

        #endregion

    }

    [Serializable]
    public struct PlayerManager_Return : IInGameEvent
    {
        public bool requestCommand_Reult;
        public PlayerManager_List requestType;
        public object return_Data;
    }
}