using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Coup_Mobile.InGame.GameManager.Ui;

public class TargetSelection_Control : MonoBehaviour, Display_UiController
{
    [SerializeField] public ScrollRect scrollRect;
    [SerializeField] public RectTransform contentPanel;
    [SerializeField] public RectTransform sampleListItem;

    [SerializeField] public HorizontalLayoutGroup HLG;
    [SerializeField] public Transform[] pos;
    [SerializeField] public Button[] button_pos;
    [SerializeField] public int old_CurrentItem;
    [SerializeField] public float sanpForce;
    [HideInInspector] public float snapSpeed;
    [SerializeField] public int MaxAbilityCount;
    [SerializeField] public int MinAbliltyCount;

    [SerializeField] public bool isSnapped;
    [SerializeField] public bool isConnected_UiControl = false;

    // Scroll Target ID.
    [SerializeField] public string isTargetSelect;
    [SerializeField] public List<string> targetName = new List<string>();
    [SerializeField] public Dictionary<string, object> defaultSetting = new Dictionary<string, object>();
    [SerializeField] public Text titleName_Text;

    void Update()
    {
        if (isConnected_UiControl) OnProcess_Scroll();
    }

    public void StarterAndSetting(object packetData)
    {
        old_CurrentItem = int.MaxValue;

        isSnapped = false;
        isConnected_UiControl = false;

        Setting_TargetScroll(packetData);
    }

    public void CommandExecute(string target, object packetData)
    {
        switch (target)
        {
          
        }
    }

    public object ReturnExecute(string target, object packetData)
    {
        return target switch
        {
            "GetisTargetSelected" => isTargetSelect,
        };
    }

    #region Local Function

    private void OnProcess_Scroll()
    {
        // Find the current step by adding the width of the item and the distance of the Horizontal Layout Group,
        // then divide the result by the x position of the content.  
        int CurrentItem = Mathf.RoundToInt(0 - contentPanel.localPosition.x / (sampleListItem.rect.width + HLG.spacing));

        if (CurrentItem != old_CurrentItem)
        {
            old_CurrentItem = CurrentItem;
            ChangeLocalScaleUI(CurrentItem);
            Interctable_AbilityUI(CurrentItem);

            ShowTitle(CurrentItem);
        }

        if (CurrentItem > MaxAbilityCount) CurrentItem = MaxAbilityCount;

        else if (CurrentItem < MinAbliltyCount) CurrentItem = MinAbliltyCount;

        // If the scrolling speed of the ScrollRect is less than 200 units, and snapping has not occurred yet. 
        if (scrollRect.velocity.magnitude < 200 && !isSnapped)
        {
            // Stop the scrolling speed of the ScrollRect.
            scrollRect.velocity = Vector2.zero;

            // Increase the snap speed based on sanpForce and Time.deltaTime.
            snapSpeed += sanpForce * Time.deltaTime;

            float MoveTowards = Mathf.MoveTowards(contentPanel.localPosition.x, 0 - (CurrentItem * (sampleListItem.rect.width + HLG.spacing)), snapSpeed);

            // Gradually move the x position of contentPanel to the calculated target position based on CurrentItem.
            contentPanel.localPosition = new Vector3(
                 MoveTowards
               , contentPanel.localPosition.y
                , contentPanel.localPosition.z);

            // If the x position of contentPanel equals the target position (CurrentItem), the snap is complete.
            if (contentPanel.localPosition.x == 0 - (CurrentItem * (sampleListItem.rect.width + HLG.spacing)))
            {
                isTargetSelect = targetName[CurrentItem];
                isSnapped = true; // Set isSanped to true to indicate that snapping is complete
            }
        }
        // If the scrolling speed of the ScrollRect is greater than 200 units.
        if (scrollRect.velocity.magnitude > 200)
        {
            // Reset the snapping status.
            isSnapped = false;

            // Reset the snap speed.
            snapSpeed = 0;
        }
    }

    private void ChangeLocalScaleUI(int SelectionAbility)
    {
        for (int i = 0; i < pos.Length; i++)
        {
            pos[i].localScale = new Vector2(0.8f, 0.8f);

        }

        if (SelectionAbility >= MinAbliltyCount && SelectionAbility <= MaxAbilityCount)
        {
            pos[SelectionAbility].transform.localScale = new Vector2(1f, 1f);
        }
    }

    private void Interctable_AbilityUI(int SelectionAbility)
    {
        for (int i = 0; i < button_pos.Length; i++)
        {
            button_pos[i].interactable = false;

        }

        //if (SelectionAbility >= MinAbliltyCount && SelectionAbility <= MaxAbilityCount)
        //{
           // button_pos[SelectionAbility].interactable = true;
        //}
    }

    private void ShowTitle(int index)
    {
        if (index > MaxAbilityCount)
        {
            titleName_Text.text = "";
            return;
        }

        titleName_Text.text = targetName[index];

    }

    #endregion

    #region Event Control

    public void Setting_TargetScroll(object packetData)
    {
        StarterAndSetting_CheckType(packetData);

        var settingTargetList = (Dictionary<string, object>)packetData;

        MaxAbilityCount = (int)settingTargetList["MaxniumIndex"];
        MinAbliltyCount = (int)settingTargetList["MinniumIndex"];
        pos = (Transform[])settingTargetList["PositionObject"];

        foreach (var item in pos)
        {
            string TargetName = item.gameObject.name;

            targetName.Add(TargetName);
        }

        defaultSetting = settingTargetList;
        isConnected_UiControl = true;
    }



    #endregion

    private bool StarterAndSetting_CheckType(object checkType)
    {
        bool isCorrect = true;

        var VerifyType = checkType is Dictionary<string, object>
            ? (Dictionary<string, object>)checkType
            : null;

        string ExceptionMessage = "AbilitySelection_Control -> StarterAndSetting_CheckType";

        foreach (var item in VerifyType)
        {
            switch (item.Key)
            {
                case "MaxniumIndex":
                case "MinniumIndex":
                    if (item.Value is not int)
                        throw new ArgumentException($"{ExceptionMessage} | key : {item.Key} is not int type.");
                    break;
                 case "PositionObject":
                    if (item.Value is not Transform[]) 
                        throw new ArgumentException($"{ExceptionMessage} | key : {item.Key} is not Transform[] type.");
                    break;
                default: throw new Exception($"Key : {item.Key} is Unknown Topic.");
            }
        }

        return isCorrect;
    }
}
