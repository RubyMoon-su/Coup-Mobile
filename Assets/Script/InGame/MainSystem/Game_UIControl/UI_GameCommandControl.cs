using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Coup_Mobile.Menu.GameSetting_Data;
using System;

namespace Coup_Mobile.InGame.GameManager.Ui
{

    public class UI_GameCommand_Control : UI_Control
    {
        // Hide GameCommand UI
        protected Button showAndHide_Button;

        // Show GameCommand UI
        protected GameObject ability_InterfaceList;

        //private Scrollbar scrollbar_Horizontal;
        protected GameObject abilitySort_Prefab;
        protected Transform localPath_Content;
        protected Transform localPath_Disable;
        protected AbilitySelection_Control ability_instance;

        protected Action<int , int , Transform[] , Button[]> settingAbilityScroll_Event;


        // Pool Command System.
        protected List<Transform /*Ability Interactive*/> ability_Sort = new List<Transform>();

        public UI_GameCommand_Control(GameUiManager gameUiManager) : base(gameUiManager)
        {
            UnityEngine.Debug.Log("UI_GameCommand Created.");

            Install_System();
        }

        protected override void Install_System()
        {
            try
            {
                // Reference Path With Short.
                GameManager_Event Event = GameManager_Event.GameAssistManager;
                object EndPoint = GameAssistManager_List.CommandList_Assist;
                
                // Get assist and path from GameAssistManager.
                // Load assist in GameAssistManager with EventBus.
                Transform LocalPath = (Transform)CreatePacket_Request(Event, EndPoint, "Content_Transform");
                GameObject Ability_Prefab = (GameObject)CreatePacket_Request(Event, EndPoint, "Prefab");
                Transform Control_Scroll = (Transform)CreatePacket_Request(Event, EndPoint, "Control");

                Button AbilityToggle_Button = ((Transform)CreatePacket_Request(Event, EndPoint, "Button")).GetComponent<Button>();

                // Set assist value to global variable. 
                //scrollbar_Horizontal = Srollbar_instance;
                localPath_Content = LocalPath;
                abilitySort_Prefab = Ability_Prefab;
                ability_instance = Control_Scroll.GetComponent<AbilitySelection_Control>();

                settingAbilityScroll_Event = ability_instance.Setting_AbilityScroll;

                Sort_AbilityDisplay();
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex);
            }
        }

        private void Sort_AbilityDisplay()
        {
            // Reference Path With Short.
            GameManager_Event Event = GameManager_Event.GameResourceManager;
            var EndPoint = ResourceManager_List.GetAbilit_CanUse;

            List<string> AbliltyName = (List<string>)CreatePacket_Request(Event, EndPoint);
            // Create Game Prefab and Setup Button.>

            List<Transform> AllAbility = new List<Transform>();
            List<Button> Interactable_Button = new List<Button>();
            for (int i = 0; i < AbliltyName.Count; i++)
            {
                GameObject CommandList = UnityEngine.Object.Instantiate(abilitySort_Prefab, localPath_Content);

                CommandList.name = AbliltyName[i];

                Image color = CommandList.GetComponent<Image>();
                Text text = CommandList.GetComponentInChildren<Text>();
                Button button = CommandList.GetComponent<Button>();

                text.text = AbliltyName[i];

                button.onClick.AddListener(() => SendAbility_Function(CommandList.name));

                CommandList.transform.SetParent(localPath_Disable);

                ability_Sort.Add(CommandList.transform);
                Interactable_Button.Add(button);
            }

            AbilitySwap_Starter();

            settingAbilityScroll_Event.Invoke(ability_Sort.Count - 1, 0, ability_Sort.ToArray(), Interactable_Button.ToArray());
        }

        public void SendAbility_Function(string AbilityName)
        {
            Debug.LogError(AbilityName);
        }

        private void AbilitySwap_Starter()
        {
            for (int i = 0; i < ability_Sort.Count; i++)
            {
                ability_Sort[i].transform.SetParent(localPath_Content);
            }
        }


        public void OnClick()
        {
            if (this.toggle_Ui)
            {
                showAndHide_Button.gameObject.transform.rotation = new Quaternion(0, 0, 0, 0);

                ability_InterfaceList.SetActive(false);
            }
            else
            {
                showAndHide_Button.gameObject.transform.rotation = new Quaternion(0, 0, 180, 0);

                ability_InterfaceList.SetActive(true);
            }

            this.toggle_Ui = !this.toggle_Ui;
        }

        public override void OnInteractive_UI(string requestment , object packet_Data)
        {
            throw new System.NotImplementedException();
        }
    }
}