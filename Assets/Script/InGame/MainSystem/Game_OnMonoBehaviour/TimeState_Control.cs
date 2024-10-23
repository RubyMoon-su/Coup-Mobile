using System;
using UnityEngine;
using UnityEngine.UI;
using Coup_Mobile.InGame.GameManager.Ui;

public class TimeState_Control : MonoBehaviour, Display_UiController
{
    #region Timer Field

    [Header("Component Ref")]
    [SerializeField] private Text timer_UI;

    [Header("Timer Request Interactive")]
    [SerializeField] private int requestTime = 20;
    [SerializeField] private int currentTime = 0;
    [SerializeField] private float elapsedTime = 0f;
    [SerializeField] private float timeSpeed_Value = 1f;

    [Header("DisPlay Setting")]
    [SerializeField] private int time_ChangeColor = 10;

    [Header("Status Timer")]
    [SerializeField] private bool isRuningTime;

    #endregion

    #region Process And Local Function

    // Update is called once per frame
    void Update()
    {
        if (isRuningTime) OnTimer_Process();

        //OnState_Process();
    }

    #region Event Control

    public void StarterAndSetting(object packetData)
    {
        // Timer Component.
        timer_UI = transform.Find("Timer").GetComponent<Text>();

        isRuningTime = false;
    }

    public void CommandExecute(string target, object packetData)
    {
        switch (target)
        {
            case "AddTimer":

                RequestTimer(packetData is int
                    ? (int)packetData
                    : throw new ArgumentException("RequestTimer PacketData is not int type."));

                break;
            case "ClearTimer":
                ClearTimer();
                break;
            case "StopTimer":
                StopTimer();
                break;
            case "ContineTimer": 
                ContineTimer();
                break;
            case "ResetTimer":
                Reset_Time();
                break;

            case "ChangeTimerSpeed":

                Change_TimeSpeed = packetData is float
                    ? (float)packetData
                    : throw new ArgumentException("ChangeTimerSpeed packetData is not float type.");

                break;
            case "ChangeDecideColor":

                Change_WarringColor = packetData is Color
                    ? (Color)packetData
                    : throw new ArgumentException("ChangeDecideColor packetData is not Color type.");

                break;
            case "ChangeDecideTimer":

                Change_WarringTime = packetData is int
                    ? (int)packetData
                    : throw new ArgumentException("ChangeDecideTimer packetData is not Color type.");

                break;
            default:
                throw new ArgumentException($"Unknown TimeState Control -> CommandExecute Target : {target}");
        }
    }

    public object ReturnExecute(string target, object packetData)
    {
        return target switch
        {
            "CurrentTime" => currentTime,
            "RemainingTime" => MathF.Abs(requestTime - currentTime),
            "RemainingTimeToRedZone" => MathF.Abs(time_ChangeColor - currentTime),
            "TotalTime" => requestTime,
            "TotalTimeWithoutRedZone" => MathF.Abs(requestTime - time_ChangeColor),
            //Add More Request In Here.
            _ => throw new ArgumentException($"Unknown TimeState Control -> ReturnExecute Target : {target}"),
        };
    }

    #endregion

    // Timer Zone
    private void OnTimer_Process()
    {
        if (isRuningTime && currentTime <= 0) Time_Out();

        elapsedTime += Time.deltaTime * timeSpeed_Value;

        if (elapsedTime >= 1)
        {
            OnChangeColor();

            currentTime -= Mathf.RoundToInt(elapsedTime);

            timer_UI.text = Convert.ToString(currentTime);

            elapsedTime = 0f;
        }
    }

    private void Time_Out()
    {
        // Stop Runing Time.
        isRuningTime = false;
        // Send event callback to Ui Manager.
    }

    private void OnChangeColor()
    {
        if (currentTime <= time_ChangeColor && requestTime > 0) timer_UI.color = Color.red;
        else timer_UI.color = new Color(0.93f, 0.90f, 0.80f, 1f);
    }

    #endregion


    #region Event Control

    private void RequestTimer(int RequestTime)
    {
        requestTime = RequestTime;

        Reset_Time();
    }

    private void ClearTimer()
    {
        isRuningTime = false;

        elapsedTime = 0f;
        currentTime = 0;
        requestTime = 0;

        OnChangeColor();

        timer_UI.text = "0";
    }

    private void ContineTimer()
    {
        isRuningTime = currentTime > 0;
    }

    private void StopTimer()
    {
        isRuningTime = false;
    }

    private void Reset_Time()
    {
        isRuningTime = false;

        elapsedTime = 0f;

        currentTime = requestTime;

        OnChangeColor();

        timer_UI.text = Convert.ToString(requestTime);

        isRuningTime = true;
    }

    #endregion

    #region  Property
    public float Change_TimeSpeed
    {
        set
        {
            float newTimeSpeed = value > 0
                ? value
                : 1f;

            timeSpeed_Value = newTimeSpeed;
        }
    }

    public int Change_WarringTime
    {
        set
        {
            int newWarringTime = value > 0
                ? value
                : 10  /*Defualt Value*/;


            time_ChangeColor = newWarringTime;
        }
    }

    public Color Change_WarringColor
    {
        set
        {
            Color newColor_Warring = value;
        }
    }

    #endregion
}
