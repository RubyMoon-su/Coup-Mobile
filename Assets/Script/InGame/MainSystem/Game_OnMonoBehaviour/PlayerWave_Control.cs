using System;
using UnityEngine;
using UnityEngine.UI;
using Coup_Mobile.InGame.GameManager.Ui;

public class PlayerWave_Control : MonoBehaviour, Display_UiController
{
    private Text playerWave_Name;

    private string CurrentPlayerName;


    #region Event Control

    public void StarterAndSetting(object packetData)
    {
        playerWave_Name = transform.Find("PlayerWave").GetComponentInChildren<Text>();

        playerWave_Name.text = "--";
        playerWave_Name.color = new Color(0.93f, 0.9f, 0.8f, 1f);
    }

    public void CommandExecute(string target, object packetData)
    {
        switch (target)
        {
            // Variable Control.
            case "SetPlayer Display":

                var NewPlayerName_Data = CheckType<object[]>(packetData) 
                    ? (object[])packetData 
                    : throw new Exception($"PlayerWave_Control -> CommandExecute | SetPlayer_Display PacketData is not Dictionary type.");

                string NewPlayerName_Wave = (string)NewPlayerName_Data[0];
                bool isMasterWave = (bool)NewPlayerName_Data[1];

                ChangePlayerWave(NewPlayerName_Wave , isMasterWave);
                
                break;
            default: throw new ArgumentException($"PlayerWave_Control -> CommandExecute | Unknown Topic : {target}");
        }
    }

    public object ReturnExecute(string target, object packetData)
    {
        return target switch
        {
            "CurrentPlayerName" => CurrentPlayerName,
            _ => throw new ArgumentException($"PlayerWave_Control -> ReturnExecute | Unknown Topic : {target}"),
        };
    }

    #endregion

    #region  Local Function

    public void ChangePlayerWave(string playerWave, bool isMasterWave)
    {
        playerWave_Name.text = playerWave;

        if (isMasterWave) playerWave_Name.color = Color.green;
        else playerWave_Name.color = new Color(0.93f, 0.9f, 0.8f, 1f); ;
    }

    private bool CheckType<T>(object packetData) => packetData.GetType() == typeof(T);

    #endregion
}
