using System;
using UnityEngine;
using Coup_Mobile.EventBus;
using Coup_Mobile.InGame.GameManager.ReportData;

namespace Coup_Mobile.InGame.GameManager.GameState
{
    public class GameState
    {
        private GameStateManager gameStateManager;

        public GameState(GameStateManager gameStateManager)
        {
            this.gameStateManager = gameStateManager;
        }

        public virtual object Request_Event(GameManager_Event @event, object endPoint, object packetData)
        {
            GameManager_Data Request_EventBus = new GameManager_Data
            {
                gameManager_Event = @event,
                EndPoint = endPoint,
                PacketData = packetData
            };

            var Event_Result = EventBus_InGameManager<IInGameEvent>.RaiseGameCommand(Request_EventBus);

            return Event_Result;
        }

        public virtual string NetworkRequestment_ConvertToJson<T>(T network_Data)
        {
            return JsonUtility.ToJson(network_Data);
        }
    }

}