using System;
using Photon.Pun;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using Coup_Mobile.EventBus;
using System.Threading.Tasks;
using System.Collections.Generic;
using Coup_Mobile.InGame.PlayerData;
using Coup_Mobile.InGame.GameManager;
using Coup_Mobile.InGame.GameManager.ReportData;

public class Display_PreviewCard : MonoBehaviour
{
    #region Common Field
    [Header("Common Field")]
    [SerializeField] private Transform content_Path;
    [SerializeField] private Transform disable_cardPath;
    [SerializeField] private List<GameObject> card_Active;
    [SerializeField] private List<GameObject> card_Disable;
    [SerializeField] private Button close_WindowButton;
    [SerializeField] private Button open_WindowButton;
    [SerializeField] private double spacing = 450d;
    [SerializeField] private int amountOfCards_Group = 5;

    [Space(5)]
    [Header("Display Field")]
    [SerializeField] private Text display_Topic;

    #endregion

    #region Sanp Field

    [Space(5)]
    [Header("Sanp Field")]
    //Sanp Field.
    [SerializeField] public ScrollRect scrollRect;
    [SerializeField] public RectTransform contentPanel;
    [SerializeField] public RectTransform sampleListItem;

    [SerializeField] public HorizontalLayoutGroup HLG;
    [SerializeField] public int old_CurrentItem;
    [SerializeField] public float sanpForce;
    [HideInInspector] public float snapSpeed;
    [SerializeField] public int MaxAbilityCount;
    [SerializeField] public int MinAbliltyCount;

    #endregion

    #region Toggle Status

    [Space(5)]
    [Header("Toggle Status")]
    public bool isSnapped;
    public bool isHaveCard;

    #endregion

    public void Start()
    {
        Install_DisplaySystem();
    }

    private async void Install_DisplaySystem()
    {
        isHaveCard = false;

        await FindRef();

        await LoadCardInfo_And_CreateCardInfo();

        await ButtonEvent_Setting();

        display_Topic.text = "My Card";
        display_Topic.color = Color.white;

        gameObject.SetActive(false);
    }

    private async Task FindRef()
    {
        // Find And Getting instance Ref.
        content_Path = transform.Find("Scroll View/Viewport/Content").transform;
        disable_cardPath = transform.Find("Disable_Card").transform;
        close_WindowButton = transform.Find("CardDisplay_Button").transform.GetComponent<Button>();
        open_WindowButton = transform.parent.transform.Find("CardInfoButton").transform.GetComponent<Button>();

        display_Topic = transform.Find("Topic").GetComponent<Text>();

        await Task.Delay(0);
    }

    private async Task LoadCardInfo_And_CreateCardInfo()
    {
        // Loading Cards Info Prefab from the Resource Folder.
        GameObject[] CardInfo = Resources.LoadAll<GameObject>("Character_Card");

        List<GameObject> CardAll = new List<GameObject>();
        List<string> CardName = new List<string>();

        // Create card groups based on their values.
        for (int i = 0; i < amountOfCards_Group; i++)
        {
            for (int o = 0; o < CardInfo.Length; o++)
            {
                GameObject CardInfoPrefab = Instantiate(CardInfo[o], disable_cardPath);

                // Remove "Clone" from the GameObject name.
                string OldName = CardInfoPrefab.name;
                string NewName = OldName.Replace("(Clone)", "");
                CardInfoPrefab.name = NewName;

                Button ClickEventButton = CardInfoPrefab.GetComponent<Button>();

                // Change the color of a button.
                ClickEventButton.transition = Selectable.Transition.ColorTint;

                ColorBlock NewSetColor = ClickEventButton.colors;
                NewSetColor.highlightedColor = Color.yellow;

                ClickEventButton.colors = NewSetColor;
                ClickEventButton.onClick.AddListener(() => Debug.LogError(CardInfoPrefab.name));

                CardAll.Add(CardInfoPrefab);
                CardName.Add(CardInfoPrefab.name);

                CardInfoPrefab.SetActive(false);
            }
        }

        card_Disable = CardAll;

        await Task.Delay(0);
    }

    private async Task ButtonEvent_Setting()
    {
        // Install a callback function in the close window button variable.
        close_WindowButton.onClick.AddListener(Close_Display);

        // Install a callback function in the open window button variable.
        // In the first event, this event will activate the GameObject during the callback process.
        open_WindowButton.onClick.AddListener(() =>
        {
            if (this.gameObject.activeSelf)
            {
                // Not Do anything.
            }
            else
            {
                this.gameObject.SetActive(true);
            }
        });

        // In the second event, this event will call the process to open the display function.
        open_WindowButton.onClick.AddListener(Open_Display);

        await Task.Delay(0);
    }

