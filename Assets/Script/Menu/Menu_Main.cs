using UnityEngine;
using System.Collections.Generic;

namespace Coup_Mobile.Menu
{
    public class Menu_Main : MonoBehaviour
    {
        [HideInInspector] private GameObject mainmenu_scene;
        [HideInInspector] private GameObject singleplayer_scene;
        [HideInInspector] private GameObject mulitplayer_scene;
        [HideInInspector] private GameObject option_scene;

        [HideInInspector] private List<GameObject> allmenu;

        #region Starter Function And Ending Function.
        void Awake()
        {
            InstallComponent();

            allmenu = new List<GameObject>{
            mainmenu_scene,
            singleplayer_scene,
            mulitplayer_scene,
            option_scene,
        };


            TurnOffButton();
        }

        private void InstallComponent()
        {
            mainmenu_scene = transform.Find("Menu_Main").gameObject;
            singleplayer_scene = transform.Find("Menu_SinglePlayer").gameObject;
            mulitplayer_scene = transform.Find("Menu_MulitPlayer").gameObject;
            option_scene = transform.Find("Menu_Option").gameObject;
        }

        private void TurnOffButton()
        {
            singleplayer_scene.SetActive(false);
            mulitplayer_scene.SetActive(false);
            option_scene.SetActive(false);
        }

        #endregion

        #region Menu Interactive.
        public void ChangeMenuScene(string sceneName)
        {
            foreach (GameObject Menu in allmenu)
            {
                Menu.SetActive(false);
            }

            string massage = sceneName;

            switch (sceneName)
            {
                case "MainMenu":

                    mainmenu_scene.SetActive(true);

                    break;
                case "SinglePlayer Menu":

                    singleplayer_scene.SetActive(true);

                    break;
                case "MulitPlayer Menu":

                    mulitplayer_scene.SetActive(true);

                    break;
                case "Option Menu":

                    option_scene.SetActive(true);

                    break;
            }

            Debug.Log(massage);

        }

        #endregion
    }
}