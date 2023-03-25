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
            var gitSettings = _appSettings.GitRepositoriesSettings;
            ValidationHelper.ValidateGitSection(gitSettings, _logger);

            var gitPsi = new ProcessStartInfo
            {
                CreateNoWindow = true,
                FileName = gitSettings.ExePath
            };

            foreach (var repository in gitSettings.Repositories)
            {
                var sourcesPath = repository.SourcesPath;
                var branch = repository.BranchName;
                ExecuteGitCommand(gitPsi, "reset --hard", sourcesPath);
                ExecuteGitCommand(gitPsi, $"checkout {branch}", sourcesPath);
                ExecuteGitCommand(gitPsi, "pull", sourcesPath);
                ExecuteGitCommand(gitPsi, "clean -x -d -f", sourcesPath);
            }


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

            if (!gitProcess.WaitForExit(30000))
            {
                gitProcess.Kill();
                throw new TimeoutException($"Git command {command} exceeded timeout. Deployment aborted.");
            }

            gitProcess?.Dispose();
        }

        private void BuildDevelopmentPackage()
        {
            _logger.LogInformation("BuildDevelopmentPackage. Start");
            var ddsSettings = _appSettings.DevelopmentStudioSettings;
            ValidationHelper.ValidateDdsSection(ddsSettings, _logger);

            var tempPackagesPath = string.IsNullOrWhiteSpace(ddsSettings.TempPackagesPath)
                ? Path.GetTempPath()
                : ddsSettings.TempPackagesPath;
            _packagePath = Path.Combine(tempPackagesPath, $"DevelopmentPackage_{DateTime.Now:ddMMyy-HH-mm-ss}.dat");

            var ddsPsi = new ProcessStartInfo
            {
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                FileName = ddsSettings.ExePath,
                Arguments = $"-d {_packagePath} -c {ddsSettings.PackageInfoPath}"
            };

            using var ddsProcess = new Process();
            ddsProcess.StartInfo = ddsPsi;
            try
            {
                ddsProcess.Start();

                var stderr = ddsProcess.StandardError.ReadToEnd(); // pick up STDERR
                var stdout = ddsProcess.StandardOutput.ReadToEnd(); // pick up STDOUT

                if (!string.IsNullOrEmpty(stderr))
                    throw new Exception(stderr);

                if (!string.IsNullOrEmpty(stdout))
                    _logger.LogInformation(stdout);

                ddsProcess.WaitForExit();
                _logger.LogInformation("BuildDevelopmentPackage. Finish");

                if (ddsProcess.ExitCode > 0)
                {
                    throw new Exception(
                        $"An error occured while building dev package. Unexpected exit code - '{ddsProcess.ExitCode}'");
                }
            }
            finally
            {
                ddsProcess?.Close();
            }
        }

        private void DeployDevelopmentPackage()
        {
            _logger.LogInformation("DeployDevelopmentPackage. Start");

            var dtSection = _appSettings.DeploymentToolCoreSettings;
            ValidationHelper.ValidateDtSection(dtSection, _logger);

            if (string.IsNullOrEmpty(_packagePath))
                throw new ArgumentNullException("_packagePath", "Package path is empty");

            if (!File.Exists(_packagePath))
                throw new IOException("Package file does not exists");

            var login = dtSection.Login;
            var password = dtSection.Password;
            var dtPsi = new ProcessStartInfo
            {
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                FileName = dtSection.ExePath,
                Arguments = $"-n {login} -p {password} -d {_packagePath} -x"
            };

            using var dtProcess = new Process();
            dtProcess.StartInfo = dtPsi;
            try
            {
                dtProcess.Start();

                var stderr = dtProcess.StandardError.ReadToEnd(); // pick up STDERR
                var stdout = dtProcess.StandardOutput.ReadToEnd(); // pick up STDOUT

                if (!string.IsNullOrEmpty(stderr))
                    throw new Exception(stderr);

                if (!string.IsNullOrEmpty(stdout))
                    _logger.LogInformation(stdout);

                dtProcess.WaitForExit();
                _logger.LogInformation("DeployDevelopmentPackage. Finish");

                if (dtProcess.ExitCode > 0)
                {
                    throw new Exception(
                        $"An error occured while building dev package. Unexpected exit code - '{dtProcess.ExitCode}'");
                }
            }
            finally
            {
                dtProcess?.Close();
            }

            if (!_appSettings.DevelopmentStudioSettings.SavePackageAfterDeploy && File.Exists(_packagePath))
            {
                try
                {
                    File.Delete(_packagePath);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Cannot delete development package");
                }
            }
        }
    }
}