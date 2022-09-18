using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DirectumRXAutoDeployer.Configuration;
using Sungero.IntegrationService;
using AgileBoards;
using Microsoft.OData.Extensions.Client;

namespace DirectumRXAutoDeployer.Notifiers
{
    public class AgileBoardsConnector : INotifier
    {
        private readonly IODataClientFactory _clientFactory;
        private readonly string _agileBoardsServiceRoot;

        public AgileBoardsConnector(AppSettings appSettings, IODataClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
            _agileBoardsServiceRoot = appSettings.NotifiersSettings.First(s => s.Target.ToLower() == "agileboards")
                .IntegrationServiceUri;
        }

        public Task NotifyAboutStartAsync()
        {
            return Task.CompletedTask;
        }

        public Task NotifyAboutFinishAsync()
        {
            var client = _clientFactory.CreateClient<Container>(new Uri(_agileBoardsServiceRoot), "AgileBoards");
            client.AgileBoards.MoveTicket("agile", 3, new List<int>() { 4 }, 14, 0);
            return Task.CompletedTask;
        }

        public Task NotifyAboutErrorAsync(string errorMessage)
        {
            return Task.CompletedTask;
        }
    }
}