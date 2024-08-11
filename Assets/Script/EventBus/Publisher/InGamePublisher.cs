using System;

namespace Coup_Mobile.EventBus
{
    public interface IInGameManager<T> : IInGameEvent
    {
        Func<T , T> OnPlayerCommand {get; set;}
    }
    public class InGameManager<T> : IInGameManager<T> where T : IInGameEvent
    {
        Func<T , T> OnPlayerCommand = (_) => (_);

        Func<T , T> IInGameManager<T>.OnPlayerCommand 
        {
            get => OnPlayerCommand;
            set => OnPlayerCommand = value;
        }

        public InGameManager(Func<T , T> OnPlayerCommand) => this.OnPlayerCommand = OnPlayerCommand; 
    }
}
