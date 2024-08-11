using Coup_Mobile.InGame.GameManager;

namespace Coup_Mobile.EventBus.ConvertType
{
    public static class ConvertType_EventBus
    {
        public static object ConvertType_PacketData(object TypeConvert) 
        {
            if (TypeConvert is GameAssistManager_Return gameAssistManager)
            {
                return gameAssistManager.return_Data;
            }
            else if (TypeConvert is GameUIManager_Return gameUIManager)
            {
                return gameUIManager.return_Data;
            }
            else if (TypeConvert is GameNetworkManager_Return gameNetworkManager)
            {
                return gameNetworkManager.return_Data;
            }
            else if (TypeConvert is GameResource_Result gameResourceManager)
            {   
                return gameResourceManager.return_Data;
            }
            else if (TypeConvert is GameStateManager_Return gameStateManager)
            {
                return gameStateManager.return_Data;
            }
            else 
            {
                UnityEngine.Debug.LogError($"'{TypeConvert.GetType().Name}' is Not Support to convert This Type Result.");
                throw new System.Exception();
            }
        }
    }
}