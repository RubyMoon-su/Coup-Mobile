using UnityEngine;
using System.Threading.Tasks;
using Coup_Mobile.InGame.GameManager.ReportData;

namespace Coup_Mobile.InGame.GameManager.GameState
{
    public class InitializationChecker_GameState : GameState, IGameState_Strategy , IOtherPlayerResponsive_Oberver
    {
        public InitializationChecker_GameState(GameStateManager gameStateManager) : base(gameStateManager)
        {
            Debug.Log($"{this.GetType().Name} Created.");

            // Register Network Notify.
            gameStateManager.Attach(this);
        }

        public async Task<object> ProcessGameState(object PacketData)
        {
            GameState_List EndPoint = GameState_List.Check_Component_And_GameEvent;


            if (PacketData is GameManager_Data)
            {

                try
                {
                    await Task.Delay(0);

                    int Timer = 0;
                    int System_TimeOut = GameManager.SYSTEM_TIMEOUT;

                    // Check Install Complate GameManager.
                    while (!GameManager.GetInstall_ComponentStatus)
                    {
                        await Task.Delay(1000);

                        Timer++;

                        if (Timer == System_TimeOut)
                        {
                            UnityEngine.Debug.LogError("Can't Install Game Manager Complete.");
                            return new GameState_Report(EndPoint, null, false, "Can't  Install Game Manager Complete.");
                        }
                    }

                    bool System_InstalledStatus = await CheckAllGameManager_Process();
                    if (!System_InstalledStatus) return new GameState_Report(EndPoint, null, false , null);
                    

                    // Return Report To Next State.
                    return new GameState_Report(EndPoint, null, true, null);
                }
                catch (System.Exception ex)
                {
                    // Failed Process State.
                    return new GameState_Report(EndPoint, null, false, "CheckGameEvent " + ex.Message);
                }
            }

            // Incorrect Type.
            return new GameState_Report(EndPoint, null, false, "Data Type Incorrect");
        }

        private async Task<bool> CheckPlayerManager_SettingComplate()
        {
            int Timer = 0;

            int TimeCount = GameManager.SYSTEM_TIMEOUT;
            GameManager_Event EventPath = GameManager_Event.PlayerManager;
            PlayerManager_List EndPoint = PlayerManager_List.Get_Install_Complate;

            bool Check_PlayerManager_Status = false;
            while (!Check_PlayerManager_Status)
            {
                PlayerManager_Return PM_Result = (PlayerManager_Return)Request_Event(EventPath, EndPoint, null);

                Check_PlayerManager_Status = (bool)PM_Result.return_Data;

                await Task.Delay(1000);

                Timer++;

                if (Timer >= TimeCount)
                {
                    Debug.LogError("Can't Install Player Manager Complete.");
                    return false;
                }
            }

            return true;

        }

        private async Task<bool> CheckGameResourceManager_SettingComplate()
        {
            int Timer = 0;

            int TimeCount = GameManager.SYSTEM_TIMEOUT;
            GameManager_Event EventPath = GameManager_Event.GameResourceManager;
            ResourceManager_List EndPoint = ResourceManager_List.GetInstall_Complate;

            bool Check_GameResource_Status = false;

            while (!Check_GameResource_Status)
            {
                GameResource_Result GR_Result = (GameResource_Result)Request_Event(EventPath, EndPoint, null);

                Check_GameResource_Status = (bool)GR_Result.return_Data;

                await Task.Delay(1000);

                Timer++;

                if (Timer >= TimeCount)
                {
                    Debug.LogError("Can't Install Game Resource Manager Complete.");
                    return false;
                }
            }

            return true;
        }

        private async Task<bool> CheckGameUiManager_SettingComplate()
        {
            int Timer = 0;

            int TimeCount = GameManager.SYSTEM_TIMEOUT;
            GameManager_Event EventPath = GameManager_Event.GameUiManager;
            GameUIManager_List EndPoint = GameUIManager_List.GetInstall_Complate;

            bool Check_GameUiManager_Status = false;

            while (!Check_GameUiManager_Status)
            {
                GameUIManager_Return GUM_Result = (GameUIManager_Return)Request_Event(EventPath, EndPoint, null);

                Check_GameUiManager_Status = (bool)GUM_Result.return_Data;

                await Task.Delay(1000);

                Timer++;

                if (Timer >= TimeCount)
                {
                    Debug.LogError("Can't Insall Game Ui Manager Complate");
                    return false;
                }
            }

            return true;
        }

