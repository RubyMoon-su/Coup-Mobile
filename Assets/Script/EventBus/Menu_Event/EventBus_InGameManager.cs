using UnityEngine;
using System.Collections.Generic;
using Coup_Mobile.InGame.GameManager.ReportData;


namespace Coup_Mobile.EventBus
{
    public static class EventBus_InGameManager<T> where T : IInGameEvent
    {
        #region Event Command

        public static readonly HashSet<IInGameManager<T>> PlayerRequestCommand = new HashSet<IInGameManager<T>>();

        public static readonly HashSet<IInGameManager<T>> GameManagerCommand = new HashSet<IInGameManager<T>>();

        #endregion

        public static void EventCommand_Control(IInGameManager<T> @event, string EndPoint)
        {
            switch (EndPoint)
            {
                case "GameManager":

                    if (GameManagerCommand.Contains(@event))
                    {
                        GameManagerCommand.Remove(@event);
                    }
                    else
                    {
                        GameManagerCommand.Add(@event);
                    }

                    break;
                case "PlayerCommand":

                    if (PlayerRequestCommand.Contains(@event))
                    {
                        PlayerRequestCommand.Remove(@event);
                    }
                    else
                    {
                        PlayerRequestCommand.Add(@event);
                    }

                    break;
                default : UnityEngine.Debug.LogError($"No EndPoint Name = {EndPoint} In EventBus System");
                    break;
            }

        }

        public static object RaiseGameCommand(object PacketData)
        {
            if (GameManagerCommand.Count <= 0)
            {
                Debug.LogError("No GameManager Command Register 'EventBus InGame'");
                return false;
            }

            HashSet<IInGameManager<T>> EventSelection = new HashSet<IInGameManager<T>>();

            if (PacketData is GameManager_Data)
            {
                EventSelection = GameManagerCommand;
            }
            else
            {
                Debug.LogError($"DataType = {PacketData.GetType()} is not Corrent.");
                return null;
            }

            if (EventSelection.Count <= 0)
            {
                Debug.LogError("No Event Register Raise.");
                return false;
            }

            foreach (var @event in EventSelection)
            {
                return @event.OnPlayerCommand.Invoke((T)PacketData);
            }

            return false;
        }
    }
}
