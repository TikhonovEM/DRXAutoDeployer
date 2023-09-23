using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectumRXAutoDeployer.Notifiers.Mattermost
{
    public class MattermostSettings : MessengerNotifierSettings
    {
        public string Server { get; set; }
        public string Token { get; set; }
        public string ChannelId { get; set; }
        public bool OneThread { get; set; }
    }
}
