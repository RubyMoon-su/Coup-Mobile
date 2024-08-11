using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class TimeState_Control : MonoBehaviour
{
    #region Timer Field

    [Header("Component Ref")]
    [SerializeField] private Text timer_UI;

    [Header("Timer Request Interactive")]
    [SerializeField][Tooltip("This Variable is Default Timer")] private int requestTime = 20;
    [SerializeField][Tooltip("This Variable is Main Timer")] private int currentTime = 0;
    [SerializeField] private float elapsedTime = 0f;

    [Header("DisPlay Setting")]
    [SerializeField] private int time_ChangeColor = 10;

    [Header("Status Timer")]
    [SerializeField] private bool isRuningTime;

    #endregion

    #region Process And Local Function

    // Start is called before the first frame update
    void Start()
    {
        // Timer Component.
        timer_UI = transform.Find("TimerDisplay/Timer").GetComponent<Text>();

        isRuningTime = false;
    }

    // Update is called once per frame
    void Update()
    {
        OnTimer_Process();

        //OnState_Process();
    }

    // Timer Zone
    private void OnTimer_Process()
    {
        if (!isRuningTime) return;

        if (isRuningTime && currentTime <= 0) Time_Out();

        elapsedTime += Time.deltaTime;

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
        if (currentTime <= time_ChangeColor) timer_UI.color = Color.red;
        else timer_UI.color = new Color(0.93f, 0.90f, 0.80f, 1f);
    }

    #endregion


    #region Request OutSide Timer

    public void RequestTimer(int RequestTime)
    {
        requestTime = RequestTime;

        Reset_Time();
    }

    public void Reset_Time()
    {
        isRuningTime = false;

        elapsedTime = 0f;

        currentTime = requestTime;

        OnChangeColor();

        timer_UI.text = Convert.ToString(requestTime);

        isRuningTime = true;
    }

    public void Setting_StateInfo()
    {

    }

    #endregion
}
