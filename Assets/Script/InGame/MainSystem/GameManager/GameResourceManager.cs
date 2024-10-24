using System;
using UnityEngine;
using System.Linq;
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

        // How Much Character Can Play.
        private Dictionary<string, int> characterAndAmount_InGame = new Dictionary<string, int>();

        // Cost Pay Action.
        private List<Tuple<string, int, int>> abilityProperties = new List<Tuple<string, int, int>>();

        private List<string> cards_In_Center = new List<string>();
        private List<string> ability_HasActive = new List<string>();

        // Total Card And Coin In Game.
        private int totalCoin;
        private int totalCard;
        private int allCoinTreasury_Reserve;

        // Coin And Card Collection.
        private int coinStarter;
        private int cardStarter;

        // Timer Properties Variable.
        private int waitCommandTime;
        private int waitCounterTime;
        private int waitVerifyTime;
        private int waitResultTime;
        private int waitConversionTime;
        private int waitChangeTeamTime;
        private int waitEmbezzlementTime;
        private int waitBettwenStateTime;

        // Component
        private GameManager gameManager;
        private GameSetting gameSetting;

        // Status GameResoucre.
        public static bool Install_Complate = false;

        public GameResourceManager(GameManager gameManager, GameSetting gameSetting)
        {
            Debug.Log("GameResourceManager Created");

            // Collect Main GameManager.
            this.gameManager = gameManager;

            // Collect Game Setting.
            this.gameSetting = gameSetting;

            // Install Resource.
            InstallResource(this.gameSetting);
        }

        private async void InstallResource(GameSetting gameSetting)
        {
            try
            {
                if (!Check_NullOrDefualt_Data<GameSetting>(gameSetting))
                {
                    Debug.LogError("GameResoucreManager -> InstallResoucre | PacketData is null Or Default.");
                    gameSetting = default;
                    return;
                }

                // All Number Character In Game ,How Much Character Can Play.
                characterAndAmount_InGame = new Dictionary<string, int>
                {
                    {"Duke" , 0},
                    {"Captain" , 0},
                    {"Assassin" , 0},
                    {"Ambassdor" , 0},
                    {"Contessa" , 0},
                    {"inquisitor" , 0}
                };

                await InstallResoucre_CheckCorrect(gameSetting);

                SetUp_CollectionCharacter_Center();

                Install_Complate = true;

            }
            catch (Exception ex)
            {
                Install_Complate = false;

                Debug.LogError(ex);
            }
        }

        private async Task<bool> InstallResoucre_CheckCorrect(GameSetting GS)
        {
            bool TR_Result = Update_TimeResource_Rule(GS.advanceSetting);
            bool CAI_Result = Update_CostAndIncome_Rule(GS.propertiesSetting);
            bool CA_Result = Update_CharacterAmount_Rule(GS.characterSetting);

            bool AllResult_IsCorrect = TR_Result && CAI_Result && CA_Result;

            await Task.Delay(0);

            return AllResult_IsCorrect == true
                ? AllResult_IsCorrect
                : throw new Exception($"Can't Setting GameRule, Result | CAI_Result = {CAI_Result} | CA_Result = {CA_Result} | TR_Result = {TR_Result}");
        }

        #region Game Resource Control.

        public GameResource_Result GameResource_Control(GameManager_Data Request_Data)
        {
            object Return_Packet = null;

            ResourceManager_List EndPoint = CheckAndConverter<ResourceManager_List>(Request_Data.EndPoint);

            Return_Packet = ProcessResoucre_List(EndPoint, Request_Data.PacketData);

            return CreateReturn_Data(EndPoint, Return_Packet, Return_Packet is null ? false : true);
        }

        private object ProcessResoucre_List(ResourceManager_List target, object packet_Install)
        {
            return target switch
            {
                // Starter Resoucre.
                ResourceManager_List.GetStarterCard => cardStarter,
                ResourceManager_List.GetStarterCoin => coinStarter,
                ResourceManager_List.GetAllCenterCoin => totalCoin,
                ResourceManager_List.GetAllCenterCard => totalCard,
                ResourceManager_List.GetAllTreasury => allCoinTreasury_Reserve,

                // Number of Coins And Character Cords.
                ResourceManager_List.GetAllCharacterCard => characterAndAmount_InGame,
                ResourceManager_List.GetAllCharacterNumber => totalCard,

                // Collect the ability commands are in this Match.
                ResourceManager_List.GetAbilit_CanUse => ability_HasActive,

                // Get Character Info as well as Number of Card the per Character.
                ResourceManager_List.GetDukeAnoumt => FindAmountOfCharacterCard("Duke"),
                ResourceManager_List.GetCaptainAnoumt => FindAmountOfCharacterCard("Captain"),
                ResourceManager_List.GetContessaAnoumt => FindAmountOfCharacterCard("Contessa"),
                ResourceManager_List.GetAssassinAnoumt => FindAmountOfCharacterCard("Assassin"),
                ResourceManager_List.GetAmbassdorAnoumt => FindAmountOfCharacterCard("Ambassdor"),
                ResourceManager_List.GetInquisitorAnoumt => FindAmountOfCharacterCard("Inquisitor"),
                ResourceManager_List.GetAllCharacterAnoumt => characterAndAmount_InGame,

                // Resoucre of this Match.
                ResourceManager_List.GetDukeCostAndIncome => FindIncomeAndCost("Duke"),
                ResourceManager_List.GetCoupCostAndIncome => FindIncomeAndCost("Coup"),
                ResourceManager_List.GetIncomeCostAndIncome => FindIncomeAndCost("Income"),
                ResourceManager_List.GetCaptainCostAndIncome => FindIncomeAndCost("Captain"),
                ResourceManager_List.GetContessaCostAndIncome => FindIncomeAndCost("Contessa"),
                ResourceManager_List.GetAssassinCostAndIncome => FindIncomeAndCost("Assassins"),
                ResourceManager_List.GetAmbassdorCostAndIncome => FindIncomeAndCost("Ambassdor"),
                ResourceManager_List.GetForgetAidCostAndIncome => FindIncomeAndCost("ForgetAid"),
                ResourceManager_List.GetInquisitorCostAndIncome => FindIncomeAndCost("Inquisitor"),

                // Time Management Resoucre.
                ResourceManager_List.GetWaitResultTime => waitResultTime,
                ResourceManager_List.GetWaitVerifyTime => waitVerifyTime,
                ResourceManager_List.GetWaitCommandTime => waitCommandTime,
                ResourceManager_List.GetWaitCounterTime => waitCounterTime,
                ResourceManager_List.GetWaitConversionTime => waitConversionTime,
                ResourceManager_List.GetWaitChangeTeam_Time => waitChangeTeamTime,
                ResourceManager_List.GetWaitEmbezzlementTime => waitEmbezzlementTime,
                ResourceManager_List.GetWaitBettwenStateTime => waitBettwenStateTime,

                // Random Resoucre.
                ResourceManager_List.GetRandomPlayerCard => GetRandomPlayerCard(),
                ResourceManager_List.GetRandomPlayerCoin => GetRandomPlayerCoin(),

                // Setting Resoucre.
                ResourceManager_List.Setting_CostAndIncome => ProcessSetting_CostAndIncome(packet_Install),
                ResourceManager_List.Setting_TimerResource => ProcessSetting_TimerResource(packet_Install),
                ResourceManager_List.Setting_CharacterAmount => ProcessSetting_CharacterAnoumt(packet_Install),

                // Status Install of GameResoucre Manager.
                ResourceManager_List.GetInstall_Complate => Install_Complate,
                _ => throw new ArgumentException($"GameResoucreManager -> ProcessResoucre | Unknown {target} Target."),
            };
        }

        private bool ProcessSetting_CharacterAnoumt(object packet_Install)
        {
            CharacterSetting settingCharacter_Data = CheckAndConverter<CharacterSetting>(packet_Install);

            return Update_CharacterAmount_Rule(settingCharacter_Data);
        }

        private bool ProcessSetting_CostAndIncome(object packet_Install)
        {
            PropertiesSetting settingProperties_Data = CheckAndConverter<PropertiesSetting>(packet_Install);

            return Update_CostAndIncome_Rule(settingProperties_Data);
        }

        private bool ProcessSetting_TimerResource(object packet_Install)
        {
            AdvanceSetting settingAdvance_Data = CheckAndConverter<AdvanceSetting>(packet_Install);

            return Update_TimeResource_Rule(settingAdvance_Data);
        }

        #endregion

        #region Local Function

        private int FindAmountOfCharacterCard(string characterName) => characterAndAmount_InGame[characterName];

        private Tuple<string, int, int> FindIncomeAndCost(string characterSelection)
        {
            foreach (var Selection in abilityProperties)
            {
                if (Selection.Item1 == characterSelection)
                {
                    return Selection;
                }
            }

            return null;
        }

        private bool Update_CostAndIncome_Rule(PropertiesSetting PS)
        {
            try
            {
                if (!Check_NullOrDefualt_Data<PropertiesSetting>(PS)) throw new ArgumentException("Update_CostAndIncome_Rule | PacketData is Null Or Defualt.");

                totalCoin = PS.totalCoin;
                totalCard = PS.totalCard;
                cardStarter = PS.starterCard;
                coinStarter = PS.strarterCoin;
                allCoinTreasury_Reserve = PS.maxnium_TreasuryReserve;

                abilityProperties = new List<Tuple<string, int, int>>
                {
                    new Tuple<string , int , int>("Income" , PS.income , PS.income_Cost),
                    new Tuple<string , int , int>("ForgetAid" , PS.forgetaid_Income , PS.forgetaid_Cost),
                    new Tuple<string , int , int>("Coup" , PS.coup_Income , PS.coup_Cost),
                    new Tuple<string , int , int>("Duke" , PS.duke_Income , PS.duke_Cost),
                    new Tuple<string , int , int>("Assassin" , PS.assassin_Income , PS.assassin_Cost),
                    new Tuple<string , int , int>("Captain" , PS.captain_Income , PS.captain_Cost),
                    new Tuple<string , int , int>("Ambassdor" , PS.ambassdor_Income , PS.ambassin_Cost),
                    new Tuple<string , int , int>("Contassa" , PS.contassa_Income , PS.contassa_Cost),
                    new Tuple<string , int , int>("Inquisitor" , PS.inquisitor_Income , PS.inquisitor_Cost),
                };

                ability_HasActive = PS.allAbility_CanUse;

                return true;
            }
            catch (ArgumentException Arg)
            {
                Debug.LogError(Arg);

                return false;
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);

                return false;
            }
        }

        private bool Update_CharacterAmount_Rule(CharacterSetting CS)
        {
            try
            {
                if (!Check_NullOrDefualt_Data<CharacterSetting>(CS)) throw new ArgumentException("Update_CharacterAmount_Rule | PacketData is Null Or Default.");

                characterAndAmount_InGame["Duke"] = CS.duke;
                characterAndAmount_InGame["Captain"] = CS.captain;
                characterAndAmount_InGame["Contessa"] = CS.contassa;
                characterAndAmount_InGame["Assassin"] = CS.assassins;
                characterAndAmount_InGame["Ambassdor"] = CS.ambassdor;

                return true;
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                return false;
            }
        }

        private bool Update_TimeResource_Rule(AdvanceSetting ADS)
        {
            try
            {
                if (!Check_NullOrDefualt_Data<AdvanceSetting>(ADS)) throw new ArgumentException("Update_TimeResoucre_Rule | PacketData is Null Or Default.");

                waitVerifyTime = ADS.waitVerifyTime;
                waitResultTime = ADS.waitResultTime;
                waitCounterTime = ADS.waitCounterTime;
                waitCommandTime = ADS.waitCommandTime;
                waitChangeTeamTime = ADS.waitChangeTeamTime;
                waitConversionTime = ADS.waitConversionTime;
                waitBettwenStateTime = ADS.waitBettwenState;
                waitEmbezzlementTime = ADS.waitEmbezzlementTime;

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

            foreach (var Character in characterAndAmount_InGame)
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

            cards_In_Center = CollectionCharacter;
        }

        private string[] GetRandomPlayerCard()
        {
            if (cards_In_Center == null) throw new ArgumentException("GetRandomPlayerCard | Card In Collection is Null Or Empty.");

            List<string> RandomCard = new List<string>();
            List<string> AllCard = cards_In_Center.ToList();

            for (int i = 0; i < AllCard.Count; i++)
            {
                if (i > cardStarter - 1) break;

                RandomCard.Add(AllCard[i]);

                cards_In_Center.Remove(AllCard[i]);
            }

            return RandomCard.ToArray();
        }

        private int GetRandomPlayerCoin()
        {
            if (totalCoin < 0) throw new ArgumentException("GetRamdomPlayerCoin | Coin In Collection is less than zero.");
            int Coin = 0;

            if (coinStarter > totalCoin)
            {
                Coin = Mathf.Abs(coinStarter - totalCoin);

                totalCoin = 0;

                return Coin;
            }
            else if (totalCoin <= 0)
            {
                totalCoin = 0;

                return 0;
            }

            Coin = coinStarter;

            totalCoin -= coinStarter;

            return Coin;
        }

        #endregion

        #region Checking And Verify Data Type

        private T CheckAndConverter<T>(object packet_Type)
        {
            if (packet_Type is null) throw new ArgumentException($"Packet Data : {packet_Type.GetType().Name} is null Or Empty.");

            return packet_Type.GetType() == typeof(T)
                ? (T)packet_Type
                : throw new ArgumentException($"Packet Data : {packet_Type.GetType().Name} is not {typeof(T).Name} Type.");
        }

        private GameResource_Result CreateReturn_Data(ResourceManager_List endpoint, object packet_return = null, bool isCorrectProcess = false)
        {
            return new GameResource_Result(endpoint, packet_return, isCorrectProcess);
        }

        private bool Check_NullOrDefualt_Data<T>(object packet_Data)
        {
            var ConvertToCheck = CheckAndConverter<T>(packet_Data);

            return !ConvertToCheck.Equals(typeof(T));
        }

        #endregion
    }

    [Serializable]
    public struct GameResource_Result : IInGameEvent
    {
        public bool requestCommand_Reult;
        public ResourceManager_List requestType;
        public object return_Data;

        public GameResource_Result(ResourceManager_List endPoint, object packet_return, bool isCorrectProcess = false)
        {
            if (isCorrectProcess)
            {
                requestCommand_Reult = true;
                requestType = endPoint;
                return_Data = packet_return;
            }
            else
            {
                requestCommand_Reult = false;
                requestType = endPoint;
                return_Data = null;
            }
        }
    }
}

 /*
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
            */
