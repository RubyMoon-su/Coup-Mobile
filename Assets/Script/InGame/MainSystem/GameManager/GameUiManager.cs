using System;
using UnityEngine;
using Coup_Mobile.EventBus;
using Coup_Mobile.InGame.UI;
using System.Collections.Generic;
using Coup_Mobile.InGame.GameManager.Ui;
using Coup_Mobile.InGame.GameManager.ReportData;

namespace Coup_Mobile.InGame.GameManager
{
    public enum GameUIManager_List
    {
        None,
        Game_Command_UI,
        Game_MiniMap_UI,
        Game_PlayerInfo_UI,
        Game_WaveInfo_UI,
        Game_Chat_UI,
        Game_WaveControl_UI,
        Game_TimeControl_Ui,

        GetInstall_Complate,
    }

    public class GameUiManager
    {
        public GameManager gameManager { get; private set; }
        private Dictionary<GameUIManager_List, IGameUi_Controller> ui_Control;

        public bool install_Complate = false;

        public GameUiManager(GameManager gameManager)
        {
            this.gameManager = gameManager;

            Install_ButtonEvent();
        }

        private void Install_ButtonEvent()
        {
            try
            {
                // Get Component MainInterface Button.
                IGameUi_Controller ui_GameCommand_control = new UI_GameCommand_Control(this);
                IGameUi_Controller ui_WaveGame_Control = new UI_WaveGame_Control(this);
                IGameUi_Controller ui_GameTimer_Control = new UI_GameTimer_Control(this);
                IGameUi_Controller ui_minimap_control = new UI_MiniMap_Control(this);
                IGameUi_Controller ui_gamechat_Control = new UI_GameChat_Control(this);
                IGameUi_Controller ui_playerinfo_Control = new UI_PlayerInfo_Control(this);

                ui_Control = new Dictionary<GameUIManager_List, IGameUi_Controller>
                {
                    {GameUIManager_List.Game_Command_UI, ui_GameCommand_control},
                    {GameUIManager_List.Game_WaveControl_UI , ui_WaveGame_Control},
                    {GameUIManager_List.Game_TimeControl_Ui , ui_GameTimer_Control},
                    {GameUIManager_List.Game_MiniMap_UI , ui_minimap_control},
                    {GameUIManager_List.Game_Chat_UI , ui_gamechat_Control},
                    {GameUIManager_List.Game_PlayerInfo_UI , ui_playerinfo_Control},
                };
                install_Complate = true;

            }
            catch (Exception ex)
            {
                Debug.LogError($"GameUi Manager Exception 'Install_ButtonEvent' {ex}");

                install_Complate = false;
            }


        }

        public GameUIManager_Return GameUIManager_Control(GameManager_Data Request_Data)
        {
            GameUIManager_Return Return_GameUI = new GameUIManager_Return();
            object Packet_Data = null;

            GameUIManager_List? EndPoint = Request_Data.EndPoint as GameUIManager_List?;

            if (EndPoint.HasValue)
            {
                GameUI_RequestData? RequestUI_Header = Request_Data.PacketData as GameUI_RequestData?;

                if (RequestUI_Header.HasValue)
                {
                    var GameUi_Path = EndPoint.Value;
                    IGameUi_Controller Ui_Controller = null;

                    foreach (var Ui_Select in ui_Control)
                    {
                        if (Ui_Select.Key == GameUi_Path)
                        {
                            Ui_Controller = Ui_Select.Value;
                        }
                    }

                    if (Ui_Controller == null)
                    {
                        Return_GameUI.QuicklyReturn_False(EndPoint.Value, "GameUi Select is null");

                        return Return_GameUI;
                    }

                    GameUI_ReturnData ReturnPacket_Ui = default;
                    string Topic = RequestUI_Header.Value.request_Topic[0];
                    var UIPacket = RequestUI_Header.Value;

                    switch (Topic)
                    {
                        case "Request":
                            ReturnPacket_Ui = Ui_Controller.OnRequest_UI(UIPacket);
                            break;
                        case "GetData":
                            ReturnPacket_Ui = Ui_Controller.OnReturnStatus_UI(UIPacket);
                            break;
                        case "Update":
                            ReturnPacket_Ui = Ui_Controller.OnUpdateData_UI(UIPacket);
                            break;
                        case "ToggleActive":
                            ReturnPacket_Ui = Ui_Controller.OnToggleActive_UI(UIPacket);
                            break;
                    }

                    if (ReturnPacket_Ui.Equals(typeof(GameUI_ReturnData)))
                    {
                        Return_GameUI.QuicklyReturn_False(EndPoint.Value, "No ReturnData from the Ui Controller.");

                        return Return_GameUI;
                    }
                }
                else
                {
                    var GameUi_Path = EndPoint.Value;

                    switch (GameUi_Path)
                    {
                        case GameUIManager_List.GetInstall_Complate:
                            Packet_Data = install_Complate;
                            break;
                        default:
                            Return_GameUI.QuicklyReturn_False(EndPoint.Value, $"{GameUi_Path} is not installed in a System.");
                            return Return_GameUI;
                    }

                    if (Packet_Data != null)
                    {
                        Return_GameUI = new GameUIManager_Return
                        {
                            requestCommand_Reult = true,
                            requestType = GameUi_Path,
                            return_Data = Packet_Data,
                        };
                        return Return_GameUI;
                    }
                }
            }

            Return_GameUI.QuicklyReturn_False(GameUIManager_List.None , "Requestment has not set the Game Ui path.");

            return Return_GameUI;
        }
    }

    public struct GameUIManager_Return : IInGameEvent
    {
        public bool requestCommand_Reult;
        public GameUIManager_List requestType;
        public object return_Data;
        public string system_message;

        public void QuicklyReturn_False(GameUIManager_List Type, string system_message = null)
        {
            requestCommand_Reult = false;
            requestType = Type;
            return_Data = null;
            this.system_message = system_message;
        }
    }
}