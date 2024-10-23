using System;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using System.Collections.Generic;
using Coup_Mobile.InGame.PlayerData;

namespace Coup_Mobile.InGame.GameManager.Ui
{

    public class UI_GameCommand_Control : GameUI_Herder, IGameUi_Controller
    {
        // Toggle Button UI.
        private Button outside_ToggleButton;
        private Button inside_ToggleButton;

        // Prefab.
        private GameObject verifyAbility_Prefab;
        private GameObject abilityList_Prefab;
        private GameObject targetList_Prefab;

        // Location of ability prefabs.
        private Transform positionOfAbility_LocaltionPath;
        private Transform positionOfTarget_LocationPath;
        private Transform positionOfParent_LocationPath;
        private Transform positionOfVerifyAbility_LocationPath;
        private Transform positionOfAbilityDisable_LocationPath;

        // MonoBehaviour Ui Controller. 
        private Display_UiController abilityInstance_Controller;
        private Display_UiController targetInstance_Controller;
        private Display_UiController CardPreviewInstance_Controller;

        // Pool Command System.
        protected List<Transform /*Ability Interactive*/> collectionAbility_Sort = new List<Transform>();

        public UI_GameCommand_Control(GameUiManager gameUiManager) : base(gameUiManager)
        {
            UnityEngine.Debug.Log("UI_GameCommand Created.");

            InstallSystem();
        }

        protected override void InstallSystem()
        {
            try
            {
                // Reference Path With Short.
                object EndPoint = GameAssistManager_List.CommandList_Assist;
                GameManager_Event EventPath = GameManager_Event.GameAssistManager;
                RequestParams RequestHeader = new RequestParams(EventPath, EndPoint);

                // Load GameObject Prefab
                targetList_Prefab = CallRequest<GameObject>(RequestHeader, "TargetBoxPrefab");
                abilityList_Prefab = CallRequest<GameObject>(RequestHeader, "AbilityBoxPrefab");
                verifyAbility_Prefab = CallRequest<GameObject>(RequestHeader, "NotifyBoxPrefab");

                // Localtion Content Path.
                positionOfParent_LocationPath = CallRequest<Transform>(RequestHeader, "ParentLocationPath");
                positionOfTarget_LocationPath = CallRequest<Transform>(RequestHeader, "PositionOfTargetContent");
                positionOfVerifyAbility_LocationPath = CallRequest<Transform>(RequestHeader, "PositionOfVerifyBox");
                positionOfAbility_LocaltionPath = CallRequest<Transform>(RequestHeader, "PositionOfAbilityContent");

                // Instance Controller.
                targetInstance_Controller = CallRequest<Transform>(RequestHeader, "Targetinstance_Control").GetComponent<TargetSelection_Control>();
                abilityInstance_Controller = CallRequest<Transform>(RequestHeader, "Abilityinstnace_Control").GetComponent<AbilitySelection_Control>();
                CardPreviewInstance_Controller = CallRequest<Transform>(RequestHeader, "DisplayCardInstance_Control").GetComponent<Display_PreviewCard>();

                // Button Event.
                outside_ToggleButton = CallRequest<Transform>(RequestHeader, "OutSide_ToggleButton").GetComponent<Button>();
                inside_ToggleButton = CallRequest<Transform>(RequestHeader, "InsideToggleButton").GetComponent<Button>();

                CardPreviewInstance_Controller.StarterAndSetting(null);

                Sort_AbilityDisplay();
                Sort_TargetDisplay();
                Button_Install();

                positionOfParent_LocationPath.gameObject.SetActive(false);
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex.Message);
            }
        }

        private void Button_Install()
        {
            GameObject GO = positionOfParent_LocationPath.gameObject;

            outside_ToggleButton.onClick.AddListener(() => GO.SetActive(true));
            inside_ToggleButton.onClick.AddListener(() => GO.SetActive(false));
        }

        private void Sort_AbilityDisplay()
        {
            // Reference Path With Short.
            GameManager_Event Event = GameManager_Event.GameResourceManager;
            var EndPoint = ResourceManager_List.GetAbilit_CanUse;
            RequestParams requestParams = new RequestParams(Event, EndPoint);

            List<string> AbliltyName = CallRequest<List<string>>(requestParams);
            // Create Game Prefab and Setup Button.>

            List<Transform> AllAbility = new List<Transform>();
            List<Button> Interactable_Button = new List<Button>();
            for (int i = 0; i < AbliltyName.Count; i++)
            {
                GameObject CommandList = UnityEngine.Object.Instantiate(abilityList_Prefab, positionOfAbility_LocaltionPath);

                CommandList.name = AbliltyName[i];

                Image color = CommandList.GetComponent<Image>();
                Text text = CommandList.GetComponentInChildren<Text>();
                Button button = CommandList.GetComponent<Button>();

                text.text = AbliltyName[i];

                button.onClick.AddListener(async () => await SendAbility_Function(CommandList.name));

                CommandList.transform.SetParent(positionOfAbilityDisable_LocationPath);

                collectionAbility_Sort.Add(CommandList.transform);
                Interactable_Button.Add(button);
            }

            AbilitySwap_Starter();

            Dictionary<string, object> SettingAbility = new Dictionary<string, object>()
            {
                {"MaxniumIndex" , collectionAbility_Sort.Count - 1}
                ,{"MinniumIndex" , 0}
                ,{"PositionObject" , collectionAbility_Sort.ToArray()}
                ,{"ButtonObject" , Interactable_Button.ToArray()}
            };

            abilityInstance_Controller.StarterAndSetting(SettingAbility);
        }

