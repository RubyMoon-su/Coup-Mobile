using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MM_Menu1 : MonoBehaviour
{
    [SerializeField] private Button singleplayer_button;
    [SerializeField] private Button multiplayer_button;
    [SerializeField] private Button option_button;
    [SerializeField] private Button exitgame_button;
 
    public void SinglePlayer_Function()
    {
        Debug.Log("SinglePlayer Click");
    }

    public void MulitPlayer_Function()
    {
        Debug.Log("MulitPlayer Click");
    }

    public void Option_Finction()
    {
        Debug.Log("Option Click");
    }

    public void ExitGame_Function()
    {
        Debug.Log("ExitGame Click");

        Application.Quit();
    }
    
}
