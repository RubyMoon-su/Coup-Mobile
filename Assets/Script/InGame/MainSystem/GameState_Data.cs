using System;
using Coup_Mobile.EventBus;

namespace Coup_Mobile.InGame.GameManager.ReportData
{
    [Serializable]
    public struct GameState_Report : IInGameEvent
    {
        private GameState_List reportState;
        private object resultReport;
        private bool resultStatus;
        private string reportString;

        public GameState_Report(GameState_List ReportState , object ResultReport , bool ResultStatus , string ReportString)
        {
            reportState = ReportState;
            resultReport = ResultReport;
            resultStatus = ResultStatus;
            reportString = ReportString;
        }

        public GameState_List GetReportTopic
        {
            get => reportState;
        }

        public bool GetReportStatus
        {
            get => resultStatus;
        }

        public object GetResultReport
        {
            get => resultReport;
        }

        public string GetReportString
        {
            get => reportString;
        }
    }

    [Serializable]
    public struct GameManager_Data : IInGameEvent
    {
        public GameManager_Event gameManager_Event {get; set;}
        public object EndPoint {get; set;}
        public object PacketData {get; set;}
    }
}