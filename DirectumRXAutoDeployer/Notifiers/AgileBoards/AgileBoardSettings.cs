using System.Collections.Generic;

namespace DirectumRXAutoDeployer.Notifiers.AgileBoards
{
    public class AgileBoardSettings
    {
        public string Token { get; set; }
        public long BoardId { get; set; }
        public string IntegrationServiceUri { get; set; }
        public string AppId { get; set; }
        public string SummaryTarget { get; set; }
        public string TicketHyperlinkTemplate { get; set; }
        public List<ActionSetting> Actions { get; set; }
    }
}