using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DirectumRXAutoDeployer.Configuration;
using Sungero.IntegrationService;
using AgileBoards;

namespace DirectumRXAutoDeployer.Notifiers
{
    public class AgileBoardsConnector : INotifier
    {
        private readonly Container _context;

        public AgileBoardsConnector(AppSettings appSettings)
        {
            var settings = appSettings.NotifiersSettings.First(s => s.Target.ToLower().StartsWith("agileboards"));
            _context = new Container(new Uri(settings.IntegrationServiceUri));
        }

        public Task NotifyAboutStartAsync()
        {
            return Task.CompletedTask;
        }

        public Task NotifyAboutFinishAsync()
        {
            _context.AgileBoards.MoveTicket("agile", 3, new List<int>() { 4 }, 14, 0);
            return Task.CompletedTask;
        }

        public Task NotifyAboutErrorAsync(string errorMessage)
        {
            return Task.CompletedTask;
        }
    }
}