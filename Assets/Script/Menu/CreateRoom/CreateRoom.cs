using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;

public class CreateRoom : MonoBehaviour
{
    [SerializeField] private InputField inputname;
    [SerializeField] private Button enter_button;
    [SerializeField] private Text text_exception;

    public void ChangePlayername()
    {
        string Playername = inputname.text;
        Regex regex = new Regex("^[a-zA-Z0-9]*$");


        if (Playername.Length > 0 && !string.IsNullOrEmpty(Playername) && !string.IsNullOrWhiteSpace(Playername))
        {
            if (regex.IsMatch(Playername))
            {
                text_exception.text = "";

                enter_button.interactable = true;
            }
            else 
            {
                text_exception.text = "Do not enter special characters.";

                enter_button.interactable = false;
            }
        }
        else 
        {
            text_exception.text = "Please enter name";

            enter_button.interactable = false;
        }
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("Mainmenu");
    }
}