        private void Sort_TargetDisplay()
        {
            // Reference Path With Short.
            GameManager_Event Event = GameManager_Event.PlayerManager;
            var EndPoint = PlayerManager_List.Get_AllPlayerData;
            RequestParams requestParams = new RequestParams(Event, EndPoint);

            var TargetName = CallRequest<Dictionary<int, Player_Data>>(requestParams);
            List<Transform> targetLists = new List<Transform>();

            foreach (var data in TargetName.Values)
            {
                string PlayerName = data.playerName;

                GameObject targetList = UnityEngine.Object.Instantiate(targetList_Prefab, positionOfTarget_LocationPath);

                targetList.name = PlayerName;

                Text playerName_Text = targetList.GetComponentInChildren<Text>();

                playerName_Text.text = PlayerName;

                targetLists.Add(targetList.transform);
            }

            Dictionary<string, object> SettingTargetDisplay = new Dictionary<string, object>()
            {
                 {"MaxniumIndex" , TargetName.Count - 1 < 0 ? 1 : TargetName.Count - 1}
                ,{"MinniumIndex" , 0}
                ,{"PositionObject" , targetLists.ToArray()}
            };

            targetInstance_Controller.StarterAndSetting(SettingTargetDisplay);

        }

        public async Task SendAbility_Function(string abilityName)
        {
            Debug.LogError(abilityName);

            GameObject NotifyBox = UnityEngine.Object.Instantiate(verifyAbility_Prefab, positionOfVerifyAbility_LocationPath);

            Display_UiController NotifyBox_Control = NotifyBox.GetComponentInChildren<VerifySelection_BoxControl>();

            string targetList = (string)targetInstance_Controller.ReturnExecute("GetisTargetSelected", null);

            VerifyBox_Data SettingVerifyBox = new VerifyBox_Data("You want to use this command?", $"Opration is {abilityName} and Target is {targetList} ", "Excute", "Cancel");

            NotifyBox_Control.StarterAndSetting(SettingVerifyBox);

            bool isReponse = false;

            do
            {
                await Task.Delay(500);

                isReponse = (bool)NotifyBox_Control.ReturnExecute("GetIsReponse", null);

                Debug.LogError("Wait for Result.");
            }
            while (!isReponse);

            int Result = (int)NotifyBox_Control.ReturnExecute("GetReponseResult", null);

            switch (Result)
            {
                case 1:
                    Debug.LogError("Yes");
                    break;
                case 2:
                    Debug.LogError("No");
                    break;
                default: throw CreateException.Invoke(this, $"Unknown Numeric {Result}", "SendAbility_Function");
            }

            UnityEngine.GameObject.Destroy(NotifyBox);

        }

        private void AbilitySwap_Starter()
        {
            for (int i = 0; i < collectionAbility_Sort.Count; i++)
            {
                collectionAbility_Sort[i].transform.SetParent(positionOfAbility_LocaltionPath);
            }
        }

        #region Request Function

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

        private GameUI_ReturnData ProcessUIRequest(GameUI_RequestData requestData)
        {
            bool isSuccess = false;
            string topic = requestData.request_Topic[0];
            string target = requestData.request_Topic[1];

            switch (topic)
            {
                case "RequestCommandByAction":
                    isSuccess = RequestCommandByActionDisplay(target, requestData.packetData);
                    break;
                // Add More Request In Here.
                default: throw CreateException.Invoke(this, $"Unknown Topic Target : {target}", "ProcessUIRequest");
            }

            return Create_ReturnData(isSuccess, false, null);
        }

        private bool RequestCommandByActionDisplay(string target, object PacketData)
        {
            RequestCommandAction_Data RequestOrder = PacketData is RequestCommandAction_Data
                ? (RequestCommandAction_Data)PacketData
                : throw CreateException.Invoke(this, $"RequestData : {PacketData.GetType()} is not RequestCommandAction_Data type.", "ProcessRequestCommandByAction");

            if (RequestOrder.Equals(typeof(RequestCommandAction_Data)))
                throw CreateException.Invoke(this, "RequestData is null.", "ProcessRequestCommandByAction");

            return target switch
            {
                "RequestCommand" => Request_CommandDisplay(RequestOrder),
                "RequestCounterCommand" => RequestCounter_CommandDisplay(RequestOrder),
                "RequestVerifyCommand" => Verify_CommandDisplay(RequestOrder),
                "CounterCommand" => Counter_CommandDisplay(RequestOrder),
                "CounterVerifyCommand" => CounterVerify_CommandDisplay(RequestOrder),
                _ => throw CreateException.Invoke(this, $"Unknown RequestCommandByAction Target : {target}", "ProcessRequestCommandByAction")
            };
        }

