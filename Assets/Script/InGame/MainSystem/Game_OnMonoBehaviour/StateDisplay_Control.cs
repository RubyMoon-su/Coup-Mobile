using System;
using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;
using Coup_Mobile.InGame.GameManager.Ui;
using UnityEngine.UI;

public class StateDisplay_Control : MonoBehaviour, Display_UiController
{
    [Header("Component Ref")]
    [SerializeField] private Transform[] statePos;
    [SerializeField] private Transform[] stateObject;
    [SerializeField] private List<Tuple<int, Transform>> stateObject_WithPos;
    [SerializeField] private List<Tuple<int, Transform>> stateObject_OutPos;

    [SerializeField] private string currentState_Icon;

    [Header("Current State Movement")]
    [SerializeField] private int position_HideIndex;

    [Header("State Movement Sort")]
    [SerializeField] private bool isStateMove;

    [SerializeField] private Dropdown testDropdown;

    #region Process And Local Function

    void Update()
    {
        OnStateDisplay_Process();
    }

    private void OnStateDisplay_Process()
    {
        // Wait For Object Movement Animation.
        if (!isStateMove) return;
    }

    #endregion

    #region Event Control

    public void StarterAndSetting(object packetData)
    {
        Load_ObjectAndPosition();
    }

    public void CommandExecute(string target, object packetData)
    {
        switch (target)
        {
            // Resetting Control.
            case "UpdateAssistState":
                Load_ObjectAndPosition();
                break;

            // State Control.
            case "NextState":
                Contine_State("NextState", null);
                break;
            case "ReverseState":
                Contine_State("ReverseState", null);
                break;
            case "JumpTo_State":
                Contine_State("JumpTo_State", packetData is string
                    ? (string)packetData
                    : throw new ArgumentException($"StateDIsplay_Control -> CommandExecute -> 'JumpTo_State' | PacketData is not string type."));
                break;

            // Specific State Control.
            case "RandomState":
                RandomState(null);
                break;
            default: throw new ArgumentException($"StateDIsplay_Control -> CommandExecute | Unkown Topic : {target}");
        }
    }

    public object ReturnExecute(string target, object packetData)
    {
        return target switch
        {
            "CurrentState" => currentState_Icon,
            _ => throw new ArgumentException($"StateDisplay_Control -> ReturnExecute | Unkown Topic : {target}"),
        };
    }
    public void Test()
    {
        RandomState(testDropdown.value);
    }

    public void Test2() => RandomState(null);

    private void RandomState(int? Fixindex)
    {
        int RandomState = Fixindex.HasValue ? Fixindex.Value : UnityEngine.Random.Range(0, 5);
        string EndPoint = string.Empty;

        switch (RandomState)
        {
            case 0:
                EndPoint = "NextWaveAction";
                break;
            case 1:
                EndPoint = "SendAction";
                break;
            case 2:
                EndPoint = "CounterAction";
                break;
            case 3:
                EndPoint = "VerifyAction";
                break;
            case 4:
                EndPoint = "ResultAction";
                break;
            case 5:
                EndPoint = "WaitAction";
                break;
        }

        Contine_State("JumpTo_State", EndPoint);
    }

