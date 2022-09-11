using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using DirectumRXAutoDeployer.Configuration;
using DirectumRXAutoDeployer.Notifiers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DirectumRXAutoDeployer.Deploy
{
    public class Deployer : IDeployer
    {
        private readonly AppSettings _appSettings;
        private readonly ILogger<Deployer> _logger;
        private readonly IEnumerable<INotifier> _notifiers;
        private string _packagePath;

        public Deployer(AppSettings appSettings, ILogger<Deployer> logger, IEnumerable<INotifier> notifiers)
        {
            _appSettings = appSettings;
            _logger = logger;
            _notifiers = notifiers;
        }

        public async Task<bool> TryDeployAsync()
        {
            foreach (var notifier in _notifiers)
                await notifier.NotifyAboutStartAsync();

            try
            {
                PullUpdatesFromGit();
                BuildDevelopmentPackage();
                DeployDevelopmentPackage();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occured while deploying. ");
                foreach (var notifier in _notifiers)
                    await notifier.NotifyAboutErrorAsync(null);
                return false;
            }
            

            foreach (var notifier in _notifiers)
                await notifier.NotifyAboutFinishAsync();
            return true;
        }

        private void PullUpdatesFromGit()
        {
            _logger.LogInformation("PullUpdatesFromGit. Start");
            var gitSettings = _appSettings.GitRepositorySettings;
            ValidationHelper.ValidateGitSection(gitSettings, _logger);
            
            var gitPsi = new ProcessStartInfo
            {
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                FileName = string.IsNullOrWhiteSpace(gitSettings.ExePath) ? "git.exe" : gitSettings.ExePath
            };

            var sourcesPath = gitSettings.SourcesPath;
            var branch = string.IsNullOrWhiteSpace(gitSettings.BranchName) ? "master" : gitSettings.BranchName;
            ExecuteGitCommand(gitPsi, $"checkout {branch}", sourcesPath);
            ExecuteGitCommand(gitPsi, "pull", sourcesPath);
            _logger.LogInformation("PullUpdatesFromGit. Finish");
        }

        private void ExecuteGitCommand(ProcessStartInfo gitPsi, string command, string sourcesPath)
        {
            _logger.LogInformation($"ExecuteGitCommand '{command}'");
            var gitProcess = new Process();
            gitPsi.Arguments = command;
            gitPsi.WorkingDirectory = sourcesPath;

            gitProcess.StartInfo = gitPsi;
            gitProcess.Start();

            var stderr = gitProcess.StandardError.ReadToEnd();  // pick up STDERR
            var stdout = gitProcess.StandardOutput.ReadToEnd(); // pick up STDOUT

            if (!string.IsNullOrEmpty(stderr) && !stderr.StartsWith("Already on"))
                throw new Exception(stderr);
            
            if (!string.IsNullOrEmpty(stdout))
                _logger.LogInformation(stdout);

            gitProcess.WaitForExit();
            gitProcess.Close();
        }

        private void BuildDevelopmentPackage()
        {
            _logger.LogInformation("BuildDevelopmentPackage. Start");
            var ddsSettings = _appSettings.DevelopmentStudioSettings;
            ValidationHelper.ValidateDdsSection(ddsSettings, _logger);
            
            var tempPackagesPath = string.IsNullOrWhiteSpace(ddsSettings.TempPackagesPath) ? Path.GetTempPath() : ddsSettings.TempPackagesPath;
            _packagePath = Path.Combine(tempPackagesPath, $"DevelopmentPackage_{DateTime.Now:ddMMyy-HH-mm-ss}.dat");
            
            var ddsPsi = new ProcessStartInfo
            {
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                FileName = ddsSettings.ExePath,
                Arguments = $"-d {_packagePath} -c {ddsSettings.PackageInfoPath}"
            };

            var ddsProcess = new Process();
            ddsProcess.StartInfo = ddsPsi;
            ddsProcess.Start();

            var stderr = ddsProcess.StandardError.ReadToEnd();  // pick up STDERR
            var stdout = ddsProcess.StandardOutput.ReadToEnd(); // pick up STDOUT
            
            if (!string.IsNullOrEmpty(stderr))
                throw new Exception(stderr);
            
            if (!string.IsNullOrEmpty(stdout))
                _logger.LogInformation(stdout);

            ddsProcess.WaitForExit();
            ddsProcess.Close();
            
            _logger.LogInformation("BuildDevelopmentPackage. Finish");
        }
        
        private void DeployDevelopmentPackage()
        {
            _logger.LogInformation("DeployDevelopmentPackage. Start");
            
            var dtSection = _appSettings.DeploymentToolCoreSettings;
            ValidationHelper.ValidateDtSection(dtSection, _logger);

            var login = string.IsNullOrWhiteSpace(dtSection.Login) ? "Administrator" : dtSection.Login;
            var password = string.IsNullOrWhiteSpace(dtSection.Password) ? "11111" : dtSection.Password;
            var dtPsi = new ProcessStartInfo
            {
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                FileName = dtSection.ExePath,
                Arguments = $"-n {login} -p {password} -d {_packagePath} -x"
            };

            var dtProcess = new Process();
            dtProcess.StartInfo = dtPsi;
            dtProcess.Start();

            var stderr = dtProcess.StandardError.ReadToEnd();  // pick up STDERR
            var stdout = dtProcess.StandardOutput.ReadToEnd(); // pick up STDOUT
            
            if (!string.IsNullOrEmpty(stderr))
                throw new Exception(stderr);
            
            if (!string.IsNullOrEmpty(stdout))
                _logger.LogInformation(stdout);

            dtProcess.WaitForExit();
            dtProcess.Close();
            
            _logger.LogInformation("DeployDevelopmentPackage. Finish");
        }
    }
}