using System.Collections;
using System.Collections.Generic;

namespace Coup_Mobile.Menu
{
    public enum Observer_Lobby_Type
    {
        Observer_Ui, Observer_Slot
    }

    public interface ILobby_Subject
    {
        void Attach_LobbyObserver(ILobby_Observer observer, Observer_Lobby_Type type);
        void Detach_LobbyObserver(ILobby_Observer observer, Observer_Lobby_Type type);
        void Update_LobbyObserver(object PacketData, Observer_Lobby_Type type);
    }

    public interface ILobby_Observer
    {
        void Update_Notification(object PacketData);
    }


    public class Lobby_Observer : ILobby_Subject
    {
        private readonly List<ILobby_Observer> observer_ui = new List<ILobby_Observer>();
        private readonly List<ILobby_Observer> observer_slot = new List<ILobby_Observer>();

        public Lobby_Observer()
        {

        }

        public void Attach_LobbyObserver(ILobby_Observer observer, Observer_Lobby_Type type)
        {
            switch (type)
            {
                case Observer_Lobby_Type.Observer_Ui:
                    observer_ui.Add(observer);
                    break;
                case Observer_Lobby_Type.Observer_Slot:
                    observer_slot.Add(observer);
                    break;
            }
        }

        public void Detach_LobbyObserver(ILobby_Observer observer, Observer_Lobby_Type type)
        {
            switch (type)
            {
                case Observer_Lobby_Type.Observer_Ui:
                    observer_ui.Remove(observer);
                    break;
                case Observer_Lobby_Type.Observer_Slot:
                    observer_slot.Remove(observer);
                    break;
            }
        }

        public void Update_LobbyObserver(object PacketData, Observer_Lobby_Type type)
        {
            List<ILobby_Observer> Target_Observer = new List<ILobby_Observer>();

            switch (type)
            {
                case Observer_Lobby_Type.Observer_Ui:
                    Target_Observer = observer_ui;
                    break;
                case Observer_Lobby_Type.Observer_Slot:
                    Target_Observer = observer_slot;
                    break;
            }

            if (Target_Observer.Count > 0)
            {
                foreach(var Observer in Target_Observer)
                {
                    Observer.Update_Notification(PacketData);
                }
            }
        }
    }
}