namespace DirectumRXAutoDeployer.Configuration
{
    public class DevelopmentStudioSettings
    {
        public string ExePath { get; set; }
        public string PackageInfoPath { get; set; }
        public string TempPackagesPath { get; set; }
        public bool SavePackageAfterDeploy { get; set; }
    }
}