using System;
using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;

public class StateDisplay_Control : MonoBehaviour
{
    [Header("Component Ref")]
    [SerializeField] private Transform[] statePos;
    [SerializeField] private Transform[] stateObject;
    [SerializeField] private List<Tuple<int, Transform>> stateObject_WithPos;
    [SerializeField] private List<Tuple<int, Transform>> stateObject_OutPos;

    [Header("Current State Movement")]
    [SerializeField] private int position_HideIndex;

    [Header("State Movement Sort")]
    [SerializeField] private bool isStateMove;

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

    public void Update_PositionState()
    {
        Load_ObjectAndPosition();
    }


    public async void Contine_State()
    {
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
    }

    public void Setting_StateInfo()
    {
        Load_ObjectAndPosition();
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
