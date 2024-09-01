using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Coup_Mobile.InGame.GameManager.Ui
{
    public class UI_WaveGame_Control : UI_Control
    {
        private StateDisplay_Control stateDisplayInstance;

        private Transform positionPath;
        private Transform objectPath;
        private Transform backGroundPath;
        private GameObject[] prefabs;
        private GameObject[] prefabs_Instance;

        private Action settingState_Event;


        public UI_WaveGame_Control(GameUiManager gameUiManager) : base(gameUiManager)
        {
            Install_System();
        }

        protected async override void Install_System()
        {
            GameManager_Event Event = GameManager_Event.GameAssistManager;
            object EndPoint = GameAssistManager_List.StateSystem_Assist;

            Transform StateDisplay_Instance = (Transform)CreatePacket_Request(Event, EndPoint, "Instance");
            Transform StateDisplay_PositionPath = (Transform)CreatePacket_Request(Event, EndPoint, "Position_Path");
            Transform StateDisplay_ObjectPath = (Transform)CreatePacket_Request(Event, EndPoint, "StateObject_Path");
            Transform StateDisplay_StateBackGroundPath = (Transform)CreatePacket_Request(Event, EndPoint, "StateBackGround");
            GameObject[] StateDisplay_Prefub = (GameObject[])CreatePacket_Request(Event, EndPoint, "Prefubs");

            stateDisplayInstance = StateDisplay_Instance.GetComponent<StateDisplay_Control>();

            positionPath = StateDisplay_PositionPath;
            objectPath = StateDisplay_ObjectPath;
            backGroundPath = StateDisplay_StateBackGroundPath;
            prefabs = StateDisplay_Prefub;

            await Sort_StateDisplay();

            await Register_ActionEvent();
        }

        private async Task Register_ActionEvent()
        {
            await Task.Delay(0);

            settingState_Event = stateDisplayInstance.Setting_StateInfo;

            settingState_Event.Invoke();
        }

        private async Task Sort_StateDisplay()
        {
            GameManager_Event Event = GameManager_Event.GameStateManager;
            object EndPoint = GameStateManager_List.GetAll_MainState;

            List<GameState_List> AllStateDisplay = (List<GameState_List>)CreatePacket_Request(Event, EndPoint);

            prefabs_Instance = new GameObject[AllStateDisplay.Count];

            for (int i = 0; i < AllStateDisplay.Count; i++)
            {
                GameState_List State = AllStateDisplay[i];

                GameObject Selection_Prefabs = null;
                //Debug.LogWarning(State);
                switch (State)
                {
                    case GameState_List.Next_Wave_State:
                        Selection_Prefabs = Find_StatePrefabs("NextWaveAction");
                        break;
                    case GameState_List.Player_Send_Command:
                        Selection_Prefabs = Find_StatePrefabs("SendAction");
                        break;
                    case GameState_List.Player_Counter_Command:
                        Selection_Prefabs = Find_StatePrefabs("CounterAction");
                        break;
                    case GameState_List.Player_Verifiy_Command:
                        Selection_Prefabs = Find_StatePrefabs("VerifyAction");
                        break;
                    case GameState_List.Player_Result_Command:
                        Selection_Prefabs = Find_StatePrefabs("ResultAction");
                        break;
                    case GameState_List.Wait_For_AllPlayerAndProcessGame:
                        Selection_Prefabs = Find_StatePrefabs("WaitAction");
                        break;
                    default:
                        Debug.LogError($"{State} is not been set.");
                        break;
                }

                if (Selection_Prefabs == null)
                {
                    Debug.LogError($"Selection_Prefabs is null.");
                    return;
                }

                await Task.Delay(0);

                GameObject State_Instance = UnityEngine.Object.Instantiate<GameObject>(Selection_Prefabs , objectPath);

                prefabs_Instance[i] = State_Instance;
            }
        }



        private GameObject Find_StatePrefabs(string statePrefabs_Name)
        {
            foreach (GameObject Item in prefabs)
            {
                if (Item.name == statePrefabs_Name)
                {
                    return Item;
                }
            }

            return null;
        }
        public override void OnInteractive_UI(string requestment , object packet_Data)
        {

        }
    }
}