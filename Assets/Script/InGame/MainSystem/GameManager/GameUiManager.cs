using System;
using UnityEngine;
using Coup_Mobile.EventBus;
using System.Collections.Generic;
using Coup_Mobile.InGame.GameManager.ReportData;
using Coup_Mobile.InGame.GameManager.Ui;

namespace Coup_Mobile.InGame.GameManager
{
    public enum GameUIManager_List
    {
        Game_Command_UI,
        Game_MiniMap_UI,
        Game_PlayerInfo_UI,
        Game_WaveInfo_UI,
        Game_Chat_UI,
        Game_WaveControl_UI,

        GetInstall_Complate,
    }

    public class GameUiManager
    {
        public GameManager gameManager { get; private set; }
        private List<UI_Control> ui_Control = new List<UI_Control>();

        public bool install_Complate { get; private set; } = false;

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
                UI_GameCommand_Control ui_gamecommand_control = new UI_GameCommand_Control(this);
                //UI_MiniMap_Control ui_minimap_control = new UI_MiniMap_Control(this);
                //UI_GameChat_Control ui_gamechat_Control = new UI_GameChat_Control(this);
                //UI_PlayerInfo_Control ui_playerinfo_Control = new UI_PlayerInfo_Control(this);

                ui_Control = new List<UI_Control>
                {
                    ui_gamecommand_control,
                    //ui_minimap_control,
                    //ui_gamechat_Control,
                    //ui_playerinfo_Control,
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

            GameUIManager_List EndPoint = default;

            if (Request_Data.EndPoint is GameUIManager_List GUM_List)
            {
                EndPoint = GUM_List;

                switch (EndPoint)
                {
                    case GameUIManager_List.Game_Command_UI:
                        break;
                    case GameUIManager_List.Game_MiniMap_UI:
                        break;
                    case GameUIManager_List.Game_Chat_UI:
                        break;
                    case GameUIManager_List.Game_WaveControl_UI:
                        break;
                    case GameUIManager_List.Game_WaveInfo_UI:
                        break;
                    case GameUIManager_List.Game_PlayerInfo_UI:
                        break;
                    case GameUIManager_List.GetInstall_Complate:
                        Packet_Data = install_Complate;
                        break;
                }

                if (Packet_Data != null)
                {
                    Return_GameUI = new GameUIManager_Return
                    {
                        requestCommand_Reult = true,
                        requestType = EndPoint,
                        return_Data = Return_GameUI,
                    };
                }
            }

            Return_GameUI.QuicklyReturn_False(EndPoint);

            return Return_GameUI;
        }
    }

    public struct GameUIManager_Return : IInGameEvent
    {
        public bool requestCommand_Reult;
        public GameUIManager_List requestType;
        public object return_Data;

        public void QuicklyReturn_False(GameUIManager_List Type)
        {
            requestCommand_Reult = false;
            requestType = Type;
            return_Data = null;
        }
    }
}