        private async Task<bool> CheckGameNetowrkManager_SettingComplate()
        {
            int Timer = 0;

            int TimeCount = GameManager.SYSTEM_TIMEOUT;
            GameManager_Event EventPath = GameManager_Event.GameNetworkManager;
            GameNetworkManager_List EndPoint = GameNetworkManager_List.GetInstall_Complate;

            bool Check_GameNetworkManager_Status = false;

            var Network_Request = new GameNetwork_Requestment();
            Network_Request.SettingDataToDefault();

            while (!Check_GameNetworkManager_Status)
            {
                GameNetworkManager_Return GNM_Result = (GameNetworkManager_Return)Request_Event(EventPath, EndPoint, Network_Request);

                Check_GameNetworkManager_Status = (bool)GNM_Result.return_Data;

                await Task.Delay(1000);

                Timer++;

                if (Timer >= TimeCount)
                {
                    Debug.LogError("Can't Insall Game Network Manager Complate");
                    return false;
                }
            }

            return true;
        }

        private async Task<bool> CheckGameStateManager_SettingComplate()
        {
            int Timer = 0;

            int TimeCount = GameManager.SYSTEM_TIMEOUT;
            GameManager_Event EventPath = GameManager_Event.GameStateManager;
            GameStateManager_List EndPoint = GameStateManager_List.GetInstall_Complate;

            bool Check_GameStateManager_Status = false;

            while (!Check_GameStateManager_Status)
            {
                GameStateManager_Return GSM_Result = (GameStateManager_Return)Request_Event(EventPath, EndPoint, string.Empty);

                Check_GameStateManager_Status = (bool)GSM_Result.return_Data;

                await Task.Delay(1000);

                Timer++;

                if (Timer >= TimeCount)
                {
                    Debug.LogError("Can't Insall Game State Manager Complate");
                    return false;
                }
            }

            return true;
        }

        private async Task<bool> CheckGameAssistManager_SettingComplate()
        {
            int Timer = 0;

            int TimeCount = GameManager.SYSTEM_TIMEOUT;
            GameManager_Event EventPath = GameManager_Event.GameAssistManager;
            GameAssistManager_List EndPoint = GameAssistManager_List.GetInstall_Complate;

            bool Check_GameAssistManager_Status = false;

            while (!Check_GameAssistManager_Status)
            {
                GameAssistManager_Return GAM_Result = (GameAssistManager_Return)Request_Event(EventPath, EndPoint, string.Empty);

                Check_GameAssistManager_Status = (bool)GAM_Result.return_Data;

                await Task.Delay(1000);

                Timer++;

                if (Timer >= TimeCount)
                {
                    Debug.LogError("Can't Insall Game Assist Manager Complate");
                    return false;
                }
            }

            return true;
        }

        private async Task<bool> CheckAllGameManager_Process()
        {
            bool Installing_Result = true;

            // Check Install Complate PlayerManager.
            bool CPM_Result = await CheckPlayerManager_SettingComplate();

            // Check Install Complate GameResourceManager.
            bool CGRM_Result = await CheckGameResourceManager_SettingComplate();

            // Check Install Complate Game UI Manager.
            bool CGUM_Result = await CheckGameUiManager_SettingComplate();

            // Check Install Game Network Manager.
            bool CGNM_Result = await CheckGameNetowrkManager_SettingComplate();

            // Check Install Game State Manager.
            bool CGSM_Result = await CheckGameStateManager_SettingComplate();

            // Check Install Game Assist Manager.
            bool CGAM_Result = await CheckGameAssistManager_SettingComplate();

            if (!CPM_Result || !CGRM_Result || !CGUM_Result || !CGNM_Result || !CGSM_Result || !CGAM_Result)
            {
                Installing_Result = false;
                string Exception_Message = $"System Not Complate CPM_Result = {CPM_Result} | CGRM_Result = {CGRM_Result} | CGUM_Result = {CGUM_Result} | CGNM_Result = {CGNM_Result} | CGSM_Result = {CGSM_Result} | CGAM_Result = {CGAM_Result}";
                Debug.LogError(Exception_Message);
            }

            return Installing_Result;
        }
        #region  Update Form Network

        public void UpdateFormNetwork(object PacketData)
        {
            //Not Do anything.
        }

        #endregion
    }
}