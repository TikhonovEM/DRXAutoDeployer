using System.Dynamic;
using System.Net;
using DirectumRXAutoDeployer.Configuration;

namespace DirectumRXAutoDeployer.Notifiers
{
    public class AgileBoardSettings
    {
        public string Token { get; set; }
        public int BoardId { get; set; }
        public string IntegrationServiceUri { get; set; }
        public string ColumnFrom { get; set; }
        public string ColumnTo { get; set; }
        public string AppId { get; set; }
    }
}