using System.Collections;
using System.Collections.Generic;
using EventBus_System;
using UnityEngine;
using UnityEngine.UI;

namespace Coup_Mobile.Menu
{
    public class Into : MonoBehaviour
    {
        #region Field Common

        private GameObject backGroundScene;

        #endregion

        public void PressButtonStart()
        {
            EventBus_SceneManager<IEvent>.RaiseSceneManager_Event(Changescene.ChangeScene.Mainmenu , false , null);
        }
    }
}
