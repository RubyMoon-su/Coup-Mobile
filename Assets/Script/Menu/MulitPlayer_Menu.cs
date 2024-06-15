using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MulitPlayer_Menu : MonoBehaviour
{
    [HideInInspector] private Menu_Main mainmenu;

    #region  Start Component
    void Awake()
    {
        InstallComponent();
    } 

    private void InstallComponent()
    {
        mainmenu = GetComponentInParent<Menu_Main>();
    }

    #endregion

    public void CreateRoom_Function()
    {
        SceneManager.LoadScene("CreateRoom");
    }

    public void JoinRoom_Function()
    {
        SceneManager.LoadScene("FindRoom");
    }

    public void MatchMaking_Function()
    {
        SceneManager.LoadScene("MatchMaking");
    }

    public void BackToMainMenu_Function()
    {
        mainmenu.ChangeMenuScene("MainMenu");
    }
}
