using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerWave_Control : MonoBehaviour
{
    private Text playerWave_Name;

    public void Start()
    {
        playerWave_Name = transform.Find("PlayerWave").GetComponentInChildren<Text>();

        playerWave_Name.text = "--";
        playerWave_Name.color = new Color(0.93f , 0.9f , 0.8f , 1f);
    }

    public void ChangePlayerWave(string playerWave , bool isMasterWave)
    {
        playerWave_Name.text = playerWave;

        if (isMasterWave) playerWave_Name.color = Color.green;
        else playerWave_Name.color = new Color(0.93f , 0.9f , 0.8f , 1f);;
    }
}
