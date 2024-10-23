using UnityEngine;
using Coup_Mobile.EventBus;
using Coup_Mobile.InGame.GameManager.ReportData;
using UnityEditor.iOS;
using System;

namespace Coup_Mobile.InGame.GameManager
{
    public enum GameAssistManager_List
    {
        None,
        CommandList_Assist,
        TimerSystem_Assist,
        StateSystem_Assist,
        PlayerInfo_Assist,
        MiniMap_Assist,
        ChatSystem_Assist,

        GetInstall_Complate,
    }

    public class GameAssistManager
    {
        public GameManager gameManager { get; private set; }

        #region  Component and Prefabs Field

        // Game Command Ui Assist.
        private GameObject abilityUI_TargetBoxPrefab;
        private GameObject abilityUI_NotifyBoxPrefab;
        private GameObject abilityUI_AbilityBoxPrefab;

        // Position Ref Path
        private Transform abilityUI_ParentLocationPath;
        private Transform abilityUI_PositionOfVerifyBox;
        private Transform abilityUI_PositionOfTargetContent;
        private Transform abilityUI_PositionOfAbilityContent;

        // Ability And Target List Control.
        private Transform abilityUI_Targetinstance_Control;
        private Transform abilityUI_Abilityinstnace_Control;
        private Transform abilityIU_DisplayCardInstance_Control;

        // Toggle Button.
        private Transform abilityUI_InsideToggleButton;
        private Transform abilityUI_OutSide_ToggleButton;

        //------------------------------------------------------//

        // Game MiniMap UI Assist.

        // Button Event.
        private Transform miniMapUI_OptionButton;
        private Transform miniMapUI_OverViewButton;

        // Position Ref Path.
        private Transform miniMapUI_DisplaySceen_Path;

        //------------------------------------------------------//

        // Game PlayerInfo Ui Assist.
        private Transform playerInfo_LogoImage;
        private Transform playerInfo_CardAnountText;
        private Transform playerInfo_CoinAnountText;

        //------------------------------------------------------//

        // Game TimerInfo Ui Assist.
        private Transform timerUIInstance_Control;

        //------------------------------------------------------//

        // Game StateInfo Ui Assist.
        private GameObject[] stateUI_IconState_Prefabs;
        private Transform stateUI_ParentLocationPath;
        private Transform stateUI_StateWaveInstance_Control;
        private Transform stateUI_FixPosition_LocaltaionPath;
        private Transform stateUI_EnableIConState_LocationPath;
        private Transform stateUI_BackGround_Path;

        //------------------------------------------------------//

        // Game Chat UI Assist.
        private GameObject chat_Messagebox_Prefab;
        private GameObject chat_PlayerList_Prefab;
        private Transform chat_EnterMessage_Button;
        private Transform chat_MessageList_ScrollRect;
        private Transform chat_ScollVertical_Scrollbar;
        private Transform chat_InputMessage_InputField;
        private Transform chat_TagList_ScrollRect;
        private Transform chat_ToggleChat_Button;
        private Transform chat_AlphaOpacity_CanvasGroup;
        private Transform chat_MessagePath_Transform;
        private Transform chat_TagPlayerPath_Transform;

        #endregion

        private bool install_Complate = false;

        public GameAssistManager(GameManager gameManager)
        {
            this.gameManager = gameManager;

            LoadAssist();
        }

        private void LoadAssist()
        {
            try
            {
                Load_CommandList_Assist();

                Load_TimerControl_Assist();

                Load_StateDisplayControl_Assist();

                Load_PlayerInfoDisplay_Assist();

                Load_ChatInfoControl_Assist();

                Load_MiniMap_DisPlayControl_Assist();

                install_Complate = true;
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex);

                install_Complate = false;
            }
        }

        #region Load Component Function

        private void Load_CommandList_Assist()
        {
            abilityUI_TargetBoxPrefab = Resources.Load<GameObject>("Target_Sort");
            abilityUI_AbilityBoxPrefab = Resources.Load<GameObject>("CommandList_Sort");
            abilityUI_NotifyBoxPrefab = Resources.Load<GameObject>("Norify_Interactive/VerifyReponse_Box");

            abilityUI_ParentLocationPath = GameObject.Find("InGane_Canvas/Ui_Interface/CommandInfo").transform;

            abilityUI_InsideToggleButton = abilityUI_ParentLocationPath.Find("CloseButton");
            abilityUI_Targetinstance_Control = abilityUI_ParentLocationPath.Find("TargetView");
            abilityUI_PositionOfVerifyBox = abilityUI_ParentLocationPath.Find("Notify_Position");
            abilityUI_Abilityinstnace_Control = abilityUI_ParentLocationPath.Find($"CommandView");
            abilityUI_OutSide_ToggleButton = abilityUI_ParentLocationPath.parent.Find("Command_Button");
            abilityUI_PositionOfTargetContent = abilityUI_ParentLocationPath.Find("TargetView/Viewport/Content");
            abilityUI_PositionOfAbilityContent = abilityUI_ParentLocationPath.Find($"CommandView/Viewport/Content");
            //abilityIU_DisplayPreviewCand_Control = GameObject.Find("InGane_Canvas/Ui_Interface/PlayerInfo/Card_Display").transform;
            abilityIU_DisplayCardInstance_Control = abilityUI_ParentLocationPath.parent.Find("PlayerInfo/Card_Display").transform;
        }

