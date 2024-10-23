using System;
using UnityEngine;
using Coup_Mobile.EventBus.Packet_Request;

namespace Coup_Mobile.InGame.GameManager.Ui
{
    public interface IGameUi_Controller
    {
        public abstract GameUI_ReturnData OnRequest_UI(GameUI_RequestData requestData);
        public abstract GameUI_ReturnData OnReturnStatus_UI(GameUI_RequestData getData);
        public abstract GameUI_ReturnData OnUpdateData_UI(GameUI_RequestData updateData);
        public abstract GameUI_ReturnData OnToggleActive_UI(GameUI_RequestData toggleActive);
    }

    public abstract class GameUI_Herder : IGameUi_Controller
    {
        #region Ref Component
        protected GameUiManager gameUimanager;

        protected bool toggle_Ui;
        protected bool install_Complate = false;


        #endregion

        public GameUI_Herder(GameUiManager gameUiManager)
        {
            this.gameUimanager = gameUiManager;

            //Install_System();
        }

        protected virtual T CallRequest<T>(RequestParams requestParams, object PacketData = null)
        {
            object Reuslt_Converted = EventBus_PacketRequest.CreatePacket_Request_InGameManager(requestParams.EventPath, requestParams.EndPoint, PacketData);

            return (T)Reuslt_Converted;
            /*
                        switch (Reuslt_Converted)
                        {
                            case GameAssistManager_Return GameAssistPacket: return (T)GameAssistPacket.return_Data;
                            case GameNetworkManager_Return GameNetworkPacket: return (T)GameNetworkPacket.return_Data;
                            case GameStateManager_Return GameStatePacket: return (T)GameStatePacket.return_Data;
                            case GameResource_Result GameResourcePacket: return (T)GameResourcePacket.return_Data;
                            case GameUIManager_Return GameUiPacket: return (T)GameUiPacket.return_Data;
                            case PlayerManager_Return PlayerPacket: return (T)PlayerPacket.return_Data;
                            default: throw new Exception($"{Reuslt_Converted.GetType()} No Return Type Install in CreatePacket_Request Method");
                        }
                        */
        }

        /// <summary>
        /// Starter of UI function.
        /// </summary>
        protected abstract void InstallSystem();

        #region  Interface Function Control UI

        /// <summary>
        /// Send a data packet request to the UI controller.
        /// </summary>
        /// <returns></returns>
        public abstract GameUI_ReturnData OnRequest_UI(GameUI_RequestData requestData);

        /// <summary>
        /// Request Data for the UI controller.
        /// </summary>
        /// <returns></returns>
        public abstract GameUI_ReturnData OnReturnStatus_UI(GameUI_RequestData getData);

        /// <summary>
        /// Send a data packet to update the UI controller.
        /// </summary>
        /// <returns></returns>
        public abstract GameUI_ReturnData OnUpdateData_UI(GameUI_RequestData updateData);

        /// <summary>
        /// Toggle the UI's active controller. 
        /// </summary>
        /// <returns></returns>
        public abstract GameUI_ReturnData OnToggleActive_UI(GameUI_RequestData toggleActive);

        #endregion

        #region Local Function

        /// <summary>
        /// This Method is the Shortcut of the Create a Return Data Type.
        /// </summary>
        /// <param name="returnData">Any data it you want to return the data.</param>
        /// <param name="OnException">Variable data is Checked from systems are correct or incorrect process system.</param>
        /// <param name="ExceptionMessage">Message from the process system Error.</param>
        /// <returns>Result from the process System.</returns>
        protected virtual GameUI_ReturnData Create_ReturnData(object returnData, bool OnException, string ExceptionMessage)
        {
            GameUI_ReturnData ReturnData = new GameUI_ReturnData
            {
                // Any data it you want to return the data.
                return_Data = returnData != null ? returnData : null,


                exception_Info = ExceptionMessage != string.Empty ? ExceptionMessage : null, // Error Message.
                exception_status = OnException, // Error Status.
            };

            return ReturnData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="PacketData"></param>
        /// <returns></returns>
        protected virtual bool Check_PacketDataType<T>(object PacketData)
        {
            return PacketData.GetType() == typeof(T);
        }

        /// <summary>
        /// 
        /// </summary>
        protected Func<GameUI_Herder, string, string, Exception> CreateException = (ClassName, Text, MethodName) =>
        {
            return new Exception($"{ClassName.GetType().Name} -> {MethodName} | {Text}");
        };

        protected virtual void ValidateRequestData(GameUI_RequestData requestData , string functionName)
        {
            if (!Check_PacketDataType<string>(requestData.request_Topic[0]))
                throw CreateException.Invoke(this, "MainTopic is null.", functionName);

            if (!Check_PacketDataType<string>(requestData.request_Topic[1]))
                throw CreateException.Invoke(this, "Process Target is null.", functionName);
        }

        protected GameUI_ReturnData HandleException(Exception ex)
        {
            string errorMessage = string.IsNullOrEmpty(ex.Message) ? "None" : ex.Message;
            return Create_ReturnData(false, true, errorMessage);
        }

        #endregion

    }
    public class RequestParams
    {
        public GameManager_Event EventPath { get; set; }
        public object EndPoint { get; set; }

        public RequestParams(GameManager_Event eventPath, object endPoint)
        {
            EventPath = eventPath;
            EndPoint = endPoint;
        }
    }

    /// <summary>
    /// This packet is used to send a request to the UI controller.
    /// </summary>
    [Serializable]
    public struct GameUI_RequestData
    {
        public string[] request_Topic;
        public object packetData;
    }

    /// <summary>
    /// This packet is used to send a request for return data from the UI controller.
    /// </summary>
    [Serializable]
    public struct GameUI_ReturnData
    {
        public object return_Data;

        public bool exception_status;
        public string exception_Info;
    }
}