using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Coup_Mobile.InGame.GameManager.Ui
{

    public class UI_GameCommand_Control : UI_Control
    {
        // Hide GameCommand UI
        private Button toggle_Button;

        // Show GameCommand UI
        private GameObject ability_InterfaceList;
        //private Scrollbar scrollbar_Horizontal;
        private GameObject abilitySort_Prefab;
        private Transform localPath_Content;
        private Transform localPath_Disable;
        private Button platefort_Ability;
        private AbilitySelection_Control control_Scroll;

        // Display Ability List.
        private Transform selection_Ability;
        private Transform next_Ability;
        private Transform before_Ability;

        private int half_AbilityAmount;

        // Pool Command System.
        private List<Transform /*Ability Interactive*/> ability_Sort = new List<Transform>();

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
                Transform Disable_Path = (Transform)CreatePacket_Request(Event, EndPoint, "Disable_Transform");
                Transform Control_Scroll = (Transform)CreatePacket_Request(Event, EndPoint, "Control");

                Button AbilityToggle_Button = ((Transform)CreatePacket_Request(Event, EndPoint, "Button")).GetComponent<Button>();
                //Scrollbar Srollbar_instance = PathCreate.GetComponent<Scrollbar>();

                // Set assist value to global variable. 
                //scrollbar_Horizontal = Srollbar_instance;
                localPath_Content = LocalPath;
                abilitySort_Prefab = Ability_Prefab;
                localPath_Disable = Disable_Path;
                control_Scroll = Control_Scroll.GetComponent<AbilitySelection_Control>();
                //scrollbar_Horizontal.onValueChanged.AddListener(ChangeScrollList);

                // Reference Path With Short.
                Event = GameManager_Event.GameResourceManager;
                EndPoint = ResourceManager_List.GetAbilit_CanUse;

                List<string> AbliltyName = (List<string>)CreatePacket_Request(Event, EndPoint);
                // Create Game Prefab and Setup Button.>

                List<Transform> AllAbility = new List<Transform>();
                for (int i = 0; i < AbliltyName.Count; i++)
                {
                    GameObject CommandList = Object.Instantiate(abilitySort_Prefab, localPath_Content);

                    CommandList.name = AbliltyName[i];

                    Image color = CommandList.GetComponent<Image>();
                    Text text = CommandList.GetComponentInChildren<Text>();
                    Button button = CommandList.GetComponent<Button>();

                    text.text = AbliltyName[i];

                    button.onClick.AddListener(() => CommandTest(button.gameObject.name));

                    CommandList.transform.SetParent(Disable_Path);

                    ability_Sort.Add(CommandList.transform);
                }

                half_AbilityAmount = (int)Mathf.Round(ability_Sort.Count / 2);

                selection_Ability = ability_Sort[0];
                next_Ability = ability_Sort[1];
                before_Ability = ability_Sort[ability_Sort.Count - 1];

                AbilitySwap_Starter();

                control_Scroll.Setting_AbilityScroll(ability_Sort.Count - 1, 0, ability_Sort.ToArray());

            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex);
            }
        }

        public void CommandTest(string A)
        {
            Debug.LogError(A);
        }

        private void OnToggle_AbilityPanel()
        {

        }

        private void AbilitySwap_Starter()
        {
            for (int i = 0; i < ability_Sort.Count; i++)
            {
                ability_Sort[i].transform.SetParent(localPath_Content);
            }



            //scrollbar_Horizontal.value = 0.5f;
        }

        private void AbilitySelection_Swap(int Scroll_Direction)
        {
            int? CurrentAbility_Index = null;

            for (int i = 0; i < ability_Sort.Count; i++)
            {
                if (selection_Ability.gameObject.name == ability_Sort[i].name)
                {
                    CurrentAbility_Index = i;
                    break;
                }
            }

            if (!CurrentAbility_Index.HasValue)
            {
                Debug.LogError("CurrentAbility Is Null");
            }

            int NewIndexResult = CurrentAbility_Index.Value;

            switch (Scroll_Direction)
            {
                case 1:

                    NewIndexResult++;

                    if (NewIndexResult > ability_Sort.Count - 1) NewIndexResult = 0;

                    selection_Ability = ability_Sort[NewIndexResult];

                    break;
                case -1:

                    NewIndexResult--;

                    if (NewIndexResult < 0) NewIndexResult = ability_Sort.Count - 1;

                    selection_Ability = ability_Sort[NewIndexResult];

                    break;
            }

            for (int i = 0; i < ability_Sort.Count; i++)
            {
                if (i > half_AbilityAmount)
                {

                }


            }
        }

        public override void OnInteractive_UI()
        {
            throw new System.NotImplementedException();
        }

        public void OnClick()
        {
            if (this.toggle_Ui)
            {
                toggle_Button.gameObject.transform.rotation = new Quaternion(0, 0, 0, 0);

                ability_InterfaceList.SetActive(false);
            }
            else
            {
                toggle_Button.gameObject.transform.rotation = new Quaternion(0, 0, 180, 0);

                ability_InterfaceList.SetActive(true);
            }

            this.toggle_Ui = !this.toggle_Ui;
        }

        public void OnClickChange()
        {
            Debug.Log("Change");


        }

        public void ChangeScrollList(float Scroll_Value)
        {
            Debug.Log($"OnScroll {Scroll_Value}");
            /*
                        if (Scroll_Value > 0.8f)
                        {
                            scrollbar_Horizontal.value = 0.56f;


                        }
                        else if (Scroll_Value < 0.2f)
                        {
                            scrollbar_Horizontal.value = 0.56f;



                        }
                        else
                        {

                        }
            */
        }


    }
}