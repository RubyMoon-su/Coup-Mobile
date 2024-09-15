using System;
using UnityEngine;
using Coup_Mobile.EventBus;
using System.Threading.Tasks;
using System.Collections.Generic;
using Coup_Mobile.InGame.GameManager.GameState;
using Coup_Mobile.InGame.GameManager.ReportData;
using Coup_Mobile.InGame.PlayerData;
using Coup_Mobile.Menu.GameSetting_Data;


namespace Coup_Mobile.InGame.GameManager
{
    public enum GameStateManager_List
    {
        // Control System Game State.
        Start_StarterState,
        Start_MainState,
        Stop_StarterState,
        Stop_MainState,
        Reset_StarterState,
        Reset_MainState,

        // Control Action Game State.
        ChangeStateTo,
        Next_State,
        Previous_State,

        // Common Getter.
        GetCurrentState,
        GetNextState,
        GetPreviousState,

        GetAll_StarterState,
        GetAll_MainState,

        // Field Getter.
        GetInstall_Complate,

        UpdateStateNetwork,
    }
    public enum GameState_List
    {
        // Normal Game State.
        // Process Before StartGame.
        Check_Component_And_GameEvent,
        Setup_Player_Properties,
        Process_Player_Sort,

        // Process State Endless Loop 
        Next_Wave_State,
        Player_Send_Command,
        Player_Counter_Command,
        Player_Verifiy_Command,
        Player_Result_Command,

        // Process Cycle through all states.
        Wait_For_AllPlayerAndProcessGame,

        // Refomation Game State
        // Process Before StartGame.
        Player_Select_Team,

        // Process State Endless Loop.
        Player_Conversion_Command,
        Player_Conversion_Result_Command,
        Player_Embezzlement_Command,
        Player_Embezzlement_Result_Command,



        #region Custorm Game

        #endregion

    }

    public class GameStateManager : IOtherPlayerResponsive_Subject
    {

        #region GameState Template
        private readonly List<GameState_List> StarterState = new List<GameState_List>()
        {
            GameState_List.Check_Component_And_GameEvent ,
            GameState_List.Setup_Player_Properties,
            GameState_List.Process_Player_Sort,
        };

        private readonly List<GameState_List> GameState = new List<GameState_List>()
        {
            GameState_List.Next_Wave_State,
            GameState_List.Player_Send_Command,
            GameState_List.Player_Counter_Command,
            GameState_List.Player_Verifiy_Command,
            GameState_List.Player_Result_Command,
            GameState_List.Wait_For_AllPlayerAndProcessGame,
        };

        #endregion

        #region  GameState Selection

        private List<GameState_List> StarterState_Result = new List<GameState_List>();
        private List<GameState_List> GameState_Result = new List<GameState_List>();

        #endregion

        #region  GameState Field

        // StarterGame State Field
        private Dictionary<GameState_List, IGameState_Strategy> starter_GameStateColleciton;

        // Main GameState Field
        // State Game Collection.
        private Dictionary<GameState_List, IGameState_Strategy> main_GameStateCollection;

        #endregion

        #region Universal Variable Global Field

        // Ref Component.
        private GameManager gameManager;

        // State Component And Info.
        private GameState_List currentState;
        private GameState_List nextState;
        private GameState_List beforeState;

        // Update Form Network.
        private List<IOtherPlayerResponsive_Oberver> registered_Notify = new List<IOtherPlayerResponsive_Oberver>();

        // Game State Status.
        private bool isGameStateStop = false;
        private bool install_Complate = false;

        #endregion

        #region Online Variable Global Field



        #endregion

        #region Offline Variabel Global Field



        #endregion

        #region Install State Function

        public GameStateManager(GameManager gameManager , GameSetting gameSetting)
        {
            Debug.Log("GameStateManager Created.");

            this.gameManager = gameManager;

            _ = Install_System(gameSetting);
        }

