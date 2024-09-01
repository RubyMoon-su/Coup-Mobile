using Photon.Pun;
using System.Threading.Tasks;
using Coup_Mobile.InGame.GameManager.ReportData;

namespace Coup_Mobile.InGame.GameManager.GameState
{
    public class Process_PlayerSort_GameState : GameState, IGameState_Strategy
    {
        public Process_PlayerSort_GameState(GameStateManager gameStateManager) : base(gameStateManager)
        {
            UnityEngine.Debug.Log($"{this.GetType().Name} Created.");
        }

        public async Task<object> ProcessGameState(object PacketData)
        {
            if (PacketData is GameManager_Data GM_Data)
            {
                try
                {
                    if (PhotonNetwork.IsMasterClient)
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            UnityEngine.Debug.Log($"Process_PlayerSort Count {i}");

                            await Task.Delay(1000);
                        }

                        UnityEngine.Debug.Log("Loop Complate");

                        await Task.Delay(5000);


                        return new GameState_Report(GameState_List.Setup_Player_Properties, null, true, null);
                    }
                    else
                    {
                        while (true)
                        {

                        }
                    }
                }
                catch (System.Exception ex)
                {
                    return new GameState_Report(GameState_List.Setup_Player_Properties, null, false, ex.Message);
                }
            }

            return new GameState_Report(GameState_List.Setup_Player_Properties, null, false, "DataType Incorrect.");
        }
    }
}