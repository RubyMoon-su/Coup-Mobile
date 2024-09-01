using System;
using UnityEngine;
using System.Collections.Generic;

public interface IOtherPlayerResponsive_Subject
{
    void Attach(IOtherPlayerResponsive_Oberver observer);
    void Detach(IOtherPlayerResponsive_Oberver observer);
    void Notify(object PacketData);
}

public interface IOtherPlayerResponsive_Oberver
{
    void UpdateFormNetwork(object PacketData);
}


public interface INetworkResponsive 
{
    public void SettingDataToDefault();
}

[Serializable]
public struct OtherPlayerResponsive_StarterState : INetworkResponsive
{

    // Result Responsive Form Host Player.
    [SerializeField] private bool onContineState;
    
    // Release Setting Character And Card all player but not only Host Player. 
    [SerializeField] private string[] character_Setting;
   
    [SerializeField] private int coin_Setting;
    
    // Release Setting Player Team.
    [SerializeField] private int playerTeam;
    

    public void SettingDataToDefault()
    {
        onContineState = false;
        character_Setting = new string[2];
        coin_Setting = 0;
    }

    public bool GetSetOnContineState
    {
        get => onContineState;
        set => onContineState = value;
    }

    public string[] GetSetCharacter_Setting
    {
        get => character_Setting;
        set => character_Setting = value;
    }

    public int GetSetCoin_Setting
    {
        get => coin_Setting;
        set => coin_Setting = value;
    }

    public int GetSetPlayerTeam
    {
        get => playerTeam;
        set => playerTeam = value;
    }

    
}

[Serializable]
public struct OtherPlayerResponsive_MainState : INetworkResponsive
{
    // Contine state.
    private bool onContineState;

    // SendAction Data.
    private bool sendAction_Requestment;
    private string sendAction_type;
    private string sendAction_Target;
    private string sendAction_ower;

    // CounterAction Data.
    private bool counterAction_Countered;
    private string counterAction_Type;
    private string counterAction_Ower;

    // VerifyAction Data.
    private bool verifyaction_check;

    // ResultAction Data.
    private bool punishments_Status;
    private int resultAction_LoseCoin;
    private int resultAction_LoseCharacter;

    // Coming Soon More.

    public void SettingDataToDefault()
    {
        onContineState = false;
        sendAction_type = string.Empty;
        sendAction_type = string.Empty;
        sendAction_Target = string.Empty;
    }

    public bool GetSetOnContineState
    {
        get => onContineState;
        set => onContineState = value;
    }

    #region SendAction Variable.

    public bool GetSetSendAction_Requestment
    {
        get => sendAction_Requestment;
        set => sendAction_Requestment = value;
    }

    public string GetSetAction_Type
    {
        get => sendAction_type;
        set => sendAction_type = value;
    }

    public string GetSetAction_Ower
    {
        get => sendAction_ower;
        set => sendAction_ower = value;
    }

    public string GetSetAction_Target
    {
        get => sendAction_Target;
        set => sendAction_Target = value;
    }

    #endregion

    #region CounterAction Variable.

    public string GetSetCounterAction_Type
    {
        get => counterAction_Type;
        set => counterAction_Type = value;
    }

    public string GetSetCounterAction_Ower
    {
        get => counterAction_Ower;
        set => counterAction_Ower = value;
    }

    public bool GetSetCounterAction_Countered
    {
        get => counterAction_Countered;
        set => counterAction_Countered = value;
    }

    #endregion

    #region VerifyAction Variable.

    public bool GetSetVerifyAction_Check
    {
        get => verifyaction_check;
        set => verifyaction_check = value;
    }
    
    #endregion

    #region  ResultAction Variable.

    public bool GetSetPunishments_Status
    {
        get => punishments_Status;
        set => punishments_Status = value;
    }

    public int GetSetResultAction_LoseCoin
    {
        get => resultAction_LoseCoin;
        set => resultAction_LoseCoin = value;
    }

    public int GetSetResultAction_LoseCharacter
    {
        get => resultAction_LoseCharacter;
        set => resultAction_LoseCharacter = value;
    }

    #endregion
}

