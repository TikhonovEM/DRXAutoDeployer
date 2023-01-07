using System;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using DirectumRXAutoDeployer.Configuration;
using DirectumRXAutoDeployer.Notifiers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Extensions.Client;

namespace DirectumRXAutoDeployer
{
    public static class ServiceCollectionExtensions
    {
        private const string SectionName = "NotifiersSettings";

        private const string SectionTargetKey = "Target";
        public static IServiceCollection AddNotifiers(this IServiceCollection services, IConfigurationRoot configuration)
        {
            var notifiersSection = configuration.GetSection(SectionName);
            
            if (notifiersSection == null)
                return services;
            
            foreach (var setting in notifiersSection.GetChildren())
            {
                var _ = setting.GetValue(SectionTargetKey, string.Empty).ToLower() switch
                {
                    "telegram" => services.AddTelegramBot(setting),
                    "agileboards" => services.AddAgileBoardsODataClient(setting),
                    _ => services
                };
            }

            return services;
        }

        public static IServiceCollection AddTelegramBot(this IServiceCollection services, IConfigurationSection telegramSection)
        {
            var telegramSettings = telegramSection.Get<TelegramSettings>();
            services.AddSingleton(telegramSettings);
            services.AddScoped<INotifier, TelegramBot>();

            return services;
        }

        public static IServiceCollection AddAgileBoardsODataClient(this IServiceCollection services, IConfigurationSection agileSection)
        {

            var agileSettings = agileSection.Get<AgileBoardSettings>();

            services.AddSingleton(agileSettings);
            
            services.AddODataClient("AgileBoards")
                .AddHttpClient()
                .ConfigureHttpClient(client =>
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                        Convert.ToBase64String(Encoding.ASCII.GetBytes(agileSettings.Token)));
                });

            services.AddScoped<INotifier, AgileBoardsConnector>();

            return services;
        }
    }
}