namespace DirectumRXAutoDeployer.Configuration
{
    public class GitRepositorySettings
    {
        public string ExePath { get; set; } = "git.exe";
        public string SourcesPath { get; set; }
        public string BranchName { get; set; } = "master";
    }
}