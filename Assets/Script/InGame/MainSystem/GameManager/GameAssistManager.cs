using UnityEngine;
using Coup_Mobile.EventBus;
using Coup_Mobile.InGame.GameManager.ReportData;

namespace Coup_Mobile.InGame.GameManager
{
    public enum GameAssistManager_List
    {
        None,
        CommandList_Assist,
    }

    public class GameAssistManager
    {
        public GameManager gameManager { get; private set; }

        // Game Command Ui Assist.
        private GameObject abilityUI_Prefab;

        private Transform abilityUI_Content;
        private Transform abilityUI_Platform;
        private Transform abilityUI_Disable;
        private Transform abilityUI_Control;

        private Transform abilityUI_ToggleButton;

        // Game PlayerInfo Ui Assist.


        public GameAssistManager(GameManager gameManager)
        {
            this.gameManager = gameManager;

            LoadAssist();
        }

        private void LoadAssist()
        {
            Load_CommandList_Assist();
        }

        private void Load_CommandList_Assist()
        {
            abilityUI_Prefab = Resources.Load<GameObject>("CommandList_Sort");

            Transform CommandInfo_Path = GameObject.Find("InGane_Canvas/Ui_Interface/CommandInfo").transform;

            abilityUI_Platform = CommandInfo_Path.Find("Command_List");

            abilityUI_Content = CommandInfo_Path.Find($"Scroll View/Viewport/Content");

            abilityUI_Disable = CommandInfo_Path.Find("Scroll View/Viewport/Disable_Sort");

            abilityUI_Control = CommandInfo_Path.Find($"Scroll View");

            abilityUI_ToggleButton = CommandInfo_Path.Find("Command_Button");
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
                            case "Disable_Transform":
                                Return_Packet = abilityUI_Disable;
                                break;
                            case "Button":
                                Return_Packet = abilityUI_ToggleButton;
                                break;
                            default:
                                Debug.LogError($"{(string)Request_Data.PacketData} is not support This Type.");
                                break;

                        }

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
