using System;
using UnityEngine;
using Coup_Mobile.InGame.GameManager;
using Coup_Mobile.InGame.GameManager.Ui;

namespace Coup_Mobile.InGame.UI
{
    public class UI_GameTimer_Control : UI_Control
    {
        protected TimeState_Control timeStateControl;

        protected Action<int> requestTimer;
        protected Action resetTimer;

        public UI_GameTimer_Control(GameUiManager gameUiManager) : base(gameUiManager)
        {
            Install_System();
        }

        protected override void Install_System()
        {
            try
            {
                GameManager_Event Event = GameManager_Event.GameAssistManager;
                GameAssistManager_List EndPoint = GameAssistManager_List.TimerSystem_Assist;

                Transform Instance = (Transform)CreatePacket_Request(Event, EndPoint, "Instance");

                timeStateControl = Instance.GetComponent<TimeState_Control>();

                requestTimer = timeStateControl.RequestTimer;
                resetTimer = timeStateControl.Reset_Time;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }
        }
        public override void OnInteractive_UI(string requestment , object packet_Data)
        {
            
        }

    }
}