using UnityEngine;
using UnityEngine.UI;

public class AbilitySelection_Control : MonoBehaviour
{
    [SerializeField] public ScrollRect scrollRect;
    [SerializeField] public RectTransform contentPanel;
    [SerializeField] public RectTransform sampleListItem;

    [SerializeField] public HorizontalLayoutGroup HLG;
    [SerializeField] public Transform[] pos;
    [SerializeField] public int old_CurrentItem;
    [SerializeField] public float sanpForce;
    [HideInInspector] public float snapSpeed;
    [SerializeField] public int MaxAbilityCount;
    [SerializeField] public int MinAbliltyCount;

    [SerializeField] public bool isSnapped;
    [SerializeField] public bool isConnected_UiControl;


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

    private void OnProcess_Scroll()
    {
        // Find the current step by adding the width of the item and the distance of the Horizontal Layout Group,
        // then divide the result by the x position of the content.  
        int CurrentItem = Mathf.RoundToInt(0 - contentPanel.localPosition.x / (sampleListItem.rect.width + HLG.spacing));


        if (CurrentItem != old_CurrentItem)
        {
            old_CurrentItem = CurrentItem;
            ChangeLocalScaleUI(CurrentItem);
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

    public void Setting_AbilityScroll(int MaxAbilityCount, int MinAbliltyCount, Transform[] pos)
    {
        this.MaxAbilityCount = MaxAbilityCount;
        this.MinAbliltyCount = MinAbliltyCount;

        this.pos = pos;

        isConnected_UiControl = true;
    }
}
