using System;
using UnityEngine;
using Coup_Mobile.EventBus;
using System.Threading.Tasks;
using System.Collections.Generic;
using Coup_Mobile.Menu.GameSetting_Data;
using Coup_Mobile.InGame.GameManager.ReportData;
using Unity.VisualScripting;
using System.Linq;

namespace Coup_Mobile.InGame.GameManager
{
    #region GameResource EndPoint
    public enum ResourceManager_List
    {
        #region Card And Resource Game
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

        #endregion


        #region  Character Anoumt Card. 

        GetDukeAnoumt,
        GetAssassinAnoumt,
        GetCaptainAnoumt,
        GetAmbassdorAnoumt,
        GetContessaAnoumt,
        GetInquisitorAnoumt,
        GetAllCharacterAnoumt,

        #endregion

        #region Properties Resource 

        GetIncomeCostAndIncome,
        GetForgetAidCostAndIncome,
        GetCoupCostAndIncome,
        GetDukeCostAndIncome,
        GetAssassinCostAndIncome,
        GetCaptainCostAndIncome,
        GetAmbassdorCostAndIncome,
        GetContessaCostAndIncome,
        GetInquisitorCostAndIncome,
        GetAllCostAndIncome,

        #endregion

        #region Advance Resource

        // Timer Resource.
        GetWaitCommandTime,
        GetWaitCounterTime,
        GetWaitVerifyTime,
        GetWaitResultTime,
        GetWaitChangeTeam_Time,
        GetWaitConversionTime,
        GetWaitEmbezzlementTime,
        GetWaitBettwenStateTime,

        #endregion

        #region Setting Game Rule

        Setting_CostAndIncome,
        Setting_CharacterAmount,
        Setting_TimerResource,

        #endregion

        #region Sort Player Resource.

        GetRandomPlayerCard,
        GetRandomPlayerCoin,
        GetRandomCenterCard,

        #endregion

        // Manager Install System Status.
        GetInstall_Complate,
    }

    #endregion

    public class GameResourceManager
    {
        #region Income And Cost Field

        // How Much Character Can Play.
        private Dictionary<string /*Character Name*/, int /*Amount Character*/> allCharacter = new Dictionary<string, int>();

        // Cost Pay Action.
        private List<Tuple<string /*Action Name*/, int /*Income Ability*/ , int /*Cost Ability*/>> costAndAction = new List<Tuple<string, int, int>>();

        private List<string> allCharacter_InCenter = new List<string>();

        #endregion

        #region Coin And Character Anoumt

        // Total Card And Coin In Game.
        private int allCoinCenter;
        private int allCardCenter;
        private int allCoinTreasury_Reserve;

        // Start Card And Coin In Name.
        private int coinStarter;
        private int cardStarter;

        #endregion

        #region Time Resource

        private int waitCommandTime;
        private int waitCounterTime;
        private int waitVerifyTime;
        private int waitResultTime;
        private int waitConversionTime;
        private int waitChangeTeamTime;
        private int waitEmbezzlementTime;
        private int waitBettwenStateTime;

        #endregion

        private GameManager gameManager;
        private GameSetting gameSetting;

        public static bool Install_Complate = false;
        private List<string> enable_Ability = new List<string>();



