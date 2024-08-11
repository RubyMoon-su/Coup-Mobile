using System;
using Photon.Pun;
using UnityEngine;
using Coup_Mobile.EventBus;
using Coup_Mobile.InGame.GameManager.ReportData;

namespace Coup_Mobile.InGame.GameManager
{
    public enum GameNetworkManager_List
    {
        CheckAllPlayer_State,
        Player_SendCommand_Update,

        GetInstall_Complate,

    }
    public class GameNetworkManager : IPunObservable
    {
        private GameManager gameManager;
        private PhotonView rpc_Control;
        private bool install_Complate;

        public GameNetworkManager(GameManager gameManager)
        {
            this.gameManager = gameManager;

            Install_System();
        }

        private void Install_System()
        {
            try
            {
                gameManager.gameObject.TryGetComponent<PhotonView>(out rpc_Control);

                if (rpc_Control != null)
                {
                    Debug.Log("GameNetwork Manager Install RPC Complate.");
                    install_Complate = true;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"GameNetworkManager Install_System Error Exception = {ex}");
            }
        }

        public GameNetworkManager_Return GameNetworkManager_Control(GameManager_Data Request_Data)
        {
            GameNetworkManager_Return Return_GameNetwork = new GameNetworkManager_Return();
            object Return_Packet = null;
            try
            {
                GameNetworkManager_List EndPoint = default;

                if (Request_Data.EndPoint is GameNetworkManager_List GNM_List)
                {
                    EndPoint = GNM_List;

                    switch (EndPoint)
                    {
                        case GameNetworkManager_List.CheckAllPlayer_State:

                            rpc_Control.RPC("CheckAllPlayer_State", RpcTarget.All);

                            break;
                        case GameNetworkManager_List.Player_SendCommand_Update:

                            rpc_Control.RPC("UpdateRequest_Command", RpcTarget.All);

                            break;
                        case GameNetworkManager_List.GetInstall_Complate:

                            Return_Packet = install_Complate;

                            break;

                    }

                    if (Return_Packet != null)
                    {
                        Return_GameNetwork = new GameNetworkManager_Return
                        {
                            return_Data = Return_Packet,
                            requestType = EndPoint,
                            requestCommand_Reult = true,
                        };

                        return Return_GameNetwork;

                    }
                }

                Return_GameNetwork.QuicklyReturn_False(EndPoint);

                return Return_GameNetwork;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                return Return_GameNetwork;
            }
        }

        #region Local Network Function

        [PunRPC]
        public bool CheckAllPlayer_State()
        {
            return true;
        }

        [PunRPC]
        private bool UpdateRequest_Command()
        {
            return true;
        }


        #endregion


        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {

            }
            else if (stream.IsReading)
            {

            }
        }

    }

    public struct GameNetworkManager_Return : IInGameEvent
    {
        public bool requestCommand_Reult;
        public GameNetworkManager_List requestType;
        public object return_Data;

        public void QuicklyReturn_False(GameNetworkManager_List Type)
        {
            requestCommand_Reult = false;
            requestType = Type;
            return_Data = null;
        }
    }
}
