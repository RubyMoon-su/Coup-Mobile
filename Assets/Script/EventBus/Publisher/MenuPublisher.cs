
using System;
using Coup_Mobile.Changescene;

namespace Coup_Mobile.EventBus
{
    public interface ISceneManager<T> : IEvent
    {
        Action<ChangeScene, bool, object> OnMainMenu_ArgumentEvent { get; set; }
    }

    public class SceneManager<T> : ISceneManager<T> where T : IEvent
    {
        Action<ChangeScene, bool, object> OnMainMenu_ArgumentEvent = (_, _, _) => { };

        Action<ChangeScene, bool, object> ISceneManager<T>.OnMainMenu_ArgumentEvent
        {
            get => OnMainMenu_ArgumentEvent;
            set => OnMainMenu_ArgumentEvent = value;
        }
        public SceneManager(Action<ChangeScene, bool, object> OnMainMenu_ArgumentEvent) => this.OnMainMenu_ArgumentEvent = OnMainMenu_ArgumentEvent;
    }
}