    public async void Contine_State(string statusMovement, string stateTarget)
    {
        switch (statusMovement)
        {
            case "NextState":

                await Next_StateSort();

                for (int i = 0; i < stateObject_WithPos.Count; i++)
                {
                    int MoveIndex = stateObject_WithPos[i].Item1;
                    Transform SelectionState = stateObject_WithPos[i].Item2;

                    Transform NewPosition = statePos[MoveIndex].transform;

                    SelectionState.position = NewPosition.position;
                }

                for (int i = 0; i < stateObject_OutPos.Count; i++)
                {
                    int MoveIndex = stateObject_OutPos[i].Item1;
                    Transform SelectionState = stateObject_OutPos[i].Item2;

                    Transform NewPosition = statePos[statePos.Length - 1].transform;

                    SelectionState.position = NewPosition.position;
                }
                break;
            case "ReverseState":

                await Backward_StateSort();

                for (int i = stateObject_WithPos.Count - 1; i >= 0; i--)
                {
                    int MoveIndex = stateObject_WithPos[i].Item1;
                    Transform SelectionState = stateObject_WithPos[i].Item2;

                    Transform NewPosition = statePos[MoveIndex].transform;

                    SelectionState.position = NewPosition.position;
                }

                for (int i = stateObject_OutPos.Count - 1; i >= 0; i--)
                {
                    int MoveIndex = stateObject_OutPos[i].Item1;
                    Transform SelectionState = stateObject_OutPos[i].Item2;

                    Transform NewPosition = statePos[0].transform;

                    SelectionState.position = NewPosition.position;
                }
                break;
            case "JumpTo_State":

                int CountIndex = -1;

                for (int i = 0; i < stateObject_WithPos.Count; i++)
                {
                    if (stateObject_WithPos[i].Item2.name == stateTarget)
                    {
                        CountIndex = i;
                    }
                }

                if (CountIndex == -1)
                {
                    for (int i = 0; i < stateObject_OutPos.Count; i++)
                    {
                        if (stateObject_OutPos[i].Item2.name == stateTarget)
                        {
                            CountIndex = stateObject_WithPos.Count + i;
                        }
                    }

                    if (CountIndex < 0) throw new Exception("");
                }

                for (int i = 0; i < CountIndex; i++)
                {
                    await Next_StateSort();

                    await Task.Delay(1000);

                    for (int n = stateObject_WithPos.Count - 1; n >= 0; n--)
                    {
                        int MoveIndex = stateObject_WithPos[n].Item1;
                        Transform SelectionState = stateObject_WithPos[n].Item2;

                        Transform NewPosition = statePos[MoveIndex].transform;

                        SelectionState.position = NewPosition.position;
                    }

                    for (int o = stateObject_OutPos.Count - 1; o >= 0; o--)
                    {
                        int MoveIndex = stateObject_OutPos[o].Item1;
                        Transform SelectionState = stateObject_OutPos[o].Item2;

                        Transform NewPosition = statePos[0].transform;

                        SelectionState.position = NewPosition.position;
                    }
                }

                break;
        }
    }

    #endregion

    #region  Local Function

    private async Task Next_StateSort()
    {
        await Task.Delay(0);

        // Create lists to store the new sorted positions and out-of-sort positions.
        List<Tuple<int, Transform>> NewPosition_Sort = new List<Tuple<int, Transform>>();
        List<Tuple<int, Transform>> NewPosition_OutSort = new List<Tuple<int, Transform>>();

        // Iterate through each state object currently in stateObject_WithPos.
        foreach (var State in stateObject_WithPos)
        {
            int CurrentIndex = State.Item1;
            Transform CurrentPosition = State.Item2;

            int NewCurrentIndex = CurrentIndex - 1;  // Calculate the new index by subtracting 1.

            // If the new index is less than 0, move the object to the out-of-sort list.
            if (NewCurrentIndex < 0)
            {
                NewPosition_OutSort.Add(new Tuple<int, Transform>(0, CurrentPosition));
            }
            else
            {
                // Otherwise, add the object to the new sorted list with the updated index.
                NewPosition_Sort.Add(new Tuple<int, Transform>(NewCurrentIndex, CurrentPosition));
            }
        }

        // Ensure that the number of objects in NewPosition_Sort matches the original count in stateObject_WithPos.
        while (NewPosition_Sort.Count < stateObject_WithPos.Count)
        {
            // If there are items in the out-of-sort list, move the first item to the sorted list.
            if (stateObject_OutPos.Count > 0)
            {
                var FirstOutPost = stateObject_OutPos[0];
                stateObject_OutPos.RemoveAt(0);

                // Add the item to NewPosition_Sort with the next available index.
                NewPosition_Sort.Add(new Tuple<int, Transform>(NewPosition_Sort.Count, FirstOutPost.Item2));
            }
            else
            {
                break;  // Exit the loop if no more items are available in stateObject_OutPos.
            }
        }

        // Re-add items from NewPosition_OutSort to the out-of-sort list, with index starting from the current count.
        foreach (var item in NewPosition_OutSort)
        {
            stateObject_OutPos.Add(new Tuple<int, Transform>(stateObject_OutPos.Count, item.Item2));
        }

        // Update the stateObject_WithPos with the new sorted positions.
        stateObject_WithPos = NewPosition_Sort;

    }

