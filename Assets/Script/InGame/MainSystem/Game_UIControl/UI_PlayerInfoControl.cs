using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace Coup_Mobile.InGame.GameManager.Ui
{
    public class UI_PlayerInfo_Control : GameUI_Herder, IGameUi_Controller
    {
        private PlayerInfo_Display playerInfo_Control;

        private Text amountCard_Display;
        private Text amountCoin_Display;
        private Transform imageLogo_Path;

        public UI_PlayerInfo_Control(GameUiManager gameUiManager) : base(gameUiManager)
        {
            InstallSystem();
        }

        protected override async void InstallSystem()
        {
            await Retrieve_Ref();
        }

        private async Task Retrieve_Ref()
        {
            GameManager_Event EventPath = GameManager_Event.GameAssistManager;
            object EndPoint = GameAssistManager_List.PlayerInfo_Assist;

            RequestParams requestParams = new RequestParams(EventPath, EndPoint);

            amountCard_Display = CallRequest<Transform>(requestParams, "CardInfo_Text").GetComponent<Text>();

            amountCoin_Display = CallRequest<Transform>(requestParams, "CoinInfo_Text").GetComponent<Text>();

            imageLogo_Path = CallRequest<Transform>(requestParams, "Parent_Image");

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
                case "":
                    break;
                // Add More Request In Here.
                default: throw CreateException.Invoke(this, $"Unknown ProcessUI_Request topic : {topic}", "ProcessUI_Request");
            }

            return Create_ReturnData(isSuccess, false, null);
        }

        public override GameUI_ReturnData OnReturnStatus_UI(GameUI_RequestData getData)
        {
            try
            {
                ValidateRequestData(getData, "OnRequestStatus_UI");

                return ProcessReturn_Request(getData);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        private GameUI_ReturnData ProcessReturn_Request(GameUI_RequestData getData)
        {
            object ReturnData = null;
            string topic = getData.request_Topic[0];
            string target = getData.request_Topic[1];

            switch (topic)
            {
                case "":
                    break;
                // Add More Request In Here.
                default: throw CreateException.Invoke(this, $"Unknown ProcessReturn_Request topic : {topic}", "ProcessReturn_Request");
            }

            if (ReturnData == null) throw CreateException.Invoke(this, "ReturnPacket is Null.", "ProcessReturn_Request");

            return Create_ReturnData(ReturnData, false, null);
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
                ValidateRequestData(toggleActive , "OnToggleActive_UI");

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

            return Create_ReturnData(isSuccess , false, null);
        }
    }
}