        private void Load_TimerControl_Assist()
        {
            timerUIInstance_Control = GameObject.Find("InGane_Canvas/Ui_Interface/StateInfo/TimerDisplay").transform;
        }

        private void Load_StateDisplayControl_Assist()
        {
            stateUI_IconState_Prefabs = Resources.LoadAll<GameObject>("State_UI/");

            Transform StateInfo_Path = GameObject.Find("InGane_Canvas/Ui_Interface/StateInfo").transform;

            stateUI_StateWaveInstance_Control = StateInfo_Path;
            stateUI_FixPosition_LocaltaionPath = StateInfo_Path.Find("StateDisplay/Position").transform;
            stateUI_EnableIConState_LocationPath = StateInfo_Path.Find("StateDisplay/StateObject").transform;
            stateUI_BackGround_Path = StateInfo_Path.Find("StateDisplay/StateObject").transform;
        }

        private void Load_ChatInfoControl_Assist()
        {
            // Load Prefab
            chat_Messagebox_Prefab = Resources.Load<GameObject>("Chat/TextBox");
            chat_PlayerList_Prefab = Resources.Load<GameObject>("Chat/PlayerList");

            Transform Chat_Path = GameObject.Find("InGane_Canvas/Ui_Interface/ChatInfo").transform;

            chat_AlphaOpacity_CanvasGroup = Chat_Path;
            chat_ToggleChat_Button = Chat_Path.Find("ListButton");
            chat_EnterMessage_Button = Chat_Path.Find("EnterButton");
            chat_MessageList_ScrollRect = Chat_Path.Find("Scroll View");
            chat_InputMessage_InputField = Chat_Path.Find("InPutMessage");
            chat_TagList_ScrollRect = Chat_Path.Find("ListPlayer/Scrollbar Vertical");
            chat_MessagePath_Transform = Chat_Path.Find("Scroll View/Viewport/Content");
            chat_ScollVertical_Scrollbar = Chat_Path.Find("Scroll View/Scrollbar Vertical");
            chat_TagPlayerPath_Transform = Chat_Path.Find("ListPlayer/Scroll View/Viewport/Content");
        }

        private void Load_MiniMap_DisPlayControl_Assist()
        {
            Transform MiniMap_Path = GameObject.Find("InGane_Canvas/Ui_Interface/MiniMap").transform;

            miniMapUI_DisplaySceen_Path = MiniMap_Path.Find("Map");
            miniMapUI_OptionButton = MiniMap_Path.Find("Option");
            miniMapUI_OverViewButton = MiniMap_Path.Find("PlayerInfo");
        }

        private void Load_PlayerInfoDisplay_Assist()
        {
            stateUI_ParentLocationPath = GameObject.Find("InGane_Canvas/Ui_Interface/PlayerInfo").transform;

            playerInfo_LogoImage = stateUI_ParentLocationPath.Find("Character_Image");
            playerInfo_CardAnountText = stateUI_ParentLocationPath.Find("CardAnount");
            playerInfo_CoinAnountText = stateUI_ParentLocationPath.Find("CoinAnount");
        }

        #endregion

        public GameAssistManager_Return GameAssistManager_Control(GameManager_Data Request_Data)
        {
            GameAssistManager_Return Return_GameAssist = new GameAssistManager_Return();

            GameAssistManager_List EndPoint = (GameAssistManager_List)Request_Data.EndPoint;
            string Target = Request_Data.PacketData is string
                ? (string)Request_Data.PacketData
                : throw new ArgumentException($"GameAssistManager -> GameAssistManager_Control | Target Is not String Type.");

            object Return_Packet = null;
            switch (EndPoint)
            {
                case GameAssistManager_List.CommandList_Assist:
                    Return_Packet = ProcessGameCommand_Assist(Target);
                    break;
                case GameAssistManager_List.TimerSystem_Assist:
                    Return_Packet = ProcessTimerCommand_Assist(Target);
                    break;
                case GameAssistManager_List.StateSystem_Assist:
                    Return_Packet = ProcessState_Assist(Target);
                    break;
                case GameAssistManager_List.PlayerInfo_Assist:
                    Return_Packet = ProcessPlayerInfo_Assist(Target);
                    break;
                case GameAssistManager_List.MiniMap_Assist:
                    Return_Packet = ProcessMiniMap_Assist(Target);
                    break;
                case GameAssistManager_List.ChatSystem_Assist:
                    Return_Packet = ProcessChat_Assist(Target);
                    break;
                case GameAssistManager_List.GetInstall_Complate:
                    Return_Packet = install_Complate;
                    break;
            }
            if (Return_Packet != null)
            {
                Return_GameAssist = new GameAssistManager_Return
                {
                    requestCommand_Reult = true,
                    requestType = EndPoint,
                    return_Data = Return_Packet,
                };

                return Return_GameAssist;
            }

            Return_GameAssist.QuicklyReturn_False(GameAssistManager_List.None);

            return Return_GameAssist;
        }

