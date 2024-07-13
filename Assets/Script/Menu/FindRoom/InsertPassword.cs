using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

namespace Coup_Mobile.Menu
{
    public class InsertPassword : MonoBehaviourPun
    {
        [HideInInspector] private RoomInfo room_data { get; set; }

        [SerializeField] private InputField input_password;

        public void StartForm(RoomInfo Roomdata)
        {
            input_password.text = string.Empty;
            room_data = null;

            room_data = Roomdata;
        }

        public void Confirm_Function()
        {
            if (input_password.text.Length <= 0)
            {
                gameObject.SetActive(false);
            }

            if ((bool)room_data.CustomProperties["Room Status"] == true)
            {
                if ((int)room_data.CustomProperties["Password"] == Convert.ToInt32(input_password.text))
                {
                    PhotonNetwork.JoinRoom(room_data.Name);
                }
                else
                {
                    gameObject.SetActive(false);
                }
            }
        }

    }
}