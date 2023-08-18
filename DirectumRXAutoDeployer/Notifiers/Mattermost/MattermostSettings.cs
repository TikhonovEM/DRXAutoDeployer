using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectumRXAutoDeployer.Notifiers.Mattermost
{
    public class MattermostSettings
    {
        public string Server { get; set; }
        public string Token { get; set; }
        public string ChannelId { get; set; }
        public string StartMessage { get; set; }
        public string FinishMessage { get; set; }
        public string ErrorMessage { get; set; }
    }
}
