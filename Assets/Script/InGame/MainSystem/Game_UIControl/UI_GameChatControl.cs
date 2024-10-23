using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Coup_Mobile.InGame.GameManager.Ui
{
    public class UI_GameChat_Control : GameUI_Herder, IGameUi_Controller
    {
        #region Component And Anonymous
        private GameObject messageboxPrefab;
        private GameObject playerlistPrefab;
        private Scrollbar messageList;
        private Button enterMessage;
        private Button tagList;
        private Slider scrollVerical;
        private CanvasGroup tableAlpha;
        private Transform contentPath;
        private Transform playerListPath;

        // Field Control.
        private Dictionary<string, MessageBoxControl> allMessage_Box = new Dictionary<string, MessageBoxControl>();
        private Dictionary<string, TagPlayer> allTagPlayer_Box = new Dictionary<string, TagPlayer>();


        #endregion

        #region Stater And Load Assist

        public UI_GameChat_Control(GameUiManager gameUiManager) : base(gameUiManager)
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
            object EndPoint = GameAssistManager_List.ChatSystem_Assist;

            RequestParams requestParams = new RequestParams(EventPath, EndPoint);

            // Text Message List.
            messageList = CallRequest<Transform>(requestParams, "MessageList").GetComponent<Scrollbar>();

            // Send Message Button.
            enterMessage = CallRequest<Transform>(requestParams, "EnterMessageButton").GetComponent<Button>();

            // Scroll Text Message List.
            scrollVerical = CallRequest<Transform>(requestParams, "ScollVerical").GetComponent<Slider>();

            // Toggle Tag to Some Players.
            tagList = CallRequest<Transform>(requestParams, "ToggleChat").GetComponent<Button>();

            // Message Box Opacity.
            tableAlpha = CallRequest<Transform>(requestParams, "AlphaOpacity").GetComponent<CanvasGroup>();

            // Content Collect Path.
            contentPath = CallRequest<Transform>(requestParams, "MessagePath").GetComponent<Transform>();

            // Tag Some Player Path.
            playerListPath = CallRequest<Transform>(requestParams, "TagPlayerPath").GetComponent<Transform>();

            // MessageBox Prefab.
            messageboxPrefab = CallRequest<GameObject>(requestParams, "MessageBox_Prefab");

            // PlayerList Prefab.
            playerlistPrefab = CallRequest<GameObject>(requestParams, "PlayerList_Prefab");

            await Task.Delay(0);
        }

        #endregion

        #region Controller Function

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

            string UiData_Header = string.Empty;

            if (Check_PacketDataType<ChatMessage_Data>(requestData.packetData))
            {
                ChatMessage_Data MessageData = (ChatMessage_Data)requestData.packetData;

                switch (topic)
                {
                    case "MessageBox Insert":
                        isSuccess = ProcessMessageBox_Insert(target, MessageData);
                        break;
                    case "MessageBox Remove":
                        isSuccess = ProcessMessageBox_Remove(target, MessageData);
                        break;
                    // Add More Request In Here.
                    default:
                        throw CreateException.Invoke
                            (this, $"Unknown ChatMessage Topic: {topic}", "ProcessUIRequest");
                }
            }
            else if (Check_PacketDataType<TagPlayerList_Data>(requestData.packetData))
            {
                TagPlayerList_Data TagPlayerData = (TagPlayerList_Data)requestData.packetData;

                switch (topic)
                {
                    case "TagPlayerList Insert":
                        isSuccess = ProcessTagPlayerList_Insert(target, TagPlayerData);
                        break;
                    case "TagPlayerList Remove":
                        isSuccess = ProcessTagPlayerList_Remove(target, TagPlayerData);
                        break;
                    // Add More Request In Here.
                    default:
                        throw CreateException.Invoke(this, $"Unknown TagPlayerList Topic: {topic}", "ProcessUIRequest");
                }
            }

            return Create_ReturnData(isSuccess, isSuccess, null);
        }

        private bool ProcessMessageBox_Insert(string target, ChatMessage_Data insertChat)
        {
            bool isSuccess = false;

            if (!CheckHeaderTopic_Requestment(insertChat, "AddMessage")) throw CreateException.Invoke
                    (this, $"ChatMessage is not Create under by 'AddMessage' Header.", "ProcessMessageBox_Insert");

            if (insertChat.specificPlayer == null)
            {
                isSuccess = AddMessage(insertChat.sender, insertChat.message, insertChat.typeMessage);

                // RPC : Notify All Player.
            }
            else
            {
                isSuccess = AddMessage(insertChat.sender, insertChat.message, insertChat.typeMessage);

                // RPC : Notify Specific Player.
            }

            return isSuccess;
        }

        private bool ProcessMessageBox_Remove(string target, ChatMessage_Data removeChat)
        {
            bool isSuccess = false;

            if (!CheckHeaderTopic_Requestment(removeChat, "RemoveMessage")) throw CreateException.Invoke
                    (this, $"ChatMessage is not Create under by 'RemoveMessage' Header.", "ProcessMessageBox_Insert");

            if (removeChat.TargetMessage == null)
            {
                isSuccess = RemoveAllMessageBox();

                // RPC : Notify All Player.
            }
            else
            {
                Dictionary<string, MessageBoxControl> targetRemove = !removeChat.TargetMessage.Equals(typeof(ChatMessage_Data))
                    ? removeChat.TargetMessage
                    : throw CreateException.Invoke
                        (this, "ChatMessage the TargetRemove Data is null.", "ProcessMessageBox_Remove");


                isSuccess = RemoveMessageBoxSelect(targetRemove);

                // RPC : Notify Specific Player.
            }

            return isSuccess;
        }

        private bool ProcessTagPlayerList_Insert(string target, TagPlayerList_Data insertTagList)
        {
            bool isSuccess = false;

            if (!CheckHeaderTopic_Requestment(insertTagList, "AddTagList")) throw CreateException.Invoke
                (this, $"ChatMessage is not Create under by 'AddTagList' Header.", "ProcessTagPlayerList_Insert");

            isSuccess = AddPlayerLists(insertTagList.tag_Player);

            return isSuccess;
        }

        private bool ProcessTagPlayerList_Remove(string target, TagPlayerList_Data removeTagList)
        {
            bool isSuccess = false;

            if (!CheckHeaderTopic_Requestment(removeTagList, "RemoveList")) throw CreateException.Invoke
               (this, $"ChatMessage is not Create under by 'RemoveTagList' Header.", "ProcessTagPlayerList_Insert");

            if (removeTagList.tag_Player == null)
            {
                isSuccess = RemoveAllTagPlayerList();
            }
            else
            {
                isSuccess = RemoveTagPlayerList_Select(removeTagList.tag_Player);
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
                case "MessageBox":
                    returnData = ProcessReturn_MessageBox(target, getData.packetData);
                    break;
                case "TagPlayerList":
                    returnData = ProcessReturn_TagPlayerList(target, getData.packetData);
                    break;
                // Add More Request In Here.
                default: throw CreateException.Invoke(this, $"Unknown TagPlayerList Topic: {topic}", "ProcessReturn_Request");
            }

            return Create_ReturnData(returnData, false, null);
        }

        private object ProcessReturn_MessageBox(string target, object PacketData)
        {
            return target switch
            {
                "GetMessageList" => allMessage_Box.Count > 0 ? allMessage_Box : null,
                "FindMessageSelect" => allTagPlayer_Box.Count > 0 ? allTagPlayer_Box : null,

                // Add More Request In Here.
                _ => throw CreateException.Invoke(this, $"Unknown MessageBox Target : {target}", "ProcessReturn_MessageBox"),
            };
        }

        private object ProcessReturn_TagPlayerList(string target, object PacketData)
        {
            return target switch
            {
                "FindTagPlayerSelect" => FindPlayerListSelect(PacketData is string
                    ? (string)PacketData
                    : throw CreateException.Invoke(this, $"Packet Data is null.", "ProcessReturn_TagPlayerList")),
                "GetTagPlayerList" => allTagPlayer_Box.Count > 0 ? allTagPlayer_Box : null,

                // Add More Request In Here.
                _ => throw CreateException.Invoke(this, $"Unknown TagPlayerList Target : {target}", "ProcessReturn_TagPlayerList"),
            };
        }

        public override GameUI_ReturnData OnToggleActive_UI(GameUI_RequestData toggleActive)
        {
            try
            {
                ValidateRequestData(toggleActive, "OnToggleActive");
                return ProcessToggle_Request(toggleActive);

            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        private GameUI_ReturnData ProcessToggle_Request(GameUI_RequestData toggleActive)
        {
            bool isSuccess = false;
            string topic = toggleActive.request_Topic[0];
            string target = toggleActive.request_Topic[1];

            switch (topic)
            {
                case "":
                    break;
                // Add More Request In Here.
                default: throw CreateException.Invoke(this, $"Unknown ToggleAcitve topoc : {topic} ", "ProcessToggle_Request");
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
                case "Update Component":
                    var UpdateComponent = GetUpdateData_Component(target , updateData.packetData);

                    Text OldText = UpdateComponent is Text 
                        ? (Text)UpdateComponent
                        : throw CreateException.Invoke(this , "Update Component is support only Text type." , "ProcessUpdateData_Request");
                    break;
                // Add More Request In Here.
                default: throw CreateException.Invoke(this , $"Unknown UpdateData topic : {topic}" , "ProcessUpdateData_Request");
            }

            return Create_ReturnData(isSuccess , false , null);
        }

        private object GetUpdateData_Component(string target , object packetData)
        {
            return target switch
            {
                "EnterMessage" => enterMessage,
                "TagList" => tagList,
                "MessageList" => messageList,
                _ => throw CreateException.Invoke(this, $"Unknown GetUpdateData target : {target}" , "GetUpdateData_Component")
            };
        }

        #endregion

        #region Local Function

        #region Requestment Function

        private bool AddMessage(string sender, string message, int MessageType = 0)
        {
            try
            {
                GameObject NewMessage = UnityEngine.Object.Instantiate(messageboxPrefab, contentPath);
                MessageBoxControl messageBox = NewMessage.GetComponent<MessageBoxControl>();

                messageBox.AddMessage(sender, message, MessageType);

                // Add In Collections.
                allMessage_Box.Add(sender, messageBox);

                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                return false;
            }
        }

        private bool AddPlayerLists(string PlayerName)
        {
            try
            {
                GameObject NewTagPlayer = UnityEngine.Object.Instantiate(playerlistPrefab, playerListPath);
                TagPlayer TagPlayerBox = NewTagPlayer.GetComponent<TagPlayer>();

                TagPlayerBox.SetupPlayerList(PlayerName);

                // Add In Collection.
                allTagPlayer_Box.Add(PlayerName, TagPlayerBox);

                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                return false;
            }
        }

        private Dictionary<string, MessageBoxControl> FindMessageSelect(string senderName)
        {
            if (allMessage_Box.Count <= 0)
            {
                return null;
            }

            Dictionary<string, MessageBoxControl> PlayerMessageSelected = new Dictionary<string, MessageBoxControl>();

            try
            {

                foreach (var item in allMessage_Box)
                {
                    if (item.Key == senderName)
                    {
                        PlayerMessageSelected.Add(item.Key, item.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                return null;
            }

            return PlayerMessageSelected;
        }

        private Dictionary<string, TagPlayer> FindPlayerListSelect(string PlayerName)
        {
            if (allTagPlayer_Box.Count <= 0)
            {
                return null;
            }

            Dictionary<string, TagPlayer> PlayerListSelected = new Dictionary<string, TagPlayer>();

            try
            {
                foreach (var item in allTagPlayer_Box)
                {
                    if (item.Key == PlayerName)
                    {
                        PlayerListSelected.Add(item.Key, item.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                return null;
            }

            return PlayerListSelected;
        }

        private bool RemoveMessageBoxSelect(Dictionary<string, MessageBoxControl> TargetRemove)
        {
            if (allMessage_Box.Count <= 0)
            {
                return false;
            }

            int beforeRemove = allMessage_Box.Count;

            Dictionary<string, MessageBoxControl> TargetRemoveSelected = allMessage_Box.ToDictionary(x => x.Key, y => y.Value);

            string[] TargetRemoveString = TargetRemove.Keys.ToArray();

            foreach (var item in TargetRemove)
            {
                if (TargetRemoveSelected.ContainsKey(item.Key))
                {
                    TargetRemoveSelected.Remove(item.Key);
                    MonoBehaviour.Destroy(item.Value);
                }
            }

            // Replace Message field.
            allMessage_Box = TargetRemoveSelected;

            if (allMessage_Box.Count == beforeRemove) return false;

            return true;



        }

        private bool RemoveAllMessageBox()
        {
            if (allMessage_Box.Count <= 0)
            {
                return false;
            }

            try
            {

                Dictionary<string, MessageBoxControl> TargetRemove = allMessage_Box.ToDictionary(x => x.Key, y => y.Value);

                foreach (var item in TargetRemove)
                {
                    MonoBehaviour.Destroy(item.Value);
                }

                allMessage_Box.Clear();

                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                return false;
            }
        }

        private bool RemoveTagPlayerList_Select(string TagName)
        {
            if (allTagPlayer_Box.Count <= 0)
            {
                return false;
            }

            try
            {
                Dictionary<string, TagPlayer> TargetRemove = allTagPlayer_Box.ToDictionary(x => x.Key, y => y.Value);

                foreach (var item in allTagPlayer_Box)
                {
                    if (item.Key == TagName)
                    {
                        TargetRemove.Remove(item.Key);
                        MonoBehaviour.Destroy(item.Value);
                    }
                }

                allTagPlayer_Box = TargetRemove;

                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                return false;
            }
        }

        private bool RemoveAllTagPlayerList()
        {
            if (allTagPlayer_Box.Count <= 0)
            {
                return false;
            }

            try
            {
                Dictionary<string, TagPlayer> TargetRemove = allTagPlayer_Box.ToDictionary(x => x.Key, y => y.Value);

                foreach (var item in TargetRemove)
                {
                    MonoBehaviour.Destroy(item.Value);
                }

                allTagPlayer_Box.Clear();

                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                return false;
            }
        }

        private bool CheckHeaderTopic_Requestment(object Request_Data, string isCorrentHeader)
        {
            if (Request_Data is ChatMessage_Data ChatHeader) return ChatHeader.headerTopic == isCorrentHeader;
            else if (Request_Data is TagPlayerList_Data TagHeader) return TagHeader.headerTopic == isCorrentHeader;
            else return false;
        }
    }

    #endregion

    #endregion


}

public struct ChatMessage_Data
{
    public string headerTopic;

    // Add Message Field.
    public string sender;
    public string message;
    public int typeMessage;
    public string specificPlayer;

    public ChatMessage_Data(string sender, string message, int Type, string specificPlayer)
    {
        headerTopic = "AddMessage";

        this.sender = sender;
        this.message = message;
        this.typeMessage = Type;
        this.specificPlayer = specificPlayer;

        TargetMessage = null;
    }

    // Remove Message Field.

    public Dictionary<string, MessageBoxControl> TargetMessage;

    public ChatMessage_Data(Dictionary<string, MessageBoxControl> TargetMessage)
    {
        headerTopic = "RemoveMessage";

        this.TargetMessage = TargetMessage;

        sender = null;
        message = null;
        typeMessage = 0;
        specificPlayer = null;
    }

}

public struct TagPlayerList_Data
{
    public string headerTopic;

    public string tag_Player;

    public TagPlayerList_Data(string Players, bool isAddLists)
    {
        if (isAddLists)
        {
            headerTopic = "AddTagList";

            tag_Player = Players;
        }
        else if (!isAddLists)
        {
            headerTopic = "RemoveTagList";

            tag_Player = Players;
        }
        else
        {
            headerTopic = string.Empty;

            tag_Player = null;
        }

    }
}
