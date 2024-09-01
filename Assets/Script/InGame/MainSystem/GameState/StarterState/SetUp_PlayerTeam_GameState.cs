using System.Threading.Tasks;
using Coup_Mobile.InGame.GameManager.ReportData;

namespace Coup_Mobile.InGame.GameManager.GameState
{
    public class SetUp_PlayerTeam_GameState : GameState, IGameState_Strategy
    {
        public SetUp_PlayerTeam_GameState(GameStateManager gameStateManager) : base(gameStateManager)
        {
            UnityEngine.Debug.Log($"{this.GetType().Name} Created.");
        }

        public async Task<object> ProcessGameState(object PacketData)
        {
            try
            {
                for (int i = 0; i < 10; i++)
                {
                    UnityEngine.Debug.Log($"SetUpPlayerTeam Count {i}");

                    await Task.Delay(1000);
                }

                UnityEngine.Debug.Log("Loop Complate");

                await Task.Delay(5000);

                return new GameState_Report(GameState_List.Check_Component_And_GameEvent, null, true, null);
            }
            catch (System.Exception ex)
            {
                return new GameState_Report(GameState_List.Setup_Player_Properties, null, false, ex.Message);
            }
        }
    }
}