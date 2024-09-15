using System;
using Photon.Pun;
using UnityEngine;
using Coup_Mobile.EventBus;
using Coup_Mobile.InGame.GameManager.ReportData;
using Photon.Realtime;
using System.Threading.Tasks;

namespace Coup_Mobile.InGame.GameManager
{
    public enum GameNetworkManager_List
    {
        CheckAllPlayer_State,
        Player_SendCommand_Update,
        ShareResourceSetting,

        GetInstall_Complate,

    }
    public class GameNetworkManager : MonoBehaviourPun, IPunObservable
    {
        #region Universal Variable Global Field

        private GameManager gameManager;
        private bool install_Complate;

        #endregion

        #region Online Variable Global Field

        private PhotonView rpc_Control;

        #endregion

        #region Offline Variabel Global Field



        #endregion

        public void Install_System()
        {
            try
            {
                gameObject.TryGetComponent<PhotonView>(out rpc_Control);

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
            bool isOnline = GameManager.isOnline;
            GameNetworkManager_Return Return_Packet = new GameNetworkManager_Return();

            if (isOnline)
            {
                Return_Packet = ProcessWithOnlineMode(Request_Data);
            }
            else
            {
                Return_Packet = ProcessWithOfflineMode(Request_Data);
            }

            return Return_Packet;

        }

        #region Game Network Optional

        private GameNetworkManager_Return ProcessWithOnlineMode(GameManager_Data Request_Data)
        {
            GameNetworkManager_Return Return_GameNetwork = new GameNetworkManager_Return();
            object Return_Packet = null;

            try
            {
                GameNetworkManager_List EndPoint = default;

                if (Request_Data.EndPoint is GameNetworkManager_List GNM_List)
                {
                    EndPoint = GNM_List;
                    var NetworkPacket = Request_Data.PacketData as GameNetwork_Requestment?;

                    if (!NetworkPacket.HasValue)
                    {
                        Debug.LogError("Please Request Game Network With 'Game Network Requestment' Data Type.");
                        Return_GameNetwork.QuicklyReturn_False(EndPoint);

                        return Return_GameNetwork;
                    }

                    // Send Network Event Option.
                    Player SelectionPlayer = NetworkPacket.Value.playerSelection;
                    RpcTarget TargetOption = NetworkPacket.Value.target_Option;
                    string packetData_Json = NetworkPacket.Value.packetData;

                    // Send event with selection one player.
                    if (SelectionPlayer != null)
                    {
                        Return_Packet = Requestment_With_TargetSelection(GNM_List, SelectionPlayer, packetData_Json);
                    }
                    // Send event With rpc option.
                    else
                    {
                        Return_Packet = Requestment_With_RPCOption(GNM_List, TargetOption, packetData_Json);
                    }

                    // if requestment have data return , system will return anything data back to owner requestment.
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
                    else
                    {
                        Return_GameNetwork = new GameNetworkManager_Return
                        {
                            return_Data = null,
                            requestType = EndPoint,
                            requestCommand_Reult = true,
                        };

                        return Return_GameNetwork;
                    }
                }

                // This step in the system does not return anything to the owner’s request, but the system will return a 'process successfully completed' status.
                Return_GameNetwork.QuicklyReturn_False(EndPoint);

                return Return_GameNetwork;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                return Return_GameNetwork;
            }
        }

        private GameNetworkManager_Return ProcessWithOfflineMode(GameManager_Data Request_Data)
        {
            // this Function doesn't have a processing.

            return new GameNetworkManager_Return();
        }

        #endregion

        private object Requestment_With_RPCOption(GameNetworkManager_List endPoint, RpcTarget rpc_Option, string packetData_Json)
        {
            switch (endPoint)
            {
                case GameNetworkManager_List.CheckAllPlayer_State:

                    rpc_Control.RPC("CheckAllPlayer_State", rpc_Option, packetData_Json);

                    break;
                case GameNetworkManager_List.Player_SendCommand_Update:

                    rpc_Control.RPC("UpdateRequest_Command", rpc_Option);

                    break;
                case GameNetworkManager_List.ShareResourceSetting:

                    rpc_Control.RPC("ShareResourceSetting", rpc_Option, packetData_Json);

                    break;
                case GameNetworkManager_List.GetInstall_Complate:

                    return install_Complate;
            }

            return null;
        }

        private object Requestment_With_TargetSelection(GameNetworkManager_List endPoint, Player selectionPlayer, string packetData_Json)
        {
            switch (endPoint)
            {
                case GameNetworkManager_List.CheckAllPlayer_State:

                    rpc_Control.RPC("CheckAllPlayer_State", selectionPlayer, packetData_Json);

                    break;
                case GameNetworkManager_List.Player_SendCommand_Update:

                    rpc_Control.RPC("UpdateRequest_Command", selectionPlayer);

                    break;
                case GameNetworkManager_List.ShareResourceSetting:

                    rpc_Control.RPC("ShareResourceSetting", selectionPlayer, packetData_Json);

                    break;
                case GameNetworkManager_List.GetInstall_Complate:

                    return install_Complate;
            }

            return null;
        }

        #region Local Network Function

        [PunRPC]
        public void CheckAllPlayer_State()
        {

        }

        [PunRPC]
        private void UpdateRequest_Command()
        {

        }

        [PunRPC]
        public void ShareResourceSetting(string StarterSetting_Json)
        {
            GameManager_Data ShareResourcet_Event = new GameManager_Data
            {
                gameManager_Event = GameManager_Event.GameStateManager,
                EndPoint = GameStateManager_List.UpdateStateNetwork,
                PacketData = StarterSetting_Json,
            };

            EventBus_InGameManager<IInGameEvent>.RaiseGameCommand(ShareResourcet_Event);
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

    [Serializable]
    public struct GameNetwork_Requestment
    {
        public Player playerSelection;
        public RpcTarget target_Option;

        public string packetData;

        public void SettingDataToDefault()
        {
            playerSelection = null;
            target_Option = RpcTarget.All;
            packetData = null;
        }
    }
}
