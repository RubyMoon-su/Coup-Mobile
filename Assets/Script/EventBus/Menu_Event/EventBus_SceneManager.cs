using System.Collections.Generic;
using Coup_Mobile.Changescene;

namespace EventBus_System
{
    public static class EventBus_SceneManager<T> where T : IEvent
    {
        #region  Collection Field

        private static readonly HashSet<ISceneManager<T>> SceneManagers_Event = new HashSet<ISceneManager<T>>();

        #endregion

        #region Register Or Deregister Event.

        /// <summary>
        /// This Method is process Between Register and Deregister Method Event.
        /// </summary>
        /// <param name="event"> Method Type "Normal Method" No arugment parameter.</param>
        public static void SceneManager_Control(ISceneManager<T> @event)
        {
            if (SceneManagers_Event.Contains(@event))
            {
                SceneManagers_Event.Remove(@event);
            }
            else 
            {
                SceneManagers_Event.Add(@event);
            }
        }

        #endregion
    
        #region Raise Event.

        public static void RaiseSceneManager_Event(ChangeScene Cs , bool ChangeByNetwork , object PacketData)
        {
            foreach (var Event in SceneManagers_Event)
            {
                Event.OnMainMenu_ArgumentEvent.Invoke(Cs , ChangeByNetwork , PacketData);   
            }
        }  

        #endregion 
    }

}