        private object ProcessGameCommand_Assist(string target)
        {
            return target switch
            {
                // Prefab Zone.
                "AbilityBoxPrefab" => abilityUI_AbilityBoxPrefab,
                "TargetBoxPrefab" => abilityUI_TargetBoxPrefab,
                "NotifyBoxPrefab" => abilityUI_NotifyBoxPrefab,

                // Position Of Content.
                "PositionOfAbilityContent" => abilityUI_PositionOfAbilityContent,
                "ParentLocationPath" => abilityUI_ParentLocationPath,
                "PositionOfVerifyBox" => abilityUI_PositionOfVerifyBox,
                "PositionOfTargetContent" => abilityUI_PositionOfTargetContent,

                //Instance Control.
                "Abilityinstnace_Control" => abilityUI_Abilityinstnace_Control,
                "DisplayCardInstance_Control" => abilityIU_DisplayCardInstance_Control,
                "Targetinstance_Control" => abilityUI_Targetinstance_Control,

                // Button Event.
                "OutSide_ToggleButton" => abilityUI_OutSide_ToggleButton,
                "InsideToggleButton" => abilityUI_InsideToggleButton,
                _ => throw new ArgumentException($"{target} is not support This Type."),
            };
        }

        private object ProcessTimerCommand_Assist(string target)
        {
            return target switch
            {
                "Instance" => timerUIInstance_Control,
                _ => throw new ArgumentException($"{target} is not support This Type."),
            };
        }

        private object ProcessState_Assist(string target)
        {
            return target switch
            {
                "Instance" => stateUI_StateWaveInstance_Control,
                "Position_Path" => stateUI_FixPosition_LocaltaionPath,
                "StateObject_Path" => stateUI_EnableIConState_LocationPath,
                "Prefubs" => stateUI_IconState_Prefabs,
                "StateBackGround" => stateUI_BackGround_Path,
                _ => throw new ArgumentException($"{target} is not support This Type."),
            };
        }

        private object ProcessPlayerInfo_Assist(string target)
        {
            return target switch
            {
                "CardInfo_Text" => playerInfo_CardAnountText,
                "CoinInfo_Text" => playerInfo_CoinAnountText,
                "Parent_Image" => playerInfo_LogoImage,
                _ => throw new ArgumentException($"{target} is not support This Type."),
            };
        }

        private object ProcessMiniMap_Assist(string target)
        {
            return target switch
            {
                "MiniMap" => miniMapUI_DisplaySceen_Path,
                "OptionButton" => miniMapUI_OptionButton,
                "OverViewButton" => miniMapUI_OverViewButton,
                _ => throw new ArgumentException($"{target} is not support This Type."),
            };
        }

        private object ProcessChat_Assist(string target)
        {
            return target switch
            {
                "MessageBox_Prefab" => chat_Messagebox_Prefab,
                "PlayerList_Prefab" => chat_PlayerList_Prefab,
                "EnterMessageButton" => chat_EnterMessage_Button,
                "MessageList" => chat_MessageList_ScrollRect,
                "ScollVerical" => chat_ScollVertical_Scrollbar,
                "InputMessageBox" => chat_InputMessage_InputField,
                "TagList" => chat_TagList_ScrollRect,
                "ToggleChat" => chat_ToggleChat_Button,
                "AlphaOpacity" => chat_AlphaOpacity_CanvasGroup,
                "MessagePath" => chat_MessagePath_Transform,
                "TagPlayerPath" => chat_TagPlayerPath_Transform,
                _ => throw new ArgumentException($"{target} is not support This Type."),
            };

        }


    }

    public struct GameAssistManager_Return : IInGameEvent
    {
        public bool requestCommand_Reult;
        public GameAssistManager_List requestType;
        public object return_Data;

        public void QuicklyReturn_False(GameAssistManager_List Type)
        {
            requestCommand_Reult = false;
            requestType = Type;
            return_Data = null;
        }
    }
}
