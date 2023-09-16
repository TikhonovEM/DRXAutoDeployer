using System.Collections.Generic;

namespace DirectumRXAutoDeployer.Configuration
{
    public class GitRepositoriesSettings
    {
        public string ExePath { get; set; } = "git.exe";

        public List<RepositorySetting> Repositories { get; set; }
    }
}