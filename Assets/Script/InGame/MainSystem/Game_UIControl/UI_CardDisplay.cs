using System;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

namespace Coup_Mobile.InGame.GameManager.Ui
{
    public class UI_CardDisplay : GameUI_Herder, IGameUi_Controller
    {
        #region Component And Anonymous

        // Component Ui.
        private Text cardDisplay_UI;
        private Text coinDisplay_Ui;
        private Image logoDisplay_Ui;

        #endregion

        #region Starter And Load Assist

        public UI_CardDisplay(GameUiManager gameUiManager) : base(gameUiManager)
        {
            this.gameUimanager = gameUiManager;

            InstallSystem();
        }
        protected override async void InstallSystem()
        {
            await Task.Delay(0);

            await Load_Assist();
        }

        private async Task Load_Assist()
        {
            // Defining the event path and endpoint for requesting components from the GameAssistManager.
            GameManager_Event EventPath = GameManager_Event.GameAssistManager;
            object EndPoint = GameAssistManager_List.PlayerInfo_Assist;

            // Creates the request parameters to fetch the desired UI elements.
            RequestParams requestParams = new RequestParams(EventPath, EndPoint);

            // Fetches the UI text components for card and coin display, and the logo image.
            cardDisplay_UI = CallRequest<Transform>(requestParams, "CardInfo_Text").GetComponent<Text>();

            coinDisplay_Ui = CallRequest<Transform>(requestParams, "CoinInfo_Text").GetComponent<Text>();

            logoDisplay_Ui = CallRequest<Transform>(requestParams, "Parent_Image").GetComponent<Image>();

            await Task.Delay(0);
        }

        #endregion

        #region UI Controller Function

        /// <summary>
        /// Handles UI requests and returns results based on the incoming request data.
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        public override GameUI_ReturnData OnRequest_UI(GameUI_RequestData requestData)
        {
            try
            {
                ValidateRequestData(requestData, "OnRequest_UI");

                return ProcessUIRequest(requestData);

            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Processes the incoming UI request and handles the logic for updating text or logo images.
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        private GameUI_ReturnData ProcessUIRequest(GameUI_RequestData requestData)
        {
            bool isSuccess = false;
            string topic = requestData.request_Topic[0];
            string target = requestData.request_Topic[1];

            switch (topic)
            {
                case "Change TextInfo":
                    isSuccess = ProcessTextInfo_Request(target, requestData.packetData);
                    break;
                case "Change LogoImage":
                    isSuccess = ProcessLogoInfo_Reuqest(target, requestData.packetData);
                    break;
                default:
                    throw CreateException.Invoke(this, $"Unknown Topic: {topic}", "OnRequest_UI");
            }

            return Create_ReturnData(isSuccess, isSuccess, null);
        }

        private bool ProcessTextInfo_Request(string target, object packetData)
        {
            string ChangeText = Check_PacketDataType<string>(packetData)
                     ? (string)packetData
                     : throw CreateException.Invoke(this, "Text Value is Null", "ProcessChangeDisplayRequest");

            Text ChangeTextValue = GetTextInfo(target);

            ChangeTextValue.text = ChangeText;

            return true;
        }

        private Text GetTextInfo(string target)
        {
            return target switch
            {
                "CardText" => cardDisplay_UI,
                "CoinText" => coinDisplay_Ui,
                _ => throw CreateException.Invoke(this, $"Unknown TextInfo Target : {target} ", "GetChangeTextInfo")
            };
        }

        private bool ProcessLogoInfo_Reuqest(string target, object packetData)
        {
            Sprite ChangeImage = Check_PacketDataType<Sprite>(packetData)
                    ? (Sprite)packetData
                    : throw CreateException.Invoke(this, "Image is Null", "ProcessLogoInfo_Reuqest");

            Image ChangeImageLogo = GetImageInfo(target);

            ChangeImageLogo.sprite = ChangeImage;

            return true;
        }

        private Image GetImageInfo(string target)
        {
            return target switch
            {
                "ProfileImage" => logoDisplay_Ui,
                _ => throw CreateException.Invoke(this, $"Unknown Image Target : {target} ", "GetChangeImageInfo")
            };
        }

        public override GameUI_ReturnData OnReturnStatus_UI(GameUI_RequestData getData)
        {
            try
            {
                ValidateRequestData(getData, "OnReturnStatsus_UI");

                return ProcessReturnRequest(getData);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        private GameUI_ReturnData ProcessReturnRequest(GameUI_RequestData getData)
        {
            object returnData = null;
            string topic = getData.request_Topic[0];
            string target = getData.request_Topic[1];

            switch (topic)
            {
                case "GetTextItem":
                    returnData = GetTextInfo(target);
                    break;
                case "GetImageItem":
                    returnData = GetImageInfo(target);
                    break;
                // Add More Request In Here.
                default: throw CreateException.Invoke(this, $"Unknow Topic : {topic}", "ProcessReturnRequest");
            }

            if (returnData == null) Create_ReturnData(null, true, $"OnReturnStatus_UI -> ProcessReturnReqeust ReturnData is null");

            return Create_ReturnData(returnData, false, null);
        }

        public override GameUI_ReturnData OnToggleActive_UI(GameUI_RequestData toggleActive)
        {

            try
            {
                ValidateRequestData(toggleActive, "OnToggleActive_UI");

                return ProcessToggleRequest(toggleActive);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }

        }

        private GameUI_ReturnData ProcessToggleRequest(GameUI_RequestData toggleActive)
        {
            bool isSuccess = false;
            string topic = toggleActive.request_Topic[0];
            string target = toggleActive.request_Topic[1];

            switch (topic)
            {
                case "":
                    break;
                // Add More Request In Here.
                default: throw CreateException.Invoke(this, $"Unknown topic : {topic}", "ProcessToggleRequest");
            }

            return Create_ReturnData(isSuccess, isSuccess, null);
        }

        public override GameUI_ReturnData OnUpdateData_UI(GameUI_RequestData updateData)
        {
            try
            {
                ValidateRequestData(updateData, "OnUpdateData_UI");

                return ProcessUpdateRequest(updateData);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        private GameUI_ReturnData ProcessUpdateRequest(GameUI_RequestData updateData)
        {
            bool isSuccess = false;
            string topic = updateData.request_Topic[0];
            string target = updateData.request_Topic[1];

            switch (topic)
            {
                case "Update Text Component":

                    Text NewText = Check_PacketDataType<Text>(updateData.packetData)
                         ? (Text)updateData.packetData
                         : throw CreateException.Invoke(this, "OnUpdate_UI -> ProcessUpdateRerquest PacketData is not Text type.", "ProcessUpdateRequest");

                    Text textUpdate = GetTextInfo(target);

                    textUpdate = NewText;

                    isSuccess = true;

                    break;
                case "Update Image Component":

                    Image NewImage = Check_PacketDataType<Image>(updateData.packetData)
                        ? (Image)updateData.packetData
                        : throw CreateException.Invoke(this, "OnUpdate_UI -> ProcessUpdateRerquest PacketData is not Image type.", "ProcessUpdateRequest");

                    Image ImageUpdate = GetImageInfo(target);

                    ImageUpdate = NewImage;

                    isSuccess = true;

                    break;
            }

            return Create_ReturnData(isSuccess, isSuccess, null);
        }
    }

    #endregion

}

