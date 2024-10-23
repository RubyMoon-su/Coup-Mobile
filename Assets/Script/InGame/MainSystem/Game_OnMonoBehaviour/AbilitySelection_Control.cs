using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Coup_Mobile.InGame.GameManager.Ui;
using System.Linq;
using System.Threading.Tasks;

public class AbilitySelection_Control : MonoBehaviour, Display_UiController
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
    [SerializeField] public bool isConnected_UiControl;


    // Scroll Target ID.
    [SerializeField] public string isCommandSelect { get; private set; }
    [SerializeField] public string isTargetSelect { get; private set; }
    [SerializeField] public List<Tuple<string, string>> commandAndTitleName = new List<Tuple<string, string>>();
    [SerializeField] public Text titleName_Text;

    private Dictionary<string, object> defaultSetting = null;

    void Start()
    {
        old_CurrentItem = int.MaxValue;

        isSnapped = false;
        isConnected_UiControl = false;
    }

    void Update()
    {
        if (isConnected_UiControl) OnProcess_Scroll();
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
                isSnapped = true; // Set isSanped to true to indicate that snapping is complete
            }
        }
        // If the scrolling speed of the ScrollRect is greater than 200 units.
        if (scrollRect.velocity.magnitude > 200)
        {
            isCommandSelect = commandAndTitleName[CurrentItem].Item1;

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

        if (SelectionAbility >= MinAbliltyCount && SelectionAbility <= MaxAbilityCount)
        {
            button_pos[SelectionAbility].interactable = true;
        }
    }

    private void ShowTitle(int index)
    {
        if (index > MaxAbilityCount)
        {
            titleName_Text.text = "";
            return;
        }

        string Combine = commandAndTitleName[index].Item2 != null
                            ? commandAndTitleName[index].Item2
                            : "";

        titleName_Text.text = Combine;
    }

    public async void OnClick_Selection(string abilityName)
    {
        int Result = await VerifySelection_AbilityBox(abilityName);

        if (Result == 0)
        {
            // Send Event To the GameState Manager.
        }
        else if (Result == 1)
        {
            // Cancel a Send Event.
        }
        else 
        {
            // Cancel a send Event.
        }
    }

    private async Task<int> VerifySelection_AbilityBox(string abilityName)
    {
        

        return 2;
    }

    private string FindAbility_CodeName(string abilityName)
    {
        return abilityName switch
        {
            "Income" => "Operation Treasury",
            "Forget Aid" => "Operation Endowment",
            "Coup" => "Coup Directive",
            "Duke" => "Operation Dominion",
            "Assassin" => "Shadow Command",
            "Captain" => "Golden Siege",
            "Ambassdor" => "Treaty Dominion",
            "Contassa" => "Royal Decree",
            "Inquisitor" => "Operation Viper",
            _ => throw new ArgumentException($"AbilitySelection_Control -> FindAbility_CodeName | Unknown AbilityName Topic : {abilityName}"),
        };
    }

    private void AddAbility(Dictionary<string, object> packetData)
    {

    }

    private void RemoveAbility(string removeAbilityName)
    {

    }

    private void AddTarget(string targetName)
    {

    }

    private void RemoveTarget(string removeTargetName)
    {

    }

    private void ResetTo_default()
    {

    }

    #endregion

    #region Event Control

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
                case "ButtonObject":
                    if (item.Value is not Button[]) 
                        throw new ArgumentException($"{ExceptionMessage} | key : {item.Key} is not Button[] type.");
                    break;
                default: throw new Exception($"Key : {item.Key} is Unknown Topic.");
            }
        }

        return isCorrect;
    }


    public void StarterAndSetting(object packetData)
    {
        StarterAndSetting_CheckType(packetData);

        var settingAbility = (Dictionary<string, object>)packetData;

        MaxAbilityCount = (int)settingAbility["MaxniumIndex"];
        MinAbliltyCount = (int)settingAbility["MinniumIndex"];

        pos = (Transform[])settingAbility["PositionObject"];
        button_pos = (Button[])settingAbility["ButtonObject"];

        foreach (var item in pos)
        {
            string CommandName = item.gameObject.name;
            string TitleName = string.Empty;

            // Spcial CodeName of Commands.
            TitleName = FindAbility_CodeName(CommandName);

            commandAndTitleName.Add
            (
                new Tuple<string, string>(CommandName, TitleName != string.Empty ? TitleName : "Unknow")
            );
        }

        foreach (var item in button_pos)
        {
            item.onClick.AddListener(() => OnClick_Selection(item.gameObject.name));
        }

        defaultSetting = settingAbility;
        isConnected_UiControl = true;
    }



    public void CommandExecute(string target, object packetData)
    {
        string shotException = "AbilitySelection_Control -> CommandExecute |";

        switch (target)
        {
            case "AddAbility":
                AddAbility(packetData is Dictionary<string, object>
                    ? (Dictionary<string, object>)packetData
                    : throw new ArgumentException($"{shotException} packetData is not Dictionary type."));
                break;
            case "RemoveAbility":
                RemoveAbility(packetData is string
                    ? (string)packetData
                    : throw new ArgumentException($"{shotException} packetData is not string type."));
                break;
            case "AddTarget":
                AddTarget(packetData is string
                    ? (string)packetData
                    : throw new ArgumentException($"{shotException} packetData is not string type."));
                break;
            case "RemoveTarget":
                RemoveTarget(packetData is string
                    ? (string)packetData
                    : throw new ArgumentException($"{shotException} packetData is not string type."));
                break;
            case "ResetTo_default":
                ResetTo_default();
                break;
            default: throw new Exception("");
        }
    }

    public object ReturnExecute(string target, object packetData)
    {
        return target switch
        {
            "SelectedCommand" => isCommandSelect,
            "SelectedTarget" => isTargetSelect,
            //Add More Request In Here.
            _ => null,
        };
    }

    #endregion
}