        public GameResourceManager(GameManager gameManager, GameSetting gameSetting)
        {
            Debug.Log("GameResourceManager Created");

            this.gameManager = gameManager;

            this.gameSetting = gameSetting;

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

                bool CAI_Result = Update_CostAndIncome_Rule(gameSetting.propertiesSetting);

                bool CA_Result = Update_CharacterAmount_Rule(gameSetting.characterSetting);

                bool TR_Result = Update_TimeResource_Rule(gameSetting.advanceSetting);

                if (!CAI_Result || !CA_Result || !TR_Result) throw new Exception($"Can't Setting GameRule, Result | CAI_Result = {CAI_Result} | CA_Result = {CA_Result} | TR_Result = {TR_Result}");

                SetUp_CollectionCharacter_Center();

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
                    //------------------------ // Card And Resource Event. // ------------------------//

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

                    //----------------------- // Character Amount Card Event. // --------------------------//

                    case ResourceManager_List.GetDukeAnoumt:
                        allCharacter.TryGetValue("Duke", out int Duke);
                        Return_Packet = Duke;
                        break;
                    case ResourceManager_List.GetAssassinAnoumt:
                        allCharacter.TryGetValue("Assassin", out int Assassin);
                        Return_Packet = Assassin;
                        break;
                    case ResourceManager_List.GetCaptainAnoumt:
                        allCharacter.TryGetValue("Captain", out int Captain);
                        Return_Packet = Captain;
                        break;
                    case ResourceManager_List.GetAmbassdorAnoumt:
                        allCharacter.TryGetValue("Ambassdor", out int Ambassdor);
                        Return_Packet = Ambassdor;
                        break;
                    case ResourceManager_List.GetContessaAnoumt:
                        allCharacter.TryGetValue("Contessa", out int Contessa);
                        Return_Packet = Contessa;
                        break;
                    case ResourceManager_List.GetInquisitorAnoumt:
                        allCharacter.TryGetValue("Inquisitor", out int Inquisitor);
                        Return_Packet = Inquisitor;
                        break;
                    case ResourceManager_List.GetAllCharacterAnoumt: Return_Packet = allCharacter; break;

                    //--------------------- // Properties Resource Event.  // -----------------------//

                    case ResourceManager_List.GetIncomeCostAndIncome: Return_Packet = FindIncomeAndCost("Income"); break;
                    case ResourceManager_List.GetForgetAidCostAndIncome: Return_Packet = FindIncomeAndCost("ForgetAid"); break;
                    case ResourceManager_List.GetCoupCostAndIncome: Return_Packet = FindIncomeAndCost("Coup"); break;
                    case ResourceManager_List.GetDukeCostAndIncome: Return_Packet = FindIncomeAndCost("Duke"); break;
                    case ResourceManager_List.GetAssassinCostAndIncome: Return_Packet = FindIncomeAndCost("Assassin"); break;
                    case ResourceManager_List.GetCaptainCostAndIncome: Return_Packet = FindIncomeAndCost("Captain"); break;
                    case ResourceManager_List.GetAmbassdorCostAndIncome: Return_Packet = FindIncomeAndCost("Ambassdor"); break;
                    case ResourceManager_List.GetContessaCostAndIncome: Return_Packet = FindIncomeAndCost("Contessa"); break;
                    case ResourceManager_List.GetInquisitorCostAndIncome: Return_Packet = FindIncomeAndCost("Inquisitor"); break;
                    case ResourceManager_List.GetAllCostAndIncome: Return_Packet = costAndAction; break;

                    //--------------------- // Advance Resource Event. // --------------------------//

                    case ResourceManager_List.GetWaitCommandTime: Return_Packet = waitCommandTime; break;
                    case ResourceManager_List.GetWaitCounterTime: Return_Packet = waitCounterTime; break;
                    case ResourceManager_List.GetWaitVerifyTime: Return_Packet = waitVerifyTime; break;
                    case ResourceManager_List.GetWaitResultTime: Return_Packet = waitResultTime; break;
                    case ResourceManager_List.GetWaitChangeTeam_Time: Return_Packet = waitChangeTeamTime; break;
                    case ResourceManager_List.GetWaitConversionTime: Return_Packet = waitConversionTime; break;
                    case ResourceManager_List.GetWaitEmbezzlementTime: Return_Packet = waitEmbezzlementTime; break;
                    case ResourceManager_List.GetWaitBettwenStateTime: Return_Packet = waitBettwenStateTime; break;

                    // --------------------- // Setting Event. // -----------------------//

                    case ResourceManager_List.Setting_CharacterAmount:
                        if (Request_Data.PacketData is CharacterSetting CS_Rule)
                        {
                            bool UpdateResult_CA = Update_CharacterAmount_Rule(CS_Rule);
                            if (UpdateResult_CA) Return_Packet = UpdateResult_CA;
                            else Return_Packet = null;
                            break;
                        }
                        Return_Packet = null;
                        break;
                    case ResourceManager_List.Setting_CostAndIncome:
                        if (Request_Data.PacketData is PropertiesSetting PS_Rule)
                        {
                            bool UpdateResult_PS = Update_CostAndIncome_Rule(PS_Rule);
                            if (UpdateResult_PS) Return_Packet = UpdateResult_PS;
                            else Return_Packet = null;
                        }
                        Return_Packet = null;
                        break;
                    case ResourceManager_List.Setting_TimerResource:
                        if (Request_Data.PacketData is AdvanceSetting AS_Rule)
                        {
                            bool UpdateResult_AS = Update_TimeResource_Rule(AS_Rule);
                            if (UpdateResult_AS) Return_Packet = UpdateResult_AS;
                            else Return_Packet = null;
                        }
                        Return_Packet = null;
                        break;

                    //------------------- // Sort Player Resource // ------------------------//

                    case ResourceManager_List.GetRandomPlayerCard:
                        Return_Packet = GetRandomPlayerCard();
                        break;
                    case ResourceManager_List.GetRandomPlayerCoin:
                        Return_Packet = GetRandomPlayerCoin();
                        break;
                    case ResourceManager_List.GetRandomCenterCard:
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

            Return_GameResource = new GameResource_Result
            {
                requestCommand_Reult = false,
                requestType = (ResourceManager_List)Request_Data.EndPoint,
                return_Data = null,
            };

            return Return_GameResource;



        }

        #endregion

        #region Local Function

