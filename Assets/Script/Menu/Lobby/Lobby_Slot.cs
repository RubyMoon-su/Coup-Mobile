using System;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;

namespace Coup_Mobile.Menu
{
    [Serializable]
    public class Lobby_Slot : MonoBehaviour
    {
        #region  Common Field
        public Player playerinfo { get; set; }
        public string player_name {get; set;}
        public bool player_status { get; set; }
        public bool player_host {get; set;}

        #endregion

        #region Ref Component

        [SerializeField] private Text player_name_text;
        [SerializeField] private Image player_status_image;
        [SerializeField] private Text player_host_text;

        #endregion

        #region Starter Function 

        public void InstallComponent()
        {
            player_name_text = transform.Find("Player_Name").gameObject.GetComponent<Text>();
            player_status_image = transform.Find("Player_Status").gameObject.GetComponent<Image>();
            player_host_text = transform.Find("Host_Status").gameObject.GetComponent<Text>();
        }

        #endregion

        #region Control Slot Function

        public void AddPlayerSlot(Player PlayerInfo , bool Player_Status , bool Player_Host_Status)
        {
            // Player Information
            
            playerinfo = PlayerInfo;
            player_name = PlayerInfo.NickName;
            player_status = Player_Status;
            player_host = Player_Host_Status;

            // Display Information.

            player_name_text.text = player_name;
            player_status_image.enabled = player_status;
            player_host_text.text = player_host == true ? "Host" : "";
        }

        public void RemovePlayerSlot()
        {
            // Player Information.
            playerinfo = null;

            player_name = string.Empty;
            player_status = false;

            // Display Information

            player_name_text.text = "";
            player_status_image.enabled = false;
            player_host_text.text = "";
        }

        public void UpdateDisplay() 
        {
            player_name_text.text = player_name;
            player_status_image.enabled = player_status;
            player_host_text.text = player_host == true ? "Host" : "";
        }

        #endregion
    }
}