using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectumRXAutoDeployer.Notifiers
{
    public abstract class MessengerNotifierSettings
    {
        public string StartMessage { get; set; }
        public string FinishMessage { get; set; }
        public string ErrorMessage { get; set; }

        public string StartPullFromGitMessage { get; set; }
        public string FinishPullFromGitMessage { get; set; }

        public string StartBuildPackageMessage { get; set; }
        public string FinishBuildPackageMessage { get; set; }

        public string StartDeployPackageMessage { get; set; }
        public string FinishDeployPackageMessage { get; set; }
    }
}
