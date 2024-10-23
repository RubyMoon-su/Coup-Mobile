using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MessageBoxControl : MonoBehaviour
{
    [SerializeField] private Text senderText;

    #region Starter And Install Assist

    void Awake()
    {
        Install_Assist();
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
                    senderText = Child.GetComponent<Text>();
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
            senderText,
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

    #endregion

    #region SetUp Function

    /// <summary>
    /// </summary>
    /// <param name="sender">Name of Sender</param>
    /// <param name="message">Message to Show</param>
    /// <param name="TypeMessage"></param>
    public void AddMessage(string sender, string message, int TypeMessage = 0)
    {
        string Hightlight_Sender = "#ffffff";
        string Hightlight_Message = "ffffff";

        if (sender == "System") Hightlight_Sender = "#004dff";
        else if (sender == "Cient") Hightlight_Sender = "#ff0000";

        switch (TypeMessage)
        {
            case 0 /*"Noraml"*/:
                Hightlight_Message = "#ffffff";
                break;
            case 1 /*"Warning"*/:
                Hightlight_Message = "#ffff00";
                break;
            case 2 /*"Error"*/:
                Hightlight_Message = "#ff0000";
                break;
            default : Debug.LogWarning("MessageBoxControl -> Add Message | TypeMessage Is not Set."); return;
        }

        string messageNotify = !string.IsNullOrWhiteSpace(message) ? message : "Unknow";

        senderText.text = !string.IsNullOrWhiteSpace(sender) ? $"{DateTime.Now.ToString("hh:mm tt")} <color={Hightlight_Sender}>{sender}</color> : <color={Hightlight_Message}>{messageNotify}</color> " : "Unknown";
    }

    #endregion
}
