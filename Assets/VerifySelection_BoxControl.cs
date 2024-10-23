using System;
using UnityEngine;
using UnityEngine.UI;
using Coup_Mobile.InGame.GameManager.Ui;

public class VerifySelection_BoxControl : MonoBehaviour, Display_UiController
{
    private bool isUserReponse = false;
    private int reponse_Result = 0;

    // Conponent Field.
    private Button confirmButton;
    private Button DiscardButton;
    private Text topic;
    private Text description;


    public void StarterAndSetting(object packetData)
    {
        VerifyBox_Data verifyBox = packetData is VerifyBox_Data 
            ? (VerifyBox_Data)packetData 
            : throw new ArgumentException("");

        LoadingAssist();
        SetupDisplay_Box(verifyBox);
    }
    public void CommandExecute(string target, object packetData)
    {
        throw new System.NotImplementedException();
    }

    public object ReturnExecute(string target, object packetData)
    {
        return target switch
        {
            "GetIsReponse" => isUserReponse,
            "GetReponseResult" => reponse_Result,
            _ => throw new ArgumentException(""),
        };
    }

    private void LoadingAssist()
    {
        topic = transform.Find("Topic").GetComponent<Text>();
        description = transform.Find("Ability_Select").GetComponent<Text>();
        confirmButton = transform.Find("Aceept_button").GetComponent<Button>();
        DiscardButton = transform.Find("Cancel_Button").GetComponent<Button>();
    }

    private void SetupDisplay_Box(VerifyBox_Data verifyBox_Data)
    {
        topic.text = verifyBox_Data.topic;
        description.text = verifyBox_Data.description;
        confirmButton.onClick.AddListener(() => Reponse_Input(1));
        DiscardButton.onClick.AddListener(() => Reponse_Input(2));


        Text confirmText = confirmButton.transform.GetComponentInChildren<Text>();
        Text discardText = DiscardButton.transform.GetComponentInChildren<Text>();
        confirmText.text = verifyBox_Data.fristButton_Name;
        discardText.text = verifyBox_Data.SecondButton_Name;
    }

    private void Reponse_Input(int result)
    {
        reponse_Result = result;
        isUserReponse = true;
    }
}

public struct VerifyBox_Data
{
    public string topic;
    public string description;
    public string fristButton_Name;
    public string SecondButton_Name;

    public VerifyBox_Data(string topic , string description , string fristButton_Name = "Yes" , string SecondButton_Name = "No")
    {
        this.topic = topic;
        this.description = description;
        this.fristButton_Name = fristButton_Name;
        this.SecondButton_Name = SecondButton_Name;
    }
}
