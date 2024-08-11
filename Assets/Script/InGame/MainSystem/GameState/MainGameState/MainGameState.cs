using System.Threading.Tasks;

namespace Coup_Mobile.InGame.GameManager.GameState
{
    #region Normal Game State

    /// <summary>
    /// This Class Is First MainGame State, if Game Mode Selected 'Reformation' First MainGame State is player Conversion Command.
    /// This Class Selected First Player And Wait For Selected Player Send Something Command With Other Player Or For MySelf. 
    /// The system will continue to choose, circling every player in the game.
    /// Player Selection By System Can Send Command With Other Player Or For MySelf , Other Player Not Selected By System Can't Send Command Or Not Do anyting.
    /// Player Not Selectioned By System , Can Counter Or InteractiveActive With Player Selection By System On 'Player_CounterCommand' GameState.
    /// </summary>
    public class Player_SendCommand_GameState : GameState , IGameState_Strategy
    {
        public Player_SendCommand_GameState(GameStateManager gameStateManager) : base(gameStateManager)
        {
            UnityEngine.Debug.Log($"{this.GetType().Name} Created.");
        }

        public async Task<object> ProcessGameState(object PacketData)
        {
            await Task.Delay(0);

            return (object)new object();
        }
    }

    /// <summary>
    /// This Class Is Second MainGame State , if Game Mod Selected 'Reformation' Second MainGame State is Player Conversion Result Command.
    /// Player not Selected By System , Can Counter Player Send Command.
    /// GameMode 'Reformation' , Player Can Counter Player Send Command , if Player Selected Not Same Team or if All Player stay Same Team.
    /// </summary>
    public class Player_CounterCommand_GameState : GameState , IGameState_Strategy
    {
        public Player_CounterCommand_GameState(GameStateManager gameStateManager) : base(gameStateManager)
        {
            UnityEngine.Debug.Log($"{this.GetType().Name} Created.");
        }

        public async Task<object> ProcessGameState(object PacketData)
        {
            await Task.Delay(0);

            throw new System.NotImplementedException();
        }
    }

    public class Player_VerifyCommand_GameState : GameState , IGameState_Strategy
    {
        public Player_VerifyCommand_GameState(GameStateManager gameStateManager) : base(gameStateManager)
        {
            UnityEngine.Debug.Log($"{this.GetType().Name} Created.");
        }

        public async Task<object> ProcessGameState(object PacketData)
        {
            await Task.Delay(0);

            throw new System.NotImplementedException();
        }
    }

    public class Player_ResultCommand_GameState : GameState , IGameState_Strategy
    {
        public Player_ResultCommand_GameState(GameStateManager gameStateManager) : base(gameStateManager)
        {
            UnityEngine.Debug.Log($"{this.GetType().Name} Created.");
        }

        public async Task<object> ProcessGameState(object PacketData)
        {
            await Task.Delay(0);

            throw new System.NotImplementedException();
        }
    }

    public class WaitForAllPlayer_GameState : GameState , IGameState_Strategy
    {
        public WaitForAllPlayer_GameState(GameStateManager gameStateManager) : base(gameStateManager)
        {
            UnityEngine.Debug.Log($"{this.GetType().Name} Created.");
        }

        public async Task<object> ProcessGameState(object PacketData)
        {
            await Task.Delay(0);

            throw new System.NotImplementedException();
        }
    }

    #endregion

    #region Reformation GameState

    public class Player_ConversionCommand_GameState : GameState , IGameState_Strategy
    {
        public Player_ConversionCommand_GameState(GameStateManager gameStateManager) : base(gameStateManager)
        {
            UnityEngine.Debug.Log($"{this.GetType().Name} Created.");
        }

        public async Task<object> ProcessGameState(object PacketData)
        {
            await Task.Delay(0);

            throw new System.NotImplementedException();
        }
    }

    public class Player_ConversionResult_Command_GameState : GameState , IGameState_Strategy
    {
        public Player_ConversionResult_Command_GameState(GameStateManager gameStateManager) : base(gameStateManager)
        {
            UnityEngine.Debug.Log($"{this.GetType().Name} Created.");
        }

        public async Task<object> ProcessGameState(object PacketData)
        {
            await Task.Delay(0);

            throw new System.NotImplementedException();
        }
    }

    public class Player_EmbezzlementCommand_GameState : GameState , IGameState_Strategy
    {
        public Player_EmbezzlementCommand_GameState(GameStateManager gameStateManager) : base(gameStateManager)
        {
            UnityEngine.Debug.Log($"{this.GetType().Name} Created.");
        }

        public async Task<object> ProcessGameState(object PacketData)
        {
            await Task.Delay(0);

            throw new System.NotImplementedException();
        }
    }

    public class Player_EmbezzlementResult_Command_GameState : GameState , IGameState_Strategy
    {
        public Player_EmbezzlementResult_Command_GameState(GameStateManager gameStateManager) : base(gameStateManager)
        {
            UnityEngine.Debug.Log($"{this.GetType().Name} Created.");
        }

        public async Task<object> ProcessGameState(object PacketData)
        {
            await Task.Delay(0);
            
            throw new System.NotImplementedException();
        }
    }

    #endregion
}