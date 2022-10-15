using System.Dynamic;
using System.Net;
using DirectumRXAutoDeployer.Configuration;

namespace DirectumRXAutoDeployer.Notifiers
{
    public class AgileBoardSettings
    {
        public int BoardId { get; set; }
        public string IntegrationServiceUri { get; set; }
        public string ColumnFrom { get; set; }
        public string ColumnTo { get; set; }
        public string AppId { get; set; }
        

        public static AgileBoardSettings FromNotifierSetting(NotifierSettings settings)
        {
            return new AgileBoardSettings()
            {
                BoardId = settings.BoardId,
                IntegrationServiceUri = settings.IntegrationServiceUri,
                ColumnFrom = settings.ColumnFrom,
                ColumnTo = settings.ColumnTo,
                AppId = settings.AppId
            };
        }
    }
}