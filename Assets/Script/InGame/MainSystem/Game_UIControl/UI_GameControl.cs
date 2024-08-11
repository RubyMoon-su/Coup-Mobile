using UnityEngine;
using Coup_Mobile.EventBus.Packet_Request;

namespace Coup_Mobile.InGame.GameManager.Ui
{
    public abstract class UI_Control
    {
        #region Ref Component
        protected GameUiManager gameUimanager;

        protected bool toggle_Ui;
        protected bool install_Complate = false;

        protected Coroutine method_Status = null;

        #endregion

        public UI_Control(GameUiManager gameUiManager)
        {
            this.gameUimanager = gameUiManager;

            //Install_System();
        }

        protected abstract void Install_System();
        public abstract void OnInteractive_UI();

        public virtual void OnEnabled(bool? Toggle = null)
        {
            if (Toggle.HasValue)
            {
                toggle_Ui = Toggle.Value;
            }
            else
            {
                toggle_Ui = !toggle_Ui;
            }
        }

        protected virtual object CreatePacket_Request(GameManager_Event Event, object EndPoint, object PacketData = null)
        {
            object Reuslt_Converted = EventBus_PacketRequest.CreatePacket_Request_InGameManager(Event, EndPoint, PacketData);

            return Reuslt_Converted;
        }

    }
}