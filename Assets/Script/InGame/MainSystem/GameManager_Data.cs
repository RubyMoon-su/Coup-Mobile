using System;
using System.Collections.Generic;
using Coup_Mobile.InGame.GameManager;
using Coup_Mobile.InGame.PlayerData;

namespace Coup_Mobile.Menu.GameSetting_Data
{
    [Serializable]
    public struct GameSetting
    {
        public GameMode gameMode;
        public List<Player_Data> allPlayerData;
        public int allPlayerInGame;
        public CharacterSetting characterSetting;
        public PropertiesSetting propertiesSetting;
        public AdvanceSetting advanceSetting;

    }

    [Serializable]
    public struct CharacterSetting
    {
        public int duke , assassins , captain , ambassdor , contassa , inquisitor;

        public void SetDefalult_Setting()
        {
            duke = 3;
            assassins = 3;
            captain = 3;
            ambassdor = 3;
            contassa = 3;
            inquisitor = 15;
        }
    }

    [Serializable]
    public struct PropertiesSetting
    {
        public int strarterCoin , starterCard , starter_TreasuryReserve;
        public int totalCoin , totalCard , maxnium_TreasuryReserve;

        // Game Resource Cost
        public int duke_Cost , assassin_Cost , captain_Cost , ambassin_Cost , contassa_Cost , inquisitor_Cost , income_Cost , forgetaid_Cost , coup_Cost;

        // Game Resource Income
        public int duke_Income , assassin_Income , captain_Income , ambassdor_Income , contassa_Income , inquisitor_Income , income , forgetaid_Income , coup_Income;

        // Ability Can Use.
        public List<string> allAbility_CanUse;

        public void SetDefalult_Setting()
        {
            strarterCoin = 2;
            starterCard = 2;
            starter_TreasuryReserve = 0;

            totalCoin = 50;
            totalCard = 15;
            maxnium_TreasuryReserve = 50;

            // Game Resource Cost For Use Abilty.

            income_Cost = 0;
            forgetaid_Cost = 0;
            coup_Cost = 7;
            duke_Cost = 0;
            assassin_Cost = 3;
            captain_Cost = 0;
            ambassin_Cost = 0;
            contassa_Cost = 0;
            inquisitor_Cost = 0;
            // Game Resource Income For Use Abily Effect.

            income = 1;
            forgetaid_Income = 2;
            coup_Income = 0;
            duke_Income = 3;
            assassin_Income = 0;
            captain_Income = 2;
            ambassdor_Income = 0;
            contassa_Income = 0;
            inquisitor_Income = 0;

            allAbility_CanUse = new List<string>
            {
                "Income","Forget Aid","Coup","Duke","Assassin","Captain","Ambassdor","Contassa"
            };
        }
    }

    [Serializable]
    public struct AdvanceSetting
    {
        #region Time Response

        public int waitCommandTime;
        public int waitCounterTime;
        public int waitVerifyTime;
        public int waitResultTime;
        public int waitChangeTeamTime;
        public int waitConversionTime;
        public int waitEmbezzlementTime;
        public int waitBettwenState;
        
        #endregion

        public void SetDefalult_Setting()
        {
            waitCommandTime = 10;
            waitCounterTime = 4;
            waitVerifyTime = 4;
            waitResultTime = 5;
            waitChangeTeamTime = 10;
            waitConversionTime = 10;
            waitEmbezzlementTime = 4;
            waitBettwenState = 3;

        }

    }
}