        private async Task Install_System(GameSetting gameMode)
        {
            // Setup GameState List.
            bool Setup_StarterState = await Process_StarterState(gameMode.gameMode);

            int Timer = 0;
            int SystemTimeOut = GameManager.SYSTEM_TIMEOUT;

            while (!Setup_StarterState)
            {
                await Task.Delay(1000);

                Timer++;

                if (Timer == SystemTimeOut)
                {
                    Debug.LogError("Can't Install StarterState.");
                    return;
                }
            }

            Timer = 0;

            bool Setup_GameState = await Process_GameState(gameMode.gameMode);

            while (!Setup_GameState)
            {
                await Task.Delay(1000);

                Timer++;

                if (Timer == SystemTimeOut)
                {
                    Debug.LogError("Can't Install GameState.");
                    return;
                }
            }

            //------------ // Create Object GameState. // --------------//

            await CreateAndCollection_StarterState();

            await CreateAndCollection_MainState();

            Debug.Log("GameStateManager Install Complate.");
            install_Complate = true;

            await Starter_GameState();
        }

        private async Task<bool> CreateAndCollection_StarterState()
        {
            bool Result_Collection = true;

            await Task.Delay(0);

            try
            {
                starter_GameStateColleciton = new Dictionary<GameState_List, IGameState_Strategy>();

                // Starter GameState.
                starter_GameStateColleciton.Add(GameState_List.Check_Component_And_GameEvent, new InitializationChecker_GameState(this));
                starter_GameStateColleciton.Add(GameState_List.Setup_Player_Properties, new SetupPlayerProperties_GameState(this));
                starter_GameStateColleciton.Add(GameState_List.Process_Player_Sort, new Process_PlayerSort_GameState(this));
                starter_GameStateColleciton.Add(GameState_List.Player_Select_Team, new SetUp_PlayerTeam_GameState(this));
            }
            catch (Exception ex)
            {
                Debug.LogError($"Can't Create object instance starter State. {ex}");
                Result_Collection = false;

                return Result_Collection;
            }

            return Result_Collection;
        }

        private async Task<bool> CreateAndCollection_MainState()
        {
            bool Result_Collection = true;

            await Task.Delay(0);

            try
            {
                main_GameStateCollection = new Dictionary<GameState_List, IGameState_Strategy>();

                // Main GameState.
                main_GameStateCollection.Add(GameState_List.Player_Send_Command, new Player_SendCommand_GameState(this));
                main_GameStateCollection.Add(GameState_List.Player_Counter_Command, new Player_CounterCommand_GameState(this));
                main_GameStateCollection.Add(GameState_List.Player_Verifiy_Command, new Player_VerifyCommand_GameState(this));
                main_GameStateCollection.Add(GameState_List.Player_Result_Command, new Player_ResultCommand_GameState(this));
                main_GameStateCollection.Add(GameState_List.Wait_For_AllPlayerAndProcessGame, new WaitForAllPlayer_GameState(this));

                main_GameStateCollection.Add(GameState_List.Player_Conversion_Command, new Player_ConversionCommand_GameState(this));
                main_GameStateCollection.Add(GameState_List.Player_Conversion_Result_Command, new Player_ConversionResult_Command_GameState(this));
                main_GameStateCollection.Add(GameState_List.Player_Embezzlement_Command, new Player_EmbezzlementCommand_GameState(this));
                main_GameStateCollection.Add(GameState_List.Player_Embezzlement_Result_Command, new Player_EmbezzlementResult_Command_GameState(this));
            }
            catch (Exception ex)
            {
                Debug.LogError($"Can't Create object instance starter State. {ex}");
                Result_Collection = false;

                return Result_Collection;
            }

            return Result_Collection;
        }


        #region  State Starter Function

