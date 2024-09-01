using System;

namespace Coup_Mobile.InGame.PlayerData
{
    [Serializable]
    public struct Player_Data
    {
        public string playerName;

        public string[] usedCharacter;
        public int usedCoin;

        public string GetPlayerName
        {
            get => playerName;
        }
    }
}