using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using DirectumRXAutoDeployer.Configuration;
using DirectumRXAutoDeployer.Deploy;
using DirectumRXAutoDeployer.Notifiers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace DirectumRXAutoDeployer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                var services = new ServiceCollection();
                ConfigureServices(services);
                var serviceProvider = services.BuildServiceProvider();
                
                var deployed = await serviceProvider.GetService<IDeployer>().TryDeployAsync();
            }
            catch (Exception ex)
            {
                LogManager.GetCurrentClassLogger().Error(ex, "Stopped program because of exception");
                throw;
            }
            finally
            {
                LogManager.Shutdown();
            }
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var appSettings = configuration.Get<AppSettings>();
            services.AddScoped(_ => appSettings);

            services.AddLogging(loggingBuilder =>
            {
                // configure Logging with NLog
                loggingBuilder.ClearProviders();
                loggingBuilder.SetMinimumLevel(LogLevel.Trace);
                loggingBuilder.AddNLog(configuration);
            });

            services.AddSingleton<IDeployer, Deployer>();
            
            ConfigureNotifiers(services, appSettings.NotifiersSettings);
        }

        private static void ConfigureNotifiers(IServiceCollection services, List<NotifierSettings> settings)
        {
            foreach (var setting in settings)
            {
                var _ = setting.Target.ToLower() switch
                {
                    "telegram" => services.AddScoped<INotifier, TelegramBot>(),
                    //"agileboards" => services.AddScoped<INotifier, AgileBoardsConnector>(),
                    _ => services
                };
            }
        }
    }
}