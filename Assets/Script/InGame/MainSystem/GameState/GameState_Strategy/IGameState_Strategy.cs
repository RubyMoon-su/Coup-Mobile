using System.Threading.Tasks;

public interface IGameState_Strategy
{
    Task<object> ProcessGameState(object PacketData);
}