using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Coup_Mobile.InGame.GameManager.Ui
{
    public class UI_WaveGame_Control : GameUI_Herder, IGameUi_Controller
    {
        private Display_UiController stateDisplayInstance;

        private Transform positionPath;
        private Transform objectPath;
        private Transform backGroundPath;
        private GameObject[] prefabs;
        private GameObject[] prefabs_Instance;


        public UI_WaveGame_Control(GameUiManager gameUiManager) : base(gameUiManager)
        {
            InstallSystem();
        }

        protected async override void InstallSystem()
        {
            GameManager_Event Event = GameManager_Event.GameAssistManager;
            object EndPoint = GameAssistManager_List.StateSystem_Assist;
            RequestParams requestParams = new RequestParams(Event, EndPoint);

            stateDisplayInstance = CallRequest<Transform>(requestParams, "Instance").GetComponent<StateDisplay_Control>();
            positionPath = CallRequest<Transform>(requestParams, "Position_Path");
            objectPath = CallRequest<Transform>(requestParams, "StateObject_Path");
            backGroundPath = CallRequest<Transform>(requestParams, "StateBackGround");
            prefabs = CallRequest<GameObject[]>(requestParams, "Prefubs");

            await Sort_StateDisplay();

            await Register_ActionEvent();
        }

        private async Task Register_ActionEvent()
        {
            await Task.Delay(0);

            stateDisplayInstance.StarterAndSetting(null);
        }

        private async Task Sort_StateDisplay()
        {
            GameManager_Event Event = GameManager_Event.GameStateManager;
            object EndPoint = GameStateManager_List.GetAll_MainState;
            RequestParams requestParams = new RequestParams(Event, EndPoint);

            List<GameState_List> AllStateDisplay = CallRequest<List<GameState_List>>(requestParams);

            prefabs_Instance = new GameObject[AllStateDisplay.Count];

            for (int i = 0; i < AllStateDisplay.Count; i++)
            {
                GameState_List State = AllStateDisplay[i];

                GameObject Selection_Prefabs = null;
                //Debug.LogWarning(State);
                switch (State)
                {
                    case GameState_List.Next_Wave_State:
                        Selection_Prefabs = Find_StatePrefabs("NextWaveAction");
                        break;
                    case GameState_List.Player_Send_Command:
                        Selection_Prefabs = Find_StatePrefabs("SendAction");
                        break;
                    case GameState_List.Player_Counter_Command:
                        Selection_Prefabs = Find_StatePrefabs("CounterAction");
                        break;
                    case GameState_List.Player_Verifiy_Command:
                        Selection_Prefabs = Find_StatePrefabs("VerifyAction");
                        break;
                    case GameState_List.Player_Result_Command:
                        Selection_Prefabs = Find_StatePrefabs("ResultAction");
                        break;
                    case GameState_List.Wait_For_AllPlayerAndProcessGame:
                        Selection_Prefabs = Find_StatePrefabs("WaitAction");
                        break;
                    default:
                        Debug.LogError($"{State} is not been set.");
                        break;
                }

                if (Selection_Prefabs == null)
                {
                    Debug.LogError($"Selection_Prefabs is null.");
                    return;
                }

                await Task.Delay(0);

                GameObject State_Instance = UnityEngine.Object.Instantiate<GameObject>(Selection_Prefabs, objectPath);

                State_Instance.name = State_Instance.name.Replace("(Clone)" , "");

                prefabs_Instance[i] = State_Instance;
            }
        }



        private GameObject Find_StatePrefabs(string statePrefabs_Name)
        {
            foreach (GameObject Item in prefabs)
            {
                if (Item.name == statePrefabs_Name)
                {
                    return Item;
                }
            }

            return null;
        }

        public override GameUI_ReturnData OnRequest_UI(GameUI_RequestData requestData)
        {
            try
            {
                ValidateRequestData(requestData, "OnRequest_UI");

                return ProcessUI_Request(requestData);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        private GameUI_ReturnData ProcessUI_Request(GameUI_RequestData requestData)
        {
            bool isSuccess = false;
            string topic = requestData.request_Topic[0];
            string target = requestData.request_Topic[1];

            switch (topic)
            {
                case "WaveInfo":
                    break;
                // Add More Request In Here.
                default: throw CreateException.Invoke(this, $"Unknown ProcessUI_Request topic : {topic}", "ProcessUI_Request");
            }

            return Create_ReturnData(isSuccess, false, null);
        }

        private bool ProcessWaveInfo(string target, object packetData)
        {
            bool isSuccess = false;

            switch (target)
            {
                case "Next":
                    break;
                case "Before":
                    break;
                case "JumpTo":
                    break;
                // Add More Request In Here.
                default : throw CreateException.Invoke(this , $"Unknown ProcessWaveInfo target : {target}" , "ProcessWaveInfo");
            }

            return isSuccess;
        }

        public override GameUI_ReturnData OnReturnStatus_UI(GameUI_RequestData getData)
        {
            try
            {
                ValidateRequestData(getData, "OnReturnStatus_UI");

                return ProcessReturn_Request(getData);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        private GameUI_ReturnData ProcessReturn_Request(GameUI_RequestData getData)
        {
            object returnData = null;
            string topic = getData.request_Topic[0];
            string target = getData.request_Topic[1];

            switch (topic)
            {
                case "":
                    break;
                // Add More Request In Here.
                default: throw CreateException.Invoke(this, $"Unknown ProcessReturn_Request topic : {topic}", "ProcessReturn_Request");
            }

            return Create_ReturnData(returnData, false, null);
        }

        public override GameUI_ReturnData OnUpdateData_UI(GameUI_RequestData updateData)
        {
            bool Process_Status = false;
            Func<string, Exception> CombineException = (_) => { throw CreateException.Invoke(this, _, "OnUpdateData_UI"); };

            try
            {
                object PacketData = updateData.packetData;

                string Topic_Request = updateData.request_Topic[0];
                string Process_Target = updateData.request_Topic[1];

                if (!Check_PacketDataType<string>(Topic_Request)) CombineException.Invoke("MainTopic is null.");
                if (!Check_PacketDataType<string>(Process_Target)) CombineException.Invoke("Process Target is null.");

                string[] ET =
                {
                     $"Topic Request :"                                 // 0
                   , $"Process Target :"                                // 1
                   , "form OnUpdateData_UI is not Installed in System."    // 2
                };

                switch (Topic_Request)
                {
                    case "":

                        switch (Process_Target)
                        {
                            case "":
                                break;
                            //Add More Request In Here.
                            default: throw CombineException.Invoke($"{ET[0]} {Topic_Request} {ET[1]} {Process_Target} {ET[2]}");
                        }

                        break;
                    //Add More Request In Here.
                    default: throw CombineException.Invoke($"{ET[0]} {Topic_Request} {ET[2]}");
                }

                return Create_ReturnData(Process_Status, false, null);
            }
            catch (Exception ex)
            {
                string Exception_message = ex.Message != string.Empty ? ex.Message : "None";

                var Report = Create_ReturnData(false, true, Exception_message);

                return Report;
            }
        }

        private GameUI_ReturnData ProcessUpdateData_Request(GameUI_RequestData updateData)
        {
            bool isSuccess = false;
            string topic = updateData.request_Topic[0];
            string target = updateData.request_Topic[1];

            switch (topic)
            {
                case "":
                    break;
                // Add More Request In Here.
                default: throw CreateException.Invoke(this, $"Unknown ProcessUpdateData_Request topic : {topic}", "ProcessUpdateData_Request");
            }

            return Create_ReturnData(isSuccess, false, null);
        }

        public override GameUI_ReturnData OnToggleActive_UI(GameUI_RequestData toggleActive)
        {
            try
            {
                ValidateRequestData(toggleActive, "OnToggleActiveUI");

                return ProcessToggleAcitve_Request(toggleActive);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        private GameUI_ReturnData ProcessToggleAcitve_Request(GameUI_RequestData toggleactive)
        {
            bool isSuccess = false;
            string topic = toggleactive.request_Topic[0];
            string target = toggleactive.request_Topic[1];

            switch (topic)
            {
                case "":
                    break;
                // Add More Request In Here.
                default: throw CreateException.Invoke(this, $"Unknown ProcessToggleAcitve_Request topic : {topic}", "ProcessToggleAcitve_Request");
            }

            return Create_ReturnData(isSuccess, false, null);
        }
    }
}