using System;
using System.Net;
using DirectumRXAutoDeployer.Configuration;
using DirectumRXAutoDeployer.Notifiers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Extensions.Client;

namespace DirectumRXAutoDeployer
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddNotifiers(this IServiceCollection services, AppSettings appSettings)
        {
            if (appSettings?.NotifiersSettings == null)
                return services;
            
            foreach (var setting in appSettings.NotifiersSettings)
            {
                var _ = setting.Target.ToLower() switch
                {
                    "telegram" => services.AddScoped<INotifier, TelegramBot>(),
                    "agileboards" => services.AddAgileBoardsODataClient(setting),
                    _ => services
                };
            }

            return services;
        }

        public static IServiceCollection AddAgileBoardsODataClient(this IServiceCollection services, NotifierSettings settings)
        {

            services.AddODataClient("AgileBoards")
                .ConfigureODataClient(dsc =>
                {
                    var credentials = settings.Token.Split(":");
                    dsc.Credentials = new NetworkCredential(credentials[0], credentials[1]);
                });

            services.AddScoped<INotifier, AgileBoardsConnector>();

            return services;
        }
    }
}