        private async Task<bool> Process_GameState(GameMode gamemode)
        {
            List<GameState_List> GameState = new List<GameState_List>();
            await Task.Delay(0);

            try
            {
                switch (gamemode)
                {
                    case GameMode.NormalGame:
                        GameState = this.GameState;
                        break;
                    case GameMode.RefomationGame:
                        GameState = new List<GameState_List>()
                        {
                            GameState_List.Player_Conversion_Command,
                            GameState_List.Player_Conversion_Result_Command,
                            GameState_List.Player_Embezzlement_Command,
                            GameState_List.Player_Embezzlement_Result_Command,
                        };

                        foreach (GameState_List State in this.GameState)
                        {
                            StarterState.Add(State);
                        }

                        break;
                    case GameMode.CustormGame:
                        break;
                }

                GameState_Result = GameState;
                return true;
            }
            catch
            {
                return false;
            }
        }

        private async Task<bool> Process_StarterState(GameMode gamemode, object CustormMode = null)
        {
            List<GameState_List> StarterState = new List<GameState_List>();

            switch (gamemode)
            {
                case GameMode.NormalGame:

                    StarterState = this.StarterState;

                    break;
                case GameMode.RefomationGame:

                    StarterState = this.StarterState;

                    StarterState.Add(GameState_List.Player_Select_Team);

                    break;
                case GameMode.CustormGame:

                    StarterState = this.StarterState;

                    break;
                default:

                    return false;
            }

            await Task.Delay(0);

            StarterState_Result = StarterState;

            return true;
        }

        #endregion



        #endregion

        #region GameState Control
        public GameStateManager_Return GameStateManager_Control(GameManager_Data Request_Data)
        {
            GameStateManager_Return Return_GameState = new GameStateManager_Return();
            object Packet_Data = null;

            if (Request_Data.EndPoint is GameStateManager_List GSM_List)
            {
                switch (GSM_List)
                {
                    // Control System Game State.
                    case GameStateManager_List.Start_StarterState:
                    case GameStateManager_List.Start_MainState:
                        isGameStateStop = false;
                        break;
                    case GameStateManager_List.Stop_StarterState:
                    case GameStateManager_List.Stop_MainState:
                        isGameStateStop = true;
                        break;

                    // Control Action Game State.
                    case GameStateManager_List.ChangeStateTo:
                        break;
                    case GameStateManager_List.Next_State:
                        break;
                    case GameStateManager_List.Previous_State:
                        break;

                    // Common Getter.
                    case GameStateManager_List.GetCurrentState:
                        break;
                    case GameStateManager_List.GetNextState:
                        break;
                    case GameStateManager_List.GetPreviousState:
                        break;
                    case GameStateManager_List.GetAll_StarterState:
                        Packet_Data = StarterState_Result;
                        break;
                    case GameStateManager_List.GetAll_MainState:
                        Packet_Data = GameState_Result;
                        break;
                    case GameStateManager_List.GetInstall_Complate: Packet_Data = install_Complate; break;
                    case GameStateManager_List.UpdateStateNetwork: Notify(Request_Data.PacketData); break;
                }
            }

            if (Packet_Data != null)
            {
                Return_GameState = new GameStateManager_Return
                {
                    requestCommand_Reult = true,
                    requestType = (GameStateManager_List)Request_Data.EndPoint,
                    return_Data = Packet_Data,
                };

                return Return_GameState;
            }

            Return_GameState = new GameStateManager_Return
            {
                requestCommand_Reult = false,
                requestType = (GameStateManager_List)Request_Data.EndPoint,
                return_Data = null,
            };

            return Return_GameState;
        }


        #endregion

        #region Local Function
        private async Task Starter_GameState()
        {
            await WaitInstallationComplate();

            if (!HasStarterState())
            {
                Debug.LogError("No StarterState Register.");
                return;
            }


            foreach (var State in StarterState_Result)
            {
                Debug.LogWarning($"Starter State Run {State}");

                beforeState = currentState;

                var State_Selection = GetStarter_SelectionState(State);

                if (State_Selection == null)
                {
                    Debug.LogError("State_Selection Is null");
                    return;
                }

                if (isGameStateStop) return;

                currentState = State;

                var Result = await ProcessGameState(State_Selection, new GameManager_Data());

                if (isGameStateStop || !ProcessResult(Result, State)) return;

                Debug.Log("Next State = " + State_Selection);

                await Task.Delay(2000);
            }

            if (isGameStateStop) return;

            // Run Starter Complate.
            // Start MainGame State.
            await Main_GameState();
        }

