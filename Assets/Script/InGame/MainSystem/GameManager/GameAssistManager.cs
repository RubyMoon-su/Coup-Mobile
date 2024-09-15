using UnityEngine;
using Coup_Mobile.EventBus;
using Coup_Mobile.InGame.GameManager.ReportData;

namespace Coup_Mobile.InGame.GameManager
{
    public enum GameAssistManager_List
    {
        None,
        CommandList_Assist,
        TimerSystem_Assist,
        StateSystem_Assist,

        GetInstall_Complate,
    }

    public class GameAssistManager
    {
        public GameManager gameManager { get; private set; }

        // Game Command Ui Assist.
        private GameObject abilityUI_Prefab;
        private Transform abilityUI_Content;
        private Transform abilityUI_Control;
        private Transform abilityUI_ToggleButton;

        // Game MiniMap UI Assist.
        private Transform miniMapInfo_OverViewButton;
        private Transform miniMapInfo_OptionButton;
        private Transform miniMapInfo_ViewPort;

        // Game PlayerInfo Ui Assist.
        private Transform playerInfo_LogoImage;
        private Transform playerInfo_CardAnountText;
        private Transform playerInfo_CoinAnountText;
        private Transform playerInfo_CardInfoButton;
        private Transform playerInfo_CenterOfCardDisplay;
        private Transform playerInfo_CardContentGameObject;

        // Game TimerInfo Ui Assist.
        private Transform timerDisplay_Instance;


        // Game StateInfo Ui Assist.
        private GameObject[] stateDisplay_Prefabs;
        private Transform stateDisplay_Instance;
        private Transform statePosition_Path;
        private Transform stateObject_Path;
        private Transform stateBackGround_Path;

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

                install_Complate = true;
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex);

                install_Complate = false;
            }
        }

        private void Load_CommandList_Assist()
        {
            abilityUI_Prefab = Resources.Load<GameObject>("CommandList_Sort");

            Transform CommandInfo_Path = GameObject.Find("InGane_Canvas/Ui_Interface/CommandInfo").transform; ;

            abilityUI_Content = CommandInfo_Path.Find($"Scroll View/Viewport/Content");
            abilityUI_Control = CommandInfo_Path.Find($"Scroll View");
            abilityUI_ToggleButton = CommandInfo_Path.Find("Command_Button");
        }

        private void Load_TimerControl_Assist()
        {
            timerDisplay_Instance = GameObject.Find("InGane_Canvas/Ui_Interface/StateInfo/TimerDisplay").transform;
        }

        private void Load_StateDisplayControl_Assist()
        {
            stateDisplay_Prefabs = Resources.LoadAll<GameObject>("State_UI/");

            Transform StateInfo_Path = GameObject.Find("InGane_Canvas/Ui_Interface/StateInfo").transform;

            stateDisplay_Instance = StateInfo_Path;
            statePosition_Path = StateInfo_Path.Find("StateDisplay/Position").transform;
            stateObject_Path = StateInfo_Path.Find("StateDisplay/StateObject").transform;
            stateBackGround_Path = StateInfo_Path.Find("StateDisplay/StateObject").transform;
        }

        private void Load_MiniMap_DisPlayControl_Assist()
        {
            Transform MiniMap_Path = GameObject.Find("InGane_Canvas/Ui_Interface/MiniMap").transform;

            miniMapInfo_ViewPort = MiniMap_Path.Find("Map");
            miniMapInfo_OptionButton = MiniMap_Path.Find("Option");
            miniMapInfo_OverViewButton = MiniMap_Path.Find("PlayerInfo");
        }

        private void Load_PlayerInfoDisplay_Assist()
        {
            Transform PlayerInfo_Path = GameObject.Find("InGane_Canvas/Ui_Interface/PlayerInfo").transform;

            playerInfo_LogoImage = PlayerInfo_Path.Find("Character_Image");
            playerInfo_CardAnountText = PlayerInfo_Path.Find("CardAnount");
            playerInfo_CoinAnountText = PlayerInfo_Path.Find("CoinAnount");
            playerInfo_CardInfoButton = PlayerInfo_Path.Find("CardInfoButton");
            playerInfo_CenterOfCardDisplay = PlayerInfo_Path.Find("Card_Display/CenterPos");
        }

        public GameAssistManager_Return GameAssistManager_Control(GameManager_Data Request_Data)
        {
            GameAssistManager_Return Return_GameAssist = new GameAssistManager_Return();
            object Return_Packet = null;

            GameAssistManager_List EndPoint;
            if (Request_Data.EndPoint is GameAssistManager_List GAM_List)
            {
                EndPoint = GAM_List;

                switch (EndPoint)
                {
                    case GameAssistManager_List.CommandList_Assist:

                        switch ((string)Request_Data.PacketData)
                        {
                            case "Prefab":
                                Return_Packet = abilityUI_Prefab;
                                break;
                            case "Content_Transform":
                                Return_Packet = abilityUI_Content;
                                break;
                            case "Control":
                                Return_Packet = abilityUI_Control;
                                break;
                            case "Button":
                                Return_Packet = abilityUI_ToggleButton;
                                break;
                            default:
                                Debug.LogError($"{(string)Request_Data.PacketData} is not support This Type.");
                                break;

                        }
                        break;
                    case GameAssistManager_List.TimerSystem_Assist:
                        switch ((string)Request_Data.PacketData)
                        {
                            case "Instance":
                                Return_Packet = timerDisplay_Instance;
                                break;
                        }


                        break;
                    case GameAssistManager_List.StateSystem_Assist:
                        switch ((string)Request_Data.PacketData)
                        {
                            case "Instance":
                                Return_Packet = stateDisplay_Instance;
                                break;
                            case "Position_Path":
                                Return_Packet = statePosition_Path;
                                break;
                            case "StateObject_Path":
                                Return_Packet = stateObject_Path;
                                break;
                            case "Prefubs":
                                Return_Packet = stateDisplay_Prefabs;
                                break;
                            case "StateBackGround":
                                Return_Packet = stateBackGround_Path;
                                break;
                            default:
                                Debug.LogError($"{(string)Request_Data.PacketData} is not support This Type.");
                                break;
                        }
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
            }

            Return_GameAssist.QuicklyReturn_False(GameAssistManager_List.None);

            return Return_GameAssist;
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
