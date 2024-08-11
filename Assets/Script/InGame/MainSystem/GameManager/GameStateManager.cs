using System;
using UnityEngine;
using Coup_Mobile.EventBus;
using System.Threading.Tasks;
using System.Collections.Generic;
using Coup_Mobile.InGame.GameManager.GameState;
using Coup_Mobile.InGame.GameManager.ReportData;


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

        // Field Getter.
        GetInstall_Complate,
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

    public class GameStateManager
    {
        #region Gobal Field

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

        private List<GameState_List> StarterState_Result = new List<GameState_List>();
        private List<GameState_List> GameState_Result = new List<GameState_List>();

        // StarterGame State Field
        private IGameState_Strategy
          checkGameEvent_GameState
        , setupPlayerProperties_GameState
        , processPlayerSort_GameState
        , setupPlayerTeam_GameState;

        // Main GameState Field
        // State Game Collection.
        private IGameState_Strategy
          sendCommand_GameState
        , counterCommand_GameState
        , verifyCommand_GameState
        , resultCommand_GameState
        , waitForAllPlayer_GameState;

        private IGameState_Strategy
          conversionCommand_GameState
        , conversionResultCommand_GameState
        , embezzlementCommand_GameState
        , embezzlementCommandResult_GameState;

        private GameManager gameManager;

        private bool install_Complate = false;

        #endregion

        #region Install State Function

        public GameStateManager(GameManager gameManager)
        {
            Debug.Log("GameStateManager Created.");

            this.gameManager = gameManager;

            _ = Install_System();
        }

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

        private async Task<bool> Process_StarterState(GameMode gamemode)
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

        private async Task Install_System()
        {
            // Setup GameState List.
            bool Setup_StarterState = await Process_StarterState(GameMode.NormalGame);

            int Timer = 0;
            int SystemTimeOut = GameManager.GetSystemTimeout;

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

            bool Setup_GameState = await Process_GameState(GameMode.NormalGame);

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

            // Starter GameState.
            checkGameEvent_GameState = new CheckGameEvent_GameState(this);
            setupPlayerProperties_GameState = new SetupPlayerProperties_GameState(this);
            processPlayerSort_GameState = new Process_PlayerSort_GameState(this);
            setupPlayerTeam_GameState = new SetUp_PlayerTeam_GameState(this);


            // Main GameState.
            sendCommand_GameState = new Player_SendCommand_GameState(this);
            counterCommand_GameState = new Player_CounterCommand_GameState(this);
            verifyCommand_GameState = new Player_VerifyCommand_GameState(this);
            resultCommand_GameState = new Player_ResultCommand_GameState(this);
            waitForAllPlayer_GameState = new WaitForAllPlayer_GameState(this);

            conversionCommand_GameState = new Player_ConversionCommand_GameState(this);
            conversionResultCommand_GameState = new Player_ConversionResult_Command_GameState(this);
            embezzlementCommand_GameState = new Player_EmbezzlementCommand_GameState(this);
            embezzlementCommandResult_GameState = new Player_EmbezzlementResult_Command_GameState(this);

            Debug.Log("GameStateManager Install Complate.");
            install_Complate = true;

            await Starter_GameState();
        }

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
                        break;
                    case GameStateManager_List.Stop_StarterState:
                        break;
                    case GameStateManager_List.Start_MainState:
                        break;
                    case GameStateManager_List.Stop_MainState:
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
                    case GameStateManager_List.GetInstall_Complate: Packet_Data = install_Complate; break;
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
            while (!install_Complate)
            {
                await Task.Delay(1000);
            }

            if (StarterState_Result.Count <= 0)
            {
                Debug.LogError("No StarterState Register.");
                return;
            }

            foreach (GameState_List State in StarterState_Result)
            {
                Debug.LogWarning($"Starter State Run {State}");
                IGameState_Strategy ProcessState = null;

                switch (State)
                {
                    case GameState_List.Check_Component_And_GameEvent:

                        ProcessState = checkGameEvent_GameState;

                        break;
                    case GameState_List.Setup_Player_Properties:

                        ProcessState = setupPlayerProperties_GameState;

                        break;
                    case GameState_List.Process_Player_Sort:

                        ProcessState = processPlayerSort_GameState;

                        break;
                    case GameState_List.Player_Select_Team:

                        ProcessState = setupPlayerTeam_GameState;

                        break;
                    default:
                        Debug.LogError($"{State} is not StarterState Type.");
                        return;
                }

                if (ProcessState == null)
                {
                    Debug.LogError($"ProcessState 'Starter_GameState' Is Null");
                    return;
                }

                var Result = (GameState_Report)await ProcessState.ProcessGameState(new GameManager_Data());

                if (Result.GetReportStatus == false)
                {
                    Debug.LogError($"LoopStop {State}");
                    if (Result.GetReportString != null) Debug.LogError($"{Result.GetReportString}");
                    return;
                }
                else if (Result.GetReportString != null)
                {
                    Debug.LogError($"LoopStop System Message {Result.GetReportString}");
                }

            }
            // Run Starter Complate.
            // Start MainGame State.
            await Continue_GameState();
        }

        private async Task Continue_GameState()
        {
            var Bool = await test();

            for (int i = 0; i < 0; i++)
            {

            }
        }

        private async Task<bool> test()
        {
            await Task.Delay(1000);
            return true;
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