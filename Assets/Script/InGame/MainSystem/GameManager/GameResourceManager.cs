using System;
using UnityEngine;
using Coup_Mobile.EventBus;
using System.Threading.Tasks;
using System.Collections.Generic;
using Coup_Mobile.Menu.GameSetting_Data;
using Coup_Mobile.InGame.GameManager.ReportData;

namespace Coup_Mobile.InGame.GameManager
{
    #region GameResource EndPoint
    public enum ResourceManager_List
    {
        // Common Getter.
        GetStarterCard,
        GetStarterCoin,
        GetAllCenterCard,
        GetAllCenterCoin,
        GetAllTreasury,
        GetAbilit_CanUse,

        // Player Data And Character Setting.     
        GetAllCharacterNumber,
        GetAllCharacterCard,

        // Action And Cost Getter
        GetActionAndCost,

        // Control Game Resource.
        Update_GameRule,

        // Manager Install System Status.
        GetInstall_Complate,
    }

    #endregion

    public class GameResourceManager
    {
        #region Gobal Field

        // How Much Character Can Play.
        private Dictionary<string /*Character Name*/, int /*Amount Character*/> allCharacter = new Dictionary<string, int>();

        // Cost Pay Action.
        private List<Tuple<string /*Action Name*/, int /*Income Ability*/ , int /*Cost Ability*/>> costAndAction = new List<Tuple<string, int, int>>();

        // All Ability It Can Use.
        private List<string> enable_Ability = new List<string>();

        // Total Card And Coin In Game.
        private int allCoinCenter;
        private int allCardCenter;
        private int allCoinTreasury_Reserve;

        // Start Card And Coin In Name.
        private int coinStarter;
        private int cardStarter;

        // Component.
        private GameManager gameManager;

        // Install Resoucre Complate Status.
        public static bool Install_Complate = false;

        #endregion

        public GameResourceManager(GameManager gameManager, GameSetting gameSetting)
        {
            Debug.Log("GameResourceManager Created");

            this.gameManager = gameManager;

            // Install Resource.
            InstallResource(gameSetting);
        }

        private async void InstallResource(GameSetting gameSetting)
        {
            try
            {
                if (gameSetting.Equals(typeof(GameSetting)))
                {
                    Debug.LogError("GameResource : GameSetting Is NUll.");
                    gameSetting = default;
                    return;
                }

                // All Number Character In Game ,How Much Character Can Play.
                allCharacter = new Dictionary<string, int>
                {
                    {"Duke" , 0},
                    {"Captain" , 0},
                    {"Assassin" , 0},
                    {"Ambassdor" , 0},
                    {"Contessa" , 0},
                };

                await Task.Delay(0);

                Update_GameRule(gameSetting);

                Install_Complate = true;

            }
            catch (Exception ex)
            {
                Install_Complate = false;

                Debug.LogError(ex);
            }
        }

        #region Game Resource Control.

        public GameResource_Result GameResource_Control(GameManager_Data Request_Data)
        {
            GameResource_Result Return_GameResource = new GameResource_Result();
            object Return_Packet = null;

            if (Request_Data.EndPoint is ResourceManager_List RM_List)
            {
                switch (RM_List)
                {
                    // Command Getter.
                    case ResourceManager_List.GetStarterCard: Return_Packet = cardStarter; break;
                    case ResourceManager_List.GetStarterCoin: Return_Packet = coinStarter; break;
                    case ResourceManager_List.GetAllCenterCard: Return_Packet = allCoinCenter; break;
                    case ResourceManager_List.GetAllCenterCoin: Return_Packet = allCardCenter; break;
                    case ResourceManager_List.GetAllTreasury: Return_Packet = allCoinTreasury_Reserve; break;

                    // Characters Setting Getter.
                    case ResourceManager_List.GetAllCharacterNumber: Return_Packet = allCardCenter; break;
                    case ResourceManager_List.GetAllCharacterCard: Return_Packet = allCharacter; break;

                    // System Install System Getter.
                    case ResourceManager_List.GetInstall_Complate: Return_Packet = Install_Complate; break;

                    // Ability Can Use Getter
                    case ResourceManager_List.GetAbilit_CanUse: Return_Packet = enable_Ability; break;

                    // Action And Cost Getter
                    case ResourceManager_List.GetActionAndCost:

                        if (Request_Data.PacketData is string GetAbility)
                        {
                            foreach (var ActionAndCost in costAndAction)
                            {
                                if (ActionAndCost.Item1 == GetAbility)
                                {
                                    // Return Tuple<string , int , int>.
                                    Return_Packet = ActionAndCost;
                                    break;
                                }
                            }
                        }

                        break;
                    case ResourceManager_List.Update_GameRule:

                        if (Request_Data.PacketData is GameSetting NewGameRule)
                        {
                            bool Update_Result = Update_GameRule(NewGameRule);

                            if (!Update_Result)
                            {
                                break;
                            }

                            Return_Packet = Update_Result;
                            break;
                        }

                        break;
                }
            }

            if (Return_Packet != null)
            {
                Return_GameResource = new GameResource_Result
                {
                    return_Data = Return_Packet,
                    requestType = (ResourceManager_List)Request_Data.EndPoint,
                    requestCommand_Reult = true,
                };

                return Return_GameResource;

            }
            else
            {
                Return_GameResource = new GameResource_Result
                {
                    requestCommand_Reult = false,
                    requestType = (ResourceManager_List)Request_Data.EndPoint,
                    return_Data = null,
                };

                return Return_GameResource;
            }


        }