        private Tuple<string, int, int> FindIncomeAndCost(string characterSelection)
        {
            foreach (var Selection in costAndAction)
            {
                if (Selection.Item1 == characterSelection)
                {
                    return Selection;
                }
            }

            return null;
        }

        private bool Update_CostAndIncome_Rule(PropertiesSetting updateCAI)
        {
            try
            {
                if (updateCAI.Equals(typeof(PropertiesSetting))) throw new NullReferenceException("PacketData CostAndIncome is Null.");

                allCoinCenter = updateCAI.totalCoin;
                allCardCenter = updateCAI.totalCard;
                allCoinTreasury_Reserve = updateCAI.maxnium_TreasuryReserve;
                coinStarter = updateCAI.strarterCoin;
                cardStarter = updateCAI.starterCard;

                costAndAction = new List<Tuple<string, int, int>>
                {
                    new Tuple<string , int , int>("Income" , updateCAI.income , updateCAI.income_Cost),
                    new Tuple<string , int , int>("ForgetAid" , updateCAI.forgetaid_Income , updateCAI.forgetaid_Cost),
                    new Tuple<string , int , int>("Coup" , updateCAI.coup_Income , updateCAI.coup_Cost),
                    new Tuple<string , int , int>("Duke" , updateCAI.duke_Income , updateCAI.duke_Cost),
                    new Tuple<string , int , int>("Assassin" , updateCAI.assassin_Income , updateCAI.assassin_Cost),
                    new Tuple<string , int , int>("Captain" , updateCAI.captain_Income , updateCAI.captain_Cost),
                    new Tuple<string , int , int>("Ambassdor" , updateCAI.ambassdor_Income , updateCAI.ambassin_Cost),
                    new Tuple<string , int , int>("Contassa" , updateCAI.contassa_Income , updateCAI.contassa_Cost),
                    new Tuple<string , int , int>("Inquisitor" , updateCAI.inquisitor_Income , updateCAI.inquisitor_Cost),
                };

                enable_Ability = updateCAI.allAbility_CanUse;

                return true;
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);

                return false;
            }
        }

        private bool Update_CharacterAmount_Rule(CharacterSetting updateCS)
        {
            try
            {
                if (updateCS.Equals(typeof(CharacterSetting))) throw new NullReferenceException("PacketData Character Amount is Null.");

                allCharacter["Duke"] = updateCS.duke;
                allCharacter["Captain"] = updateCS.captain;
                allCharacter["Assassin"] = updateCS.assassins;
                allCharacter["Ambassdor"] = updateCS.ambassdor;
                allCharacter["Contessa"] = updateCS.contassa;

                return true;
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                return false;
            }
        }

        private bool Update_TimeResource_Rule(AdvanceSetting updateAS)
        {
            try
            {
                waitCommandTime = updateAS.waitCommandTime;
                waitCounterTime = updateAS.waitCounterTime;
                waitVerifyTime = updateAS.waitVerifyTime;
                waitResultTime = updateAS.waitResultTime;
                waitChangeTeamTime = updateAS.waitChangeTeamTime;
                waitEmbezzlementTime = updateAS.waitEmbezzlementTime;
                waitConversionTime = updateAS.waitConversionTime;
                waitBettwenStateTime = updateAS.waitBettwenState;

                return true;
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                return false;
            }
        }

        private void SetUp_CollectionCharacter_Center()
        {
            List<string> CollectionCharacter = new List<string>();

            foreach (var Character in allCharacter)
            {
                string Selection_ID = Character.Key;
                int Anoumt_ID = Character.Value;

                for (int i = 0; i < Anoumt_ID; i++)
                {
                    CollectionCharacter.Add(Selection_ID);
                }
            }

            System.Random random = new System.Random();

            CollectionCharacter = CollectionCharacter.OrderBy(n => random.Next()).ToList();

            allCharacter_InCenter = CollectionCharacter;
        }

        private string[] GetRandomPlayerCard()
        {
            if (allCharacter_InCenter == null) Debug.LogError("Character Null");

            List<string> RandomCard = new List<string>();
            List<string> AllCard = allCharacter_InCenter.ToList();

            for (int i = 0; i < AllCard.Count; i++)
            {
                if (i > cardStarter - 1) break;

                RandomCard.Add(AllCard[i]);

                allCharacter_InCenter.Remove(AllCard[i]);
            }

            return RandomCard.ToArray();
        }

        private int GetRandomPlayerCoin()
        {
            if (allCoinCenter < 0) Debug.LogError("Coin 0");
            int Coin = 0;

            if (coinStarter > allCoinCenter)
            {
                Coin = Mathf.Abs(coinStarter - allCoinCenter);

                allCoinCenter = 0;

                return Coin;
            }
            else if (allCoinCenter <= 0)
            {
                allCoinCenter = 0;

                return 0;
            }

            Coin = coinStarter;

            allCoinCenter -= coinStarter;

            return Coin;
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