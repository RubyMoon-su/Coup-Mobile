using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TagPlayer : MonoBehaviour
{
    [SerializeField] private Text tag_PlayerName;
    [SerializeField] private Button selectPlayer;
    [SerializeField] private Image backgroud;

    #region Starter And Install Assist

    void Awake()
    {
        Install_Assist();

        Install_AssistOnParent();

        Install_EventButton();
    }

    private void Install_AssistOnParent()
    {
        Component[] components = gameObject.GetComponents<Component>();

        foreach (Component Tool in components)
        {
            switch (Tool.GetType().Name)
            {
                case "Button":
                    selectPlayer = Tool.GetComponent<Button>();
                    break;
                // Add More Component GameObject in here.
                default: Debug.LogWarning($"RagPlayer(Mono) -> Install AssistOnParent | Component : {Tool.GetType().Name} is not find a process in "); break;
            }
        }

        foreach (string ErrorMessage in CheckComponentOnParent_Installed())
        {
            Debug.LogError(ErrorMessage);
        }
    }

    private void Install_Assist()
    {
        int IndexChilds = transform.childCount;

        for (int i = 0; i < IndexChilds; i++)
        {
            GameObject Child = transform.GetChild(i).gameObject;

            switch (Child.name)
            {
                case "Text":
                    tag_PlayerName = Child.GetComponent<Text>();
                    break;
                case "Background":
                    backgroud = Child.GetComponent<Image>();
                    break;
                // Add More Component GameObject in here.
                default: Debug.LogWarning($"MessageBox Control(Mono) -> Install Assist | GameObject : {Child.name} is not find a process in "); break;
            }
        }

        foreach (string ErrorMessage in CheckComponent_Installed())
        {
            Debug.LogError(ErrorMessage);
        }
    }

    private string[] CheckComponent_Installed()
    {
        List<string> Component_Topic = new List<string>();
        Func<object, bool> CheckVariable_Installed = (_) => { return _ != null; };

        List<object> CheckInstalled_List = new List<object>
        {
            tag_PlayerName,
            backgroud,
        };

        foreach (var Status in CheckInstalled_List)
        {
            bool CheckInstalled = CheckVariable_Installed.Invoke(Status);

            if (!CheckInstalled)
            {
                string ErrorMessage = $"{Status.GetType().Name} is can't Install.";

                Component_Topic.Add(ErrorMessage);
            }
        }

        return Component_Topic.ToArray();
    }

    private string[] CheckComponentOnParent_Installed()
    {
        List<string> Component_Topic = new List<string>();
        Func<object, bool> CheckVariable_Installed = (_) => { return _ != null; };

        List<object> CheckInstalled_List = new List<object>
        {
            selectPlayer,
        };

        foreach (var Status in CheckInstalled_List)
        {
            bool CheckInstalled = CheckVariable_Installed.Invoke(Status);

            if (!CheckInstalled)
            {
                string ErrorMessage = $"{Status.GetType().Name} is can't Install.";

                Component_Topic.Add(ErrorMessage);
            }
        }

        return Component_Topic.ToArray();
    }

    private void Install_EventButton()
    {
        if (selectPlayer != null) selectPlayer.onClick.AddListener(() => OnClick_TagPlayer(this.gameObject.name));
        else Debug.LogError("Faill to Register Function");
    }

    #endregion

    #region OnClick Function

    private void OnClick_TagPlayer(string PlayerName)
    {
        Debug.LogError(PlayerName);
    }

    #endregion

    #region Setup Function

    public void SetupPlayerList(string PlayerName)
    {
        gameObject.name = PlayerName;

        tag_PlayerName.text = PlayerName;
    }

    #endregion
}
