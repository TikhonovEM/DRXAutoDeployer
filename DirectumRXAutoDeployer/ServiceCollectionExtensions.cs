using System;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
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
                .AddHttpClient()
                .ConfigureHttpClient(client =>
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                        Convert.ToBase64String(Encoding.ASCII.GetBytes(settings.Token)));
                });
                /*.ConfigureODataClient(dsc =>
                {
                    var credentials = settings.Token.Split(":");
                    dsc.Credentials = new NetworkCredential(credentials[0], credentials[1]);
                });*/

            services.AddScoped<INotifier, AgileBoardsConnector>();
            services.AddSingleton(AgileBoardSettings.FromNotifierSetting(settings));

            return services;
        }
    }
}