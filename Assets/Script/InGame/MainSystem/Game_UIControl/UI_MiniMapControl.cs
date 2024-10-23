using System;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

namespace Coup_Mobile.InGame.GameManager.Ui
{
    public class UI_MiniMap_Control : GameUI_Herder, IGameUi_Controller
    {
        private Button overView_Button;
        private Button option_Button;

        public UI_MiniMap_Control(GameUiManager gameUiManager) : base(gameUiManager)
        {
            this.gameUimanager = gameUiManager;

            InstallSystem();
        }
        protected async override void InstallSystem()
        {
            await Load_Assist();
        }

        private async Task Load_Assist()
        {
            GameManager_Event EventPath = GameManager_Event.GameAssistManager;
            object EndPoint = GameAssistManager_List.MiniMap_Assist;
            RequestParams requestParams = new RequestParams(EventPath, EndPoint);

            // OverView Componnet.
            overView_Button = CallRequest<Transform>(requestParams, "OverViewButton").GetComponent<Button>();

            // Option Button.
            option_Button = CallRequest<Transform>(requestParams, "OptionButton").GetComponent<Button>();

            await Task.Delay(0);
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
                case "Button Event":
                    isSuccess = ProcessButtonEvent(target, requestData.packetData);
                    break;
                case "Map Event":
                    isSuccess = ProcessMapEvent(target, requestData.packetData);
                    break;
                // Add More Request In Here.
                default: throw CreateException.Invoke(this, $"Unknown ProcessUI_Request Topic: {topic}", "ProcessUIRequest");
            }

            return Create_ReturnData(isSuccess, false, null);
        }

        private bool ProcessButtonEvent(string target, object packetData)
        {
            return target switch
            {
                "OpenOverView" => false,
                "CloseOverView" => false,
                "OpenOption" => false,
                "CloseOption" => false,
                _ => throw CreateException.Invoke(this, $"Unknown ProcessButtonEvent Target : {target}", "ProcessButtonEvent"),
            };
        }

        private bool ProcessMapEvent(string target, object packetData)
        {
            return target switch
            {
                "MoveToPoint" => false,
                _ => throw CreateException.Invoke(this, $"Unknown ProcessMapEvent Target : {target}", "ProcessMapEvent"),
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

        private GameUI_ReturnData ProcessReturn_Request(GameUI_RequestData returnData)
        {
            object ReturnData = null;
            string topic = returnData.request_Topic[0];
            string target = returnData.request_Topic[1];

            switch (topic)
            {
                case "MapInfo":
                    ReturnData = ProcessMapInfo(target, returnData.packetData);
                    break;
                // Add More Request In Here.
                default: throw CreateException.Invoke(this, $"Unknown Topic : {topic}", "ProcessReturn_Request");
            }

            if (ReturnData == null) throw CreateException.Invoke(this, "ReturnData is Null.", "ProcessReturn_Request");

            return Create_ReturnData(ReturnData, false, null);
        }

        private object ProcessMapInfo(string target, object packetData)
        {
            return target switch
            {
                "GetLocation" => null,
                "GetClickToPointLocaltion" => null,
                // Add More Request In Here.
                _ => throw CreateException.Invoke(this, $"Unknown ProcessMapInfo Target : {target}", "ProcessMapInfo"),
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
                default: throw CreateException.Invoke(this, $"Unknown topic target {target}", "ProcessToggleActive_Request");
            }

            return Create_ReturnData(isSuccess, false, null);
        }

        public override GameUI_ReturnData OnUpdateData_UI(GameUI_RequestData updateData)
        {
            try
            {
               ValidateRequestData(updateData , "OnUpdateData_UI");

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
                case "Change Button":
                    
                    Button NewButton = Check_PacketDataType<Button>(updateData.packetData) 
                        ? (Button)updateData.packetData 
                        : throw CreateException.Invoke(this , "Packetet is not Button type." , "ProcessUpdateData_Request");

                    Button OldButton = GetButton_Component(target);

                    OldButton = NewButton;

                    isSuccess = true;

                    break;
            }

            return Create_ReturnData(isSuccess , false , null);
        }

        private Button GetButton_Component(string target)
        {
            return target switch
            {
                "OverView" => overView_Button,
                "Option" => option_Button,
                _ => throw CreateException.Invoke(this , $"Unknown GetButtonComponent target {target}" , "GetButton_Component"),
            };
        }
    }
}