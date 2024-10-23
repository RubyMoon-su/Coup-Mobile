using System;
using UnityEngine;
using Coup_Mobile.InGame.GameManager;
using Coup_Mobile.InGame.GameManager.Ui;

namespace Coup_Mobile.InGame.UI
{
    public class UI_GameTimer_Control : GameUI_Herder, IGameUi_Controller
    {
        protected Display_UiController timeStateControl;

        public UI_GameTimer_Control(GameUiManager gameUiManager) : base(gameUiManager)
        {
            InstallSystem();
        }

        protected override void InstallSystem()
        {
            try
            {
                GameManager_Event Event = GameManager_Event.GameAssistManager;
                GameAssistManager_List EndPoint = GameAssistManager_List.TimerSystem_Assist;
                RequestParams requestParams = new RequestParams(Event, EndPoint);
                timeStateControl = CallRequest<Transform>(requestParams, "Instance").GetComponent<TimeState_Control>();
                timeStateControl.StarterAndSetting(null);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }
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
                case "RequestTimer":
                    isSuccess = ProcessRequest_Timer(target, requestData.packetData);
                    break;
                case "ChangeProperties":
                    isSuccess = ProcessChange_Properties(target, requestData.packetData);
                    break;
                // Add More Request In Here.
                default: throw CreateException.Invoke(this, $"Unknown ProcessUI_Request topic : {topic}", "ProcessUI_Request");
            }

            return Create_ReturnData(isSuccess, false, null);
        }

        private bool ProcessRequest_Timer(string target, object packetData)
        {
            try
            {
                timeStateControl.CommandExecute(target, packetData);
            }
            catch (ArgumentException arg)
            {
                Debug.LogError(arg);
                return false;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                return false;
            }

            return true;
        }

        private bool ProcessChange_Properties(string target, object packetData)
        {
            return target switch
            {
                "ChangeTimerSpeed" => false,
                "ChangeDecideColor" => false,
                "ChangeDecideTimer" => false,
                _ => throw CreateException.Invoke(this, $"Unknown ProcessChange_Properties Target : {target}", "ProcessChange_Properties"),
            };
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
            object returnData = false;
            string topic = getData.request_Topic[0];
            string target = getData.request_Topic[1];

            switch (topic)
            {
                case "GetTime":
                    returnData = ProcessReturn_Timer(target, getData.packetData);
                    break;
                case "GetProperties":
                    returnData = ProcessReturn_Properties(target, getData.packetData);
                    break;
                // Add More Request In Here.
                default: throw CreateException.Invoke(this, $"Unknown ProcessReturn_Request topic : {topic}", "ProcessReturn_Request");
            }

            return Create_ReturnData(returnData, false, null);
        }

        private object ProcessReturn_Timer(string target, object packetData)
        {
            return target switch
            {
                "CurrentTime" => null,
                "RemainingTime" => null,
                "RemainingTimeToRedZone" => null,
                "TotalTime" => null,
                "TotalTimeWithoutRedZone" => null,
                //Add More Request In Here.
                _ => throw CreateException.Invoke(this, $"Unknown ProcessReturn_Timer Target : {target}", "ProcessReturn_Timer"),
            };
        }

        private object ProcessReturn_Properties(string target, object packetData)
        {
            return target switch
            {
                "isTimerStop" => null,
                "isTimerRedZone" => null,
                // Add More Request In Here.
                _ => throw CreateException.Invoke(this, $"Unknown ProcessReturn_Properties Target : {target}", "ProcessReturn_Properties"),
            };
        }

        public override GameUI_ReturnData OnToggleActive_UI(GameUI_RequestData toggleActive)
        {
            try
            {
                ValidateRequestData(toggleActive, "OnToggleActive_UI");

                return ProcessToggleActive_Request(toggleActive);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        private GameUI_ReturnData ProcessToggleActive_Request(GameUI_RequestData toggleActive)
        {
            bool isSuccess = false;
            string topic = toggleActive.request_Topic[0];
            string target = toggleActive.request_Topic[1];

            switch (topic)
            {
                case "":
                    break;
                // Add More Request In Here.
                default: throw CreateException.Invoke(this, $"Unknown ProcessToggleActive_Request topic : {topic}", "ProcessToggleActive_Request");
            }

            return Create_ReturnData(isSuccess, false, null);
        }

        public override GameUI_ReturnData OnUpdateData_UI(GameUI_RequestData updateData)
        {
            try
            {
                ValidateRequestData(updateData, "OnUpdateData_UI");

                return ProcessUpdateData_Request(updateData);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        private GameUI_ReturnData ProcessUpdateData_Request(GameUI_RequestData updateData)
        {
            bool isSuccess = false;
            string topic = updateData.request_Topic[0];
            string target = updateData.request_Topic[1];

            switch (topic)
            {
                case "UpdateTimer":
                    isSuccess = ProcessUpdate_Timer(target, updateData.packetData);
                    break;
                // Add More Request In Here.
                default: throw CreateException.Invoke(this, $"Unknown ProcessUpdateData_Request topic : {topic}", "ProcessUpdateData_Request");
            }

            return Create_ReturnData(isSuccess, false, null);
        }

        private bool ProcessUpdate_Timer(string target, object packetData)
        {
            return target switch
            {
                "TimerText" => false,
                _ => throw CreateException.Invoke(this, $"Unknown ProcessUpdate_Timer Target : {target}", "ProcessUpdate_Timer"),
            };
        }
    }
}