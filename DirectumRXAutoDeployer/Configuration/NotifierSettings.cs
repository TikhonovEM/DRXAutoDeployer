namespace DirectumRXAutoDeployer.Configuration
{
    public class NotifierSettings
    {
        public string Target { get; set; }
        public string Token { get; set; }
        public string ChatIdentifier { get; set; }
        public string IntegrationServiceUri { get; set; }
        public string ColumnFrom { get; set; }
        public string ColumnTo { get; set; }
        public string AppId { get; set; }
        public int BoardId { get; set; }
    }
}