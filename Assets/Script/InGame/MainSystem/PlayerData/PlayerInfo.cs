using System;

namespace Coup_Mobile.InGame.PlayerData
{
    [Serializable]
    public struct Player_Data
    {
        public string playerName;

        public string GetPlayerName
        {
            get => playerName;
        }
    }
}