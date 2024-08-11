using Photon.Pun;
using System.Threading.Tasks;
using Coup_Mobile.InGame.GameManager.ReportData;
using Coup_Mobile.EventBus;

namespace Coup_Mobile.InGame.GameManager.GameState
{
    public class GameState
    {
        private GameStateManager gameStateManager;

        public GameState(GameStateManager gameStateManager)
        {
            this.gameStateManager = gameStateManager;
        }
    }

    #region  Normal StarterGame State

    /// <summary>
    /// This Class is First Starter GameState.
    /// This Class is Check All System Component is Complate.
    /// </summary>
    public class CheckGameEvent_GameState : GameState, IGameState_Strategy
    {
        public CheckGameEvent_GameState(GameStateManager gameStateManager) : base(gameStateManager)
        {
            UnityEngine.Debug.Log($"{this.GetType().Name} Created.");
        }

        public async Task<object> ProcessGameState(object PacketData)
        {

            if (PacketData is GameManager_Data)
            {

                try
                {
                    await Task.Delay(2000);

                    int Timer = 0;
                    int System_TimeOut = GameManager.SYSTEM_TIMEOUT;

                    //Check Install Complate GameManager.
                    while (!GameManager.GetInstall_ComponentStatus)
                    {
                        await Task.Delay(1000);

                        Timer++;

                        if (Timer == System_TimeOut)
                        {
                            UnityEngine.Debug.LogError("Can't Install Game Manager Complete.");
                            return new GameState_Report(GameState_List.Setup_Player_Properties, null, false, "Can't  Install Game Manager Complete.");
                        }
                    }

                    Timer = 0;

                    //Check Install Complate PlayerManager.
                    while (!PlayerManager.GetStatus_InstallComplate)
                    {
                        await Task.Delay(1000);

                        Timer++;

                        if (Timer == System_TimeOut)
                        {
                            UnityEngine.Debug.LogError("Can't Install Player Manager Complete.");
                            return new GameState_Report(GameState_List.Setup_Player_Properties, null, false, "Can't  Install Player Manager Complete.");
                        }
                    }

                    Timer = 0;

                    GameManager_Data CheckInstallStatus_GameResource = new GameManager_Data
                    {
                        gameManager_Event = GameManager_Event.GameResourceManager,
                        EndPoint = ResourceManager_List.GetInstall_Complate,
                        PacketData = null,
                    };

                    bool Check_GameResource_Status = false;
                    //Check Install Complate GameResourceManager.
                    while (!Check_GameResource_Status)
                    {
                        GameResource_Result GR_Result = (GameResource_Result)EventBus_InGameManager<IInGameEvent>.RaiseGameCommand(CheckInstallStatus_GameResource);

                        Check_GameResource_Status = (bool)GR_Result.return_Data;

                        await Task.Delay(1000);

                        Timer++;

                        if (Timer >= System_TimeOut)
                        {
                            UnityEngine.Debug.LogError("Can't Install Game Resource Manager Complete.");
                            return new GameState_Report(GameState_List.Setup_Player_Properties, null, false, "Can't  Install Game Resource Manager Complete.");
                        }
                    }

                    Timer = 0;

                    // Check Install Complate Game UI Manager.

                    Timer = 0;

                    // Check Install Game Network Manager.

                    // Return Report To Next State.
                    return new GameState_Report(GameState_List.Check_Component_And_GameEvent, null, false, "Test Only Check Gane Event");
                }
                catch (System.Exception ex)
                {
                    // Failed Process State.
                    return new GameState_Report(GameState_List.Check_Component_And_GameEvent, null, false, ex.Message);
                }
            }

            // Incorrect Type.
            return new GameState_Report(GameState_List.Check_Component_And_GameEvent, null, false, "Data Type Incorrect");
        }
    }

    /// <summary>
    /// This Class Is Second Starter GameState.
    /// This Class Is Setup new Properties For Player And Server.
    /// </summary>
    public class SetupPlayerProperties_GameState : GameState, IGameState_Strategy
    {
        public SetupPlayerProperties_GameState(GameStateManager gameStateManager) : base(gameStateManager)
        {
            UnityEngine.Debug.Log($"{this.GetType().Name} Created.");
        }

        public async Task<object> ProcessGameState(object PacketData)
        {
            if (PacketData is GameManager_Data)
            {
                try
                {
                    await Task.Delay(0);
                    ExitGames.Client.Photon.Hashtable SetUp_Properties = new ExitGames.Client.Photon.Hashtable
                    {
                        {"PlayerCard" , 2}
                    };

                    PhotonNetwork.LocalPlayer.SetCustomProperties(SetUp_Properties);

                    return new GameState_Report(GameState_List.Check_Component_And_GameEvent, null, true, null);
                }
                catch (System.Exception ex)
                {
                    return new GameState_Report(GameState_List.Setup_Player_Properties, null, false, ex.Message);
                }
            }

            // Incorrect Type.
            return new GameState_Report(GameState_List.Setup_Player_Properties, null, false, "DataType Incorrect");
        }
    }

    /// <summary>
    /// This Class Is Third Stater GameState.
    /// This Class Is SetUp Item Character Coin etc, For All Player In Game.
    /// </summary>
    public class Process_PlayerSort_GameState : GameState, IGameState_Strategy
    {
        public Process_PlayerSort_GameState(GameStateManager gameStateManager) : base(gameStateManager)
        {
            UnityEngine.Debug.Log($"{this.GetType().Name} Created.");
        }

        public async Task<object> ProcessGameState(object PacketData)
        {
            if (PacketData is GameManager_Data GM_Data)
            {
                try
                {
                    if (PhotonNetwork.IsMasterClient)
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            UnityEngine.Debug.Log($"Process_PlayerSort Count {i}");

                            await Task.Delay(1000);
                        }

                        UnityEngine.Debug.Log("Loop Complate");

                        await Task.Delay(5000);


                        return new GameState_Report(GameState_List.Setup_Player_Properties, null, true, null);
                    }
                    else
                    {
                        while (true)
                        {

                        }
                    }
                }
                catch (System.Exception ex)
                {
                    return new GameState_Report(GameState_List.Setup_Player_Properties, null, false, ex.Message);
                }
            }

            return new GameState_Report(GameState_List.Setup_Player_Properties, null, false, "DataType Incorrect.");
        }
    }

    #endregion

    #region Reformation StarterGame State

    /// <summary>
    /// This Class Is Optionmal Starter GameState.
    /// This Class Is Runable if Ower Set GameMode is Reformation.
    /// This Class Is Setup PlayerTeam For All Player, First Player can Select Team For mySelf.
    /// </summary>
    public class SetUp_PlayerTeam_GameState : GameState, IGameState_Strategy
    {
        public SetUp_PlayerTeam_GameState(GameStateManager gameStateManager) : base(gameStateManager)
        {
            UnityEngine.Debug.Log($"{this.GetType().Name} Created.");
        }

        public async Task<object> ProcessGameState(object PacketData)
        {
            try
            {
                for (int i = 0; i < 10; i++)
                {
                    UnityEngine.Debug.Log($"SetUpPlayerTeam Count {i}");

                    await Task.Delay(1000);
                }

                UnityEngine.Debug.Log("Loop Complate");

                await Task.Delay(5000);

                return new GameState_Report(GameState_List.Check_Component_And_GameEvent, null, true, null);
            }
            catch (System.Exception ex)
            {
                return new GameState_Report(GameState_List.Setup_Player_Properties, null, false, ex.Message);
            }
        }
    }

    #endregion
}