        #endregion

        #region Local Function

        private bool Update_GameRule(GameSetting NewGameRule)
        {
            try
            {
                if (NewGameRule.Equals(typeof(GameSetting)))
                {
                    Debug.LogError("GameResourceManager NewGameRule Is Defult Data.");
                }

                if (!NewGameRule.characterSetting.Equals(typeof(CharacterSetting)))
                {
                    CharacterSetting Character_Data = NewGameRule.characterSetting;

                    allCharacter["Duke"] = Character_Data.duke;
                    allCharacter["Captain"] = Character_Data.captain;
                    allCharacter["Assassin"] = Character_Data.assassins;
                    allCharacter["Ambassdor"] = Character_Data.ambassdor;
                    allCharacter["Contessa"] = Character_Data.contassa;

                    Debug.Log("CharacterSetting Install Complate.");
                }
                else Debug.LogWarning("CharacterSetting Has No Data Install.");


                if (!NewGameRule.propertiesSetting.Equals(typeof(PropertiesSetting)))
                {
                    // Resource In Game.
                    PropertiesSetting GameProperties = NewGameRule.propertiesSetting;

                    allCoinCenter = GameProperties.totalCoin;
                    allCardCenter = GameProperties.totalCard;
                    allCoinTreasury_Reserve = GameProperties.maxnium_TreasuryReserve;
                    coinStarter = GameProperties.strarterCoin;
                    cardStarter = GameProperties.starterCard;

                    costAndAction = new List<Tuple<string, int, int>>
                    {
                        new Tuple<string , int , int>("Income" , GameProperties.income , GameProperties.income_Cost),
                        new Tuple<string , int , int>("ForgetAid" , GameProperties.forgetaid_Income , GameProperties.forgetaid_Cost),
                        new Tuple<string , int , int>("Coup" , GameProperties.coup_Income , GameProperties.coup_Cost),
                        new Tuple<string , int , int>("Duke" , GameProperties.duke_Income , GameProperties.duke_Cost),
                        new Tuple<string , int , int>("Assassin" , GameProperties.assassin_Income , GameProperties.assassin_Cost),
                        new Tuple<string , int , int>("Captain" , GameProperties.captain_Income , GameProperties.captain_Cost),
                        new Tuple<string , int , int>("Ambassdor" , GameProperties.ambassdor_Income , GameProperties.ambassin_Cost),
                        new Tuple<string , int , int>("Contassa" , GameProperties.contassa_Income , GameProperties.contassa_Cost),
                        new Tuple<string , int , int>("Inquisitor" , GameProperties.inquisitor_Income , GameProperties.inquisitor_Cost),
                    };
                }
                else Debug.LogWarning("PropertiesSetting Has No Data Install.");

                // Ability Can Use.
                enable_Ability = NewGameRule.propertiesSetting.allAbility_CanUse;

                return true;
            }
            catch (NullReferenceException ne)
            {
                Debug.LogError("GameResourceManager Null Exception Message 'Update_GameRule' " + ne);
                return false;
            }
            catch (Exception ex)
            {
                Debug.LogError("GameResourceManager Exception Message 'Update_GameRule' " + ex);
                return false;
            }
        }


        #endregion
    }

    [Serializable]
    public struct GameResource_Result : IInGameEvent
    {
        public bool requestCommand_Reult;
        public ResourceManager_List requestType;
        public object return_Data;
    }
}