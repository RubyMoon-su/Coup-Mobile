using System;
using Photon.Pun;
using System.Linq;
using UnityEngine;
using Photon.Realtime;
using Coup_Mobile.Menu;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;


public class Lobby_SlotControl : MonoBehaviourPun, ILobby_Observer
{
    #region Field Common

    [Header("Lobby Slot Pool Collection")]
    [SerializeField] private readonly List<Lobby_Slot> free_slot = new List<Lobby_Slot>();
    [SerializeField] private readonly List<Lobby_Slot> used_slot = new List<Lobby_Slot>();

    // Template Lobby And Tranparent.
    [Header("Lobby Slot Template And Parent Prefab")]
    [SerializeField] private GameObject slot_template;
    [SerializeField] private Transform tranparent_slot;

    // Toggle Ready.
    [Header("Lobby SlotControl Install Status")]
    [SerializeField] private bool install_complate = false;

    // Current Player Room.
    [Header("Current Player logic")]
    [SerializeField] private int current_player = 0;

    [Header("All Player Slot")]
    [SerializeField] private const int allSlotPlayer = 6; 

    // Enum Type.
    public enum Lobby_SlotCommand
    {
        Add_PlayerSlot, Remove_PlayerSlot
    }

    #endregion

    #region Starter Function And Ending Function.

    public void Awake()
    {
        Install_LobbySlot();
    }

    public void OnDestory()
    {

    }

    /// <summary>
    /// This Method is Create Player Slot With Pool System And Collection Slot in Global variable.
    /// </summary>
    private void Install_LobbySlot()
    {
        // Loop For Create Slot.
        for (int i = 0; i < allSlotPlayer; i++)
        {
            // Create New Slot With Prefub And tranparent.
            GameObject NewTemplate = Instantiate(slot_template, tranparent_slot);

            // Add New Component 'Lobby_Slot' In GameObject.
            Lobby_Slot NewSlot = NewTemplate.AddComponent<Lobby_Slot>();

            // Install Component And Ui Component In GameObject.
            NewSlot.InstallComponent();

            // Collection This Slot in Gobel Variable.
            free_slot.Add(NewSlot);

            // Set GameObject Is Not Show This Slot In Game.
            NewSlot.gameObject.SetActive(false);
        }

        install_complate = true;
    }

    #endregion

    #region Class Control


    public IEnumerator Lobby_Control()
    {
        int Timer = 0;

        // Wait for Create Player Slot And Wait for Add 'Ready' Player Properties.
        while (!install_complate || !Lobbys.GetInstall_Complate)
        {
            yield return new WaitForSeconds(1);

            Debug.LogError("Timer " + Timer);

            if (Timer == Lobbys.GetSystem_TimeOut)
            {
                Debug.LogError("Lobby_Control Timeout.");

                yield return null;
            }

            Timer++;
        }

        // Start Update Slot.
        StartCoroutine(Update_PlayerSlot());

    }

    #endregion

    #region Network Function

    private IEnumerator Update_PlayerSlot()
    {
        Dictionary<int, Player> CurrentRoom_Player = null;

        int Network_TimeOut = Lobbys.GetNetwork_TimeOut;

        // Check Player In Room And Update Player Current.
        for (int i = 0; i < Network_TimeOut; i++)
        {
            CurrentRoom_Player = PhotonNetwork.CurrentRoom.Players;

            if (CurrentRoom_Player.Count > 0 && CurrentRoom_Player.Count != current_player)
            {
                current_player = CurrentRoom_Player.Values.Count;
                break;
            }

            if (i == Network_TimeOut)
            {
                Debug.LogError("No Player Update Slot");
                yield break;
            }

            yield return new WaitForSeconds(1);
        }

        // Get All Player In Room For PhotonNetwork.
        List<Player> NewPlayer = CurrentRoom_Player.Values.ToList();

        // Clear Player Slot. 
        foreach (var Slot in used_slot.ToList())
        {
            Slot.RemovePlayerSlot();
            used_slot.Remove(Slot);
            free_slot.Add(Slot);
            Slot.gameObject.SetActive(false);
        }

        // Show All Player In Room.
        for (int i = NewPlayer.Count - 1; i >= 0; i--)
        {
            Player Slot = NewPlayer[i];

            bool Player_isHost = Slot.IsMasterClient;
            bool Player_isReady;

            // Try Get Value From Player Properties Ready.
            if (Slot.CustomProperties.TryGetValue("Ready", out Player_isReady))
            {
                Debug.Log("Player Have Value");
            }
            else
            {
                Debug.Log("Player Ready Is Null");
                Player_isReady = false;
            }

            if (free_slot.Count <= 0)
            {
                Debug.LogError("No Space for new player slot.");

                yield break;
            }

            // Add Player In Slot And Control Pool Slot.
            Lobby_Slot NewPlayer_Slot = free_slot[0];
            NewPlayer_Slot.gameObject.SetActive(true);
            used_slot.Add(NewPlayer_Slot);
            free_slot.Remove(NewPlayer_Slot);
            NewPlayer_Slot.AddPlayerSlot(Slot, Player_isReady, Player_isHost);

        }
    }

    public void Update_Notification(object PacketData)
    {
        StartCoroutine(Update_PlayerSlot());
    }


    #endregion
}