        private async Task Main_GameState()
        {
            await Task.Delay(0);

            await WaitInstallationComplate();

            if (!HasMainState())
            {
                Debug.LogError("No MainState Register.");
                return;
            }

            foreach (var State in GameState_Result)
            {
                Debug.LogWarning($"Main State Run {State}");

                var State_Selection = GetMain_SelectionState(State);

                if (State_Selection == null)
                {
                    Debug.LogError("State_Selection Is null");
                    return;
                }

                if (isGameStateStop) return;

                currentState = State;

                var Result = await ProcessGameState(State_Selection, new GameManager_Data());

                if (isGameStateStop || !ProcessResult(Result, State)) return;

                nextState = FindNextState_Main(State);

                if ((int)nextState > Enum.GetNames(typeof(GameState_List)).Length)
                {
                    Debug.Log("Next Wave");
                }
                else
                {
                    Debug.Log("Next State");
                }
            }

            // Loop State Main_GameState.
            Contine_MainState();
        }

        private async void Contine_MainState()
        {
            await Main_GameState();
        }

        #region  Starter Handler Function

        private async Task WaitInstallationComplate()
        {
            while (!install_Complate)
            {
                await Task.Delay(1000);
            }
        }

        private bool HasStarterState()
        {
            return starter_GameStateColleciton.Count > 0;
        }

        private IGameState_Strategy GetStarter_SelectionState(GameState_List state)
        {
            var starterSelection = starter_GameStateColleciton[state];
            if (starterSelection == null)
            {
                Debug.LogError("State_Selection is null");
            }
            return starterSelection;
        }

        private async Task<GameState_Report> ProcessGameState(IGameState_Strategy stateSelection, GameManager_Data requestEvent)
        {
            return (GameState_Report)await stateSelection.ProcessGameState(requestEvent);
        }

        private bool ProcessResult(GameState_Report result, object state)
        {
            if (result.GetReportStatus == false)
            {
                Debug.LogError($"LoopStop {state}");
                if (result.GetReportString != null) Debug.LogError($"{result.GetReportString}");
                isGameStateStop = true;
                return false;
            }

            if (result.GetReportString != null)
            {
                isGameStateStop = false;
                Debug.LogError($"LoopStop System Message {result.GetReportString}");
            }

            return true;
        }

        #endregion

        #region Main Handler Function

        private bool HasMainState()
        {
            return main_GameStateCollection.Count > 0;
        }

        private IGameState_Strategy GetMain_SelectionState(GameState_List state)
        {
            var starterSelection = main_GameStateCollection[state];
            if (starterSelection == null)
            {
                Debug.LogError("MainState_Selection is null");
            }
            return starterSelection;
        }

        private GameState_List FindNextState_Main(GameState_List gameState)
        {
            int index = (int)gameState;
            index++;
            if (index + 1 > Enum.GetValues(typeof(GameState_List)).Length) index = 0;

            return (GameState_List)index;
        }

        #endregion

        #endregion

        #region Notify Form Network
        public void Attach(IOtherPlayerResponsive_Oberver observer)
        {
            registered_Notify.Add(observer);
        }

        public void Detach(IOtherPlayerResponsive_Oberver observer)
        {
            registered_Notify.Remove(observer);
        }

        public void Notify(object PacketData)
        {
            foreach (var Registered in registered_Notify)
            {
                Registered.UpdateFormNetwork(PacketData);
            }
        }

        #endregion

    }

    [Serializable]
    public struct GameStateManager_Return : IInGameEvent
    {
        public bool requestCommand_Reult;
        public GameStateManager_List requestType;
        public object return_Data;
    }
}