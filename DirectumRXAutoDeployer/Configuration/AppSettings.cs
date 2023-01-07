using System.Collections.Generic;

namespace DirectumRXAutoDeployer.Configuration
{
    public class AppSettings
    {
        public DevelopmentStudioSettings DevelopmentStudioSettings { get; set; }
        public GitRepositorySettings GitRepositorySettings { get; set; }
        public DeploymentToolCoreSettings DeploymentToolCoreSettings { get; set; }
    }
}