    private async Task Backward_StateSort()
    {
        await Task.Delay(0);

        // Create lists to store the new sorted positions and out-of-sort positions.
        List<Tuple<int, Transform>> NewPosition_Sort = new List<Tuple<int, Transform>>();
        List<Tuple<int, Transform>> NewPosition_OutSort = new List<Tuple<int, Transform>>();

        // Iterate through each state object currently in stateObject_WithPos.
        foreach (var State in stateObject_WithPos)
        {
            int CurrentIndex = State.Item1;
            Transform CurrentPosition = State.Item2;

            int NewCurrentIndex = CurrentIndex + 1;  // Calculate the new index by adding 1 (moving backward).

            // If the new index exceeds the maximum index, move the object to the out-of-sort list.
            if (NewCurrentIndex >= stateObject_WithPos.Count)
            {
                NewPosition_OutSort.Add(new Tuple<int, Transform>(stateObject_OutPos.Count, CurrentPosition));
            }
            else
            {
                // Otherwise, add the object to the new sorted list with the updated index.
                NewPosition_Sort.Add(new Tuple<int, Transform>(NewCurrentIndex, CurrentPosition));
            }
        }

        // Ensure that the number of objects in NewPosition_Sort matches the original count in stateObject_WithPos.
        while (NewPosition_Sort.Count < stateObject_WithPos.Count)
        {
            // If there are items in the out-of-sort list, move the first item to the sorted list.
            if (stateObject_OutPos.Count > 0)
            {
                var FirstOutPost = stateObject_OutPos[0];
                stateObject_OutPos.RemoveAt(0);

                // Add the item to NewPosition_Sort with the next available index.
                NewPosition_Sort.Add(new Tuple<int, Transform>(NewPosition_Sort.Count, FirstOutPost.Item2));
            }
            else
            {
                break;  // Exit the loop if no more items are available in stateObject_OutPos.
            }
        }

        // Re-add items from NewPosition_OutSort to the out-of-sort list, with index starting from the current count.
        foreach (var item in NewPosition_OutSort)
        {
            stateObject_OutPos.Add(new Tuple<int, Transform>(stateObject_OutPos.Count, item.Item2));
        }

        // Update the stateObject_WithPos with the new sorted positions.
        stateObject_WithPos = NewPosition_Sort;
    }

    /// <summary>
    /// Find All Position in Position GameObject.
    /// Install All Position In statePos.
    /// </summary>
    private void Load_ObjectAndPosition()
    {
        Transform State_Pos = transform.Find("StateDisplay/Position");
        statePos = new Transform[State_Pos.childCount];
        for (int i = 0; i < State_Pos.childCount; i++)
        {
            statePos[i] = State_Pos.GetChild(i);
        }

        Transform State_Path = transform.Find("StateDisplay/StateObject");
        stateObject = new Transform[State_Path.childCount];
        for (int i = 0; i < State_Path.childCount; i++)
        {
            stateObject[i] = State_Path.GetChild(i);
        }

        stateObject_WithPos = new List<Tuple<int, Transform>>();
        stateObject_OutPos = new List<Tuple<int, Transform>>();

        int OutPosition_index = 0;

        // Setting stateObject Position.
        for (int i = 0; i < stateObject.Length; i++)
        {
            if (i < statePos.Length)
            {
                stateObject[i].transform.position = statePos[i].transform.position;

                stateObject_WithPos.Add(new Tuple<int, Transform>(i, stateObject[i]));

                //Debug.Log($"WithPos {stateObject_WithPos[i].Item2.name} is Current Index {stateObject_WithPos[i].Item1}");
            }
            else
            {
                stateObject[i].transform.position = stateObject[statePos.Length - 1].transform.position;

                stateObject_OutPos.Add(new Tuple<int, Transform>(OutPosition_index, stateObject[i]));

                //Debug.Log($"OutPos {stateObject_OutPos[OutPosition_index].Item2.name} is Current Index {stateObject_OutPos[OutPosition_index].Item1}");

                OutPosition_index++;
            }


        }
    }



    #endregion
}
