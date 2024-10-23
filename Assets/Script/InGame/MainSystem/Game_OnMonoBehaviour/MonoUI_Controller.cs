namespace Coup_Mobile.InGame.GameManager.Ui
{
    public interface Display_UiController
    {
        public void StarterAndSetting(object packetData);
        public void CommandExecute(string target, object packetData);
        public object ReturnExecute(string target, object packetData);
    }
}