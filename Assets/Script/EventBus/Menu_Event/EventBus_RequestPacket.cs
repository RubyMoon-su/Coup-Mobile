using Coup_Mobile.InGame.GameManager;
using Coup_Mobile.EventBus.ConvertType;
using Coup_Mobile.InGame.GameManager.ReportData;

namespace Coup_Mobile.EventBus.Packet_Request
{
    public static class EventBus_PacketRequest
    {
        public static object CreatePacket_Request_InGameManager(GameManager_Event Event , object EndPoint , object PacketData = null)
        {
            GameManager_Data Request_Data = new GameManager_Data
            {
                gameManager_Event  = Event,
                EndPoint = EndPoint,
                PacketData = PacketData != null ? PacketData : null,
            };

            object Return_Result = EventBus_InGameManager<IInGameEvent>.RaiseGameCommand(Request_Data);

            object Result_Converted = ConvertType_EventBus.ConvertType_PacketData(Return_Result);

            return Result_Converted;
        }

        
    }
}