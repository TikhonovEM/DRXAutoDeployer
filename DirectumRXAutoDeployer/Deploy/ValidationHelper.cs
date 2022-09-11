using System;
using DirectumRXAutoDeployer.Configuration;
using Microsoft.Extensions.Logging;

namespace DirectumRXAutoDeployer.Deploy
{
    public static class ValidationHelper
    {
        public static void ValidateDtSection(DeploymentToolCoreSettings dtSettings, ILogger logger)
        {
            if (string.IsNullOrWhiteSpace(dtSettings.ExePath))
                throw new ArgumentNullException(nameof(dtSettings.ExePath));
            
            if (string.IsNullOrWhiteSpace(dtSettings.Login))
                logger.LogWarning("Param \"Login\" is empty. Will be used default");
            
            if (string.IsNullOrWhiteSpace(dtSettings.Password))
                logger.LogWarning("Param \"Password\" is empty. Will be used default");
        }

        public static void ValidateDdsSection(DevelopmentStudioSettings ddsSettings, ILogger logger)
        {
            if (string.IsNullOrWhiteSpace(ddsSettings.ExePath))
                throw new ArgumentNullException(nameof(ddsSettings.ExePath));
            
            if (string.IsNullOrWhiteSpace(ddsSettings.PackageInfoPath))
                throw new ArgumentNullException(nameof(ddsSettings.PackageInfoPath));
            
            if (string.IsNullOrWhiteSpace(ddsSettings.TempPackagesPath))
                logger.LogWarning("Param \"TempPackagesPath\" is empty. Packages will be created in standard temp directory");
        }
        
        public static void ValidateGitSection(GitRepositorySettings gitSettings, ILogger logger)
        {
            if (string.IsNullOrWhiteSpace(gitSettings.ExePath))
                logger.LogWarning("Param \"ExePath\" is empty. Will be used default");
            
            if (string.IsNullOrWhiteSpace(gitSettings.SourcesPath))
                throw new ArgumentNullException(nameof(gitSettings.SourcesPath));
            
            if (string.IsNullOrWhiteSpace(gitSettings.BranchName))
                logger.LogWarning("Param \"BranchName\" is empty. Using default branch \"master\"");
        }
    }
}