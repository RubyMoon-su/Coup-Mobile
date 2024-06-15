using System;
using UnityEngine;
using Photon.Pun;

public class NetworkController : MonoBehaviourPunCallbacks
{
    private NetworkController instance;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
    }


    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();

        if (PhotonNetwork.IsConnected)
        {
            Debug.Log("Connection");
        }
        else
        {
            Debug.Log("Not Connection");
        }
    }

#region  Connection Method
    public override void OnConnected()
    {
        
    }

    public void OnDisconnected()
    {

    }

#endregion
}
