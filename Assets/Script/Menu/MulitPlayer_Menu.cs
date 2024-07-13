using UnityEngine;
using EventBus_System;
using Coup_Mobile.Changescene;

namespace Coup_Mobile.Menu
{
    public class MulitPlayer_Menu : MonoBehaviour
    {
        [HideInInspector] private Menu_Main mainmenu;

        #region  Start Component
        void Awake()
        {
            InstallComponent();
        }

        private void InstallComponent()
        {
            mainmenu = GetComponentInParent<Menu_Main>();
        }

        #endregion

        #region Button Event

        public void CreateRoom_Function() => CallChangeScene_Event(ChangeScene.CreateRoom);


        public void JoinRoom_Function() => CallChangeScene_Event(ChangeScene.FindRoom);


        public void MatchMaking_Function() => CallChangeScene_Event(ChangeScene.MatchMaking);


        public void BackToMainMenu_Function()
        {
            mainmenu.ChangeMenuScene("MainMenu");
        }

        #endregion

        #region Call EventBus

        private void CallChangeScene_Event(ChangeScene Cs) => EventBus_SceneManager<IEvent>.RaiseSceneManager_Event(Cs, false, null);


        #endregion
    }
}