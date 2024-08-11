using Photon.Pun;
using Coup_Mobile.EventBus;

namespace Coup_Mobile.Changescene
{
    public enum ChangeScene
    {
        Mainmenu,
        CreateRoom,
        JoinRoom,
        FindRoom,
        MatchMaking,
        InGame,
        Lobby,
        EndGame,
    }

    public class SceneControl : MonoBehaviourPun
    {
        private static SceneControl instance;

        public void Awake()
        {
            if (instance != this && instance != null)
            {
                Destroy(gameObject);
            }
            else 
            {
                instance = this;
                
                DontDestroyOnLoad(this);
            }
        }

        public void Start()
        {
            InstallEvent();
        }

        private void InstallEvent()
        {
            SceneManager<IEvent> @event = new SceneManager<IEvent>(SceneManager_Control);
            EventBus_SceneManager<IEvent>.SceneManager_Control(@event);
        }

        public void SceneManager_Control(ChangeScene Scene , bool ChangeByNetwork , object PacketData)
        {
            string ChangeSceneName = Scene.ToString();

            if (ChangeByNetwork)
            {
                PhotonNetwork.LoadLevel(ChangeSceneName);
            }
            else 
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(ChangeSceneName);
            }
        }
    }
}