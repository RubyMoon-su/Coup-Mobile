using Photon.Pun;
using UnityEngine;
using System.Threading.Tasks;
using Coup_Mobile.InGame.GameManager.ReportData;
using UnityEngine.UI;

namespace Coup_Mobile.InGame.GameManager.GameState
{
    public class Check_PlayerInfo_GameState : GameState, IGameState_Strategy
    {
        public Check_PlayerInfo_GameState(GameStateManager gameStateManager) : base(gameStateManager)
        {
            Debug.Log($"{this.GetType().Name} Created.");
        }

        public async Task<object> ProcessGameState(object PacketData)
        {
            try
            {
                bool isOnline = GameManager.isOnline;

                if (isOnline)
                {
                    await OnProcess_Online();
                }
                else
                {
                    await OnProcess_Offline();
                }

                await Task.Delay(0);

                return new GameState_Report(GameState_List.Setup_Player_Properties, null, false, "Only Test Process PlayerSlot.");
            }
            catch (System.Exception ex)
            {
                return new GameState_Report(GameState_List.Setup_Player_Properties, null, false, ex.Message);
            }

        }

        #region Game Network Optional

        private async Task<bool> OnProcess_Online()
        {
            await Task.Delay(0);

            return true;
        }

        private async Task<bool> OnProcess_Offline()
        {
            await Task.Delay(0);
            return true;
        }

        #endregion

       
    }
}