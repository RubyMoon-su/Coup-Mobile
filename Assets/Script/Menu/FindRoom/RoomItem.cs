using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using System.Collections;
using System.Linq.Expressions;
using System.Threading;

namespace Coup_Mobile.Menu
{
    public class RoomItem : MonoBehaviourPun
    {
        #region Properpty RoomInfo
        [HideInInspector] public string room_name { get; set; }
        [HideInInspector] public bool room_status { get; set; }
        [HideInInspector] public int players_count_in { get; set; }
        [HideInInspector] public int max_players { get; set; }

        [HideInInspector] public RoomInfo room_info { get; set; }
        [HideInInspector] public InsertPassword insert_form {get; set;}

        #endregion

        #region ShowInfo

        [HideInInspector] private Text room_name_text;
        [HideInInspector] private Text players_count_in_text;
        [HideInInspector] private Text max_players_text;
        [HideInInspector] private Image image_logo_room;
        [HideInInspector] private Image image_status_room;
        [HideInInspector] private Button button_enter_button;

        #endregion

        private void InstallComponent()
        {
            room_name_text = transform.Find("RoomName").gameObject.GetComponent<Text>();
            players_count_in_text = transform.Find("CountNumber").gameObject.GetComponent<Text>();
            max_players_text = transform.Find("MaxNumber").gameObject.GetComponent<Text>();
            image_logo_room = transform.Find("LogoRoom").gameObject.GetComponent<Image>();
            image_status_room = transform.Find("RoomStatus").gameObject.GetComponent<Image>();
            button_enter_button = transform.Find("RoomButton").gameObject.GetComponent<Button>();

            button_enter_button.onClick.AddListener(EnterRoom);
        }

        public void AddRoomInfo(RoomInfo Room_info)
        {
            InstallComponent();

            room_info = Room_info;

            room_name = room_info.Name;
            room_status = (bool)room_info.CustomProperties["Room Status"];
            players_count_in = room_info.PlayerCount;
            max_players = room_info.MaxPlayers;

            ShowRoomInfo();
        }

        public void EditRoomInfo(RoomInfo roominfo)
        {
            room_name = room_info.Name;
            room_status = (bool)room_info.CustomProperties["Room Status"];
            players_count_in = room_info.PlayerCount;
            max_players = room_info.MaxPlayers;

            ShowRoomInfo();
        }

        private void ShowRoomInfo()
        {
            room_name_text.text = room_name;
            players_count_in_text.text = players_count_in.ToString();
            max_players_text.text = max_players.ToString();
        }

        public void EnterRoom()
        {
            if (room_info == null)
            {
                Debug.LogError("RoomInfo Data is null");
            }

            Debug.Log($"Click Room {room_name}");

            if (room_status == true)
            {
                insert_form.gameObject.SetActive(true);

                insert_form.StartForm(room_info);
            }
            else 
            {
                PhotonNetwork.JoinRoom(room_name);
            }
        }

        public void RemoveRoomItem()
        {
            room_name = string.Empty;
            room_status = false;
            players_count_in = 0;
            max_players = 0;

            room_info = null;
        }

    }
}