        private bool Request_CommandDisplay(RequestCommandAction_Data requestCommand)
        {
            bool isSuccess = false;

            return isSuccess;
        }

        private bool RequestCounter_CommandDisplay(RequestCommandAction_Data requestCounter)
        {
            bool isSuccess = false;

            return isSuccess;
        }

        private bool Counter_CommandDisplay(RequestCommandAction_Data counterCommand)
        {
            bool isSuccess = false;

            return isSuccess;
        }

        private bool CounterVerify_CommandDisplay(RequestCommandAction_Data counterVerify)
        {
            bool isSuccess = false;

            return isSuccess;
        }

        private bool Verify_CommandDisplay(RequestCommandAction_Data veriftCommand)
        {
            bool isSuccess = false;

            return isSuccess;
        }

        #endregion

        public override GameUI_ReturnData OnReturnStatus_UI(GameUI_RequestData getData)
        {
            try
            {
                ValidateRequestData(getData, "OnReturnStatis_UI");

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
                case "GetCommandOrTargetSelect":
                    ReturnData = ProcessCommandOrTargetSelect(target, getData.packetData);
                    break;
                // Add More Request In Here.
                default: throw CreateException.Invoke(this, $"Unknown Topic Target : {target}", "ProcessReturn_Request");
            }

            if (ReturnData == null) throw CreateException.Invoke(this, "ProcessReturn_Request Return Packet is Null.", "ProcessReturn_Request");

            return Create_ReturnData(ReturnData, false, null);
        }

        private object ProcessCommandOrTargetSelect(string target, object packetData)
        {
            return target switch
            {
                "SelectedCommand" => abilityInstance_Controller.ReturnExecute("SelectedCommand", null),
                "SelectedTarget" => abilityInstance_Controller.ReturnExecute("SelectedTarget", null),
                // Add More Request In Here.
                _ => throw CreateException.Invoke(this, $"Unknown CommandOrTargetSelect Target : {target}", "ProcessCommandOrTargetSelect")
            };
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
                case "Update Abilitys":
                    isSuccess = ProcessUpdateAbility(target, updateData.packetData);
                    break;
                case "Toggle ability":
                    break;
                // Add More Request In Here.
                default: throw CreateException.Invoke(this, $"Unknown Topic target : {topic}", "ProcessUpdateData_Request");
            }

            return Create_ReturnData(isSuccess, false, null);
        }

        private bool ProcessUpdateAbility(string target, object packetData)
        {
            return target switch
            {
                "AddAbility" => false,
                "AddTarget" => false,
                "RemoveAbility" => false,
                "RemoveTarget" => false,
                _ => throw CreateException.Invoke(this, $"Unknown ProcessUpdateAbility Target : {target}", "ProcessUpdateAbility"),
            };
        }

        public override GameUI_ReturnData OnToggleActive_UI(GameUI_RequestData toggleActive)
        {
            try
            {
                ValidateRequestData(toggleActive, "OnToggleActive_UI");

                return ProcessToggleAcitve_Request(toggleActive);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        private GameUI_ReturnData ProcessToggleAcitve_Request(GameUI_RequestData toggleActive)
        {
            bool isSuccess = false;
            string topic = toggleActive.request_Topic[0];
            string target = toggleActive.request_Topic[1];

            switch (topic)
            {
                case "":
                    break;
                // Add More Request In Here.
                default: throw CreateException.Invoke(this, $"Unknown Topic target : {topic}", "ProcessToggleAcitve_Request");
            }

            return Create_ReturnData(isSuccess, false, null);
        }


    }

    public struct RequestCommandAction_Data
    {
        public string headerTopic;

        #region  Normal Game Mode.

        // Field for the Request Commands.
        public string RC_RequestsCommand { get; set; }
        public string RC_OwerCommand { get; set; }
        public string RC_TargetCommand { get; set; }

        public RequestCommandAction_Data(string RC_RequestsCommand, string RC_OwerCommand, string RC_TargetCommand)
        {
            headerTopic = "RequestCommand";

            this.RC_RequestsCommand = RC_RequestsCommand;
            this.RC_TargetCommand = RC_TargetCommand;
            this.RC_OwerCommand = RC_OwerCommand;

            CC_CounterCommand = null;
            CC_TagetCounter = null;
        }

        // Field for the Counter Commands.
        public string CC_CounterCommand { get; set; }
        public string CC_TagetCounter { get; set; }

        // Field for the Verify Commands.

        #endregion

        #region Reformation Game Mode.

        // Field for the Request Change Team.

        #endregion

        #region Custorm Game Mode.

        #endregion
    }
}