namespace DirectumRXAutoDeployer.Configuration
{
    public class DeploymentToolCoreSettings
    {
        public string ExePath { get; set; }
        public string ExeArguments { get; set; } = "-n {0} -p {1} -d {2} -x -s";
        public string Login { get; set; } = "Administrator";
        public string Password { get; set; } = "11111";
    }
}