    #region  Scroll System

    public void Update()
    {
        if (isHaveCard) OnProcess_Scroll();
    }

    private void OnProcess_Scroll()
    {
        // Find the current step by adding the width of the item and the distance of the Horizontal Layout Group,
        // then divide the result by the x position of the content.  
        int CurrentItem = Mathf.RoundToInt(0 - contentPanel.localPosition.x / (sampleListItem.rect.width + HLG.spacing));

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
            // Reset the snapping status.
            isSnapped = false;

            // Reset the snap speed.
            snapSpeed = 0;
        }
    }


    #endregion

    #region Display Function Interactive

    public void EnablePreview_Card(string[] ShowCard)
    {
        GameObject[] CardRecord = new GameObject[ShowCard.Length];

        // Check total Number of Card that will be Shown on the Screen.
        MaxAbilityCount = ShowCard.Length - 1;

        // Setup All Card in Global Variable.
        int Index = 0;

        // Find all cards with names that match the show card's name.
        foreach (string CardID in ShowCard)
        {
            foreach (GameObject SelectGO in card_Disable)
            {
                if (SelectGO.name == CardID && !CardRecord.Contains(SelectGO))
                {
                    CardRecord[Index] = SelectGO;
                }
            }

            Index++;
        }

        // Remove Cards in order.
        foreach (GameObject RemoveGO in CardRecord)
        {
            card_Disable.Remove(RemoveGO);
        }

        card_Active = CardRecord.ToList();

        // Activate the selected cards and move tham to the new Parent Path. 
        foreach (GameObject Card in card_Active)
        {
            // Set Active Card.
            Card.SetActive(true);

            // Move Card To Content Folder.
            Card.transform.SetParent(content_Path);

            // Setup default position.
            Card.transform.position = content_Path.position;
        }

        // Control the scroll system, where cardActive will have a value.
        if (card_Active.Count > 0)
        {
            isHaveCard = true;
        }
        else
        {
            isHaveCard = false;
        }
    }

    public void DisablePreview_Card()
    {
        if (card_Active != null && card_Active.Count > 0)
        {
            // Loop all of Cards has Activated.
            foreach (GameObject Card in card_Active)
            {
                // Move Card to Disable Folder.
                Card.transform.SetParent(disable_cardPath);

                // Move the activated card back to the disabled folder.
                card_Disable.Add(Card);

                //Disable Card.
                Card.SetActive(false);
            }

            // Clear Card.
            card_Active = null;
        }
    }

    #endregion

    #region Button Event Function

    public void Open_Display()
    {
        // request a player cards from the PlayerManager.
        var EventPath = GameManager_Event.PlayerManager;
        object EndPoint = PlayerManager_List.Get_SelectionPlayerData;
        string playername = PhotonNetwork.LocalPlayer.NickName;

        GameManager_Data Get_PlayerCard = new GameManager_Data()
        {
            gameManager_Event = EventPath,
            EndPoint = EndPoint,
            PacketData = playername,
        };

        // Get Player Data from the EventBus.
        var RequestData = (PlayerManager_Return)EventBus_InGameManager<IInGameEvent>.RaiseGameCommand(Get_PlayerCard);
        var PlayerInfo = RequestData.return_Data as Player_Data?;

        if (PlayerInfo == null || PlayerInfo is not Player_Data)
        {
            Debug.LogError($"Error PlayerInfo is = {PlayerInfo != null} || PlayerInfo is not Player_Data = {PlayerInfo is not Player_Data}");

            return;
        }

        string[] CardInfo = PlayerInfo.Value.usedCharacter;

        EnablePreview_Card(CardInfo);
    }

    public void Close_Display()
    {
        // Stop Snapping Card Process.
        isHaveCard = false;
        MaxAbilityCount = 0;

        // Reset position to the default value.
        contentPanel.localPosition = new Vector3(0, 0, 0);

        // Clear All of Cards in Content Folder.
        DisablePreview_Card();

        // Hide the Display Card in a Window.
        gameObject.SetActive(false);
    }

    #endregion
}

/*
// Find Position per Card.
int CardCount = ShowCard.Length;

if (CardCount == 1)
{
    ShowCard[0].transform.SetParent(content_Path);
    ShowCard[0].transform.localPosition = content_Path.position;
}
else
{
    double totalWidth = (CardCount - 1) * spacing;
    double halfwidth = totalWidth / 2;

    for (int i = 0; i < CardCount; i++)
    {
        double x  = i * spacing - halfwidth;
        ShowCard[i].transform.SetParent(content_Path);

        Vector2 NewPosition = new Vector2 (content_Path.position.x + (float)x , content_Path.position.y);

        ShowCard[i].transform.position = NewPosition;
    }
}
*/
