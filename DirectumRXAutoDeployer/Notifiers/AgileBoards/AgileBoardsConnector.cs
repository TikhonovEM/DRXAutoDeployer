using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DirectumRXAutoDeployer.Notifiers.AgileBoards.ActionHandlers;
using Microsoft.Extensions.Logging;
using Microsoft.OData.Client;
using Microsoft.OData.Extensions.Client;
using Sungero.IntegrationService;

namespace DirectumRXAutoDeployer.Notifiers.AgileBoards
{
    public class AgileBoardsConnector : INotifier
    {
        private readonly ILogger<AgileBoardsConnector> _logger;
        private readonly AgileBoardSettings _agileBoardsSettings;
        private readonly List<IActionHandler> _actionHandlers = new List<IActionHandler>();

        public AgileBoardsConnector(ILogger<AgileBoardsConnector> logger, AgileBoardSettings settings, IODataClientFactory clientFactory)
        {
            _logger = logger;
            _agileBoardsSettings = settings;
            var client = clientFactory.CreateClient<Container>(new Uri(_agileBoardsSettings.IntegrationServiceUri),
                "AgileBoards");
            client.MergeOption = MergeOption.PreserveChanges;
            
            if (_agileBoardsSettings.Actions == null || !_agileBoardsSettings.Actions.Any())
            {
                _logger.LogWarning("List of actions is empty. Nothing to update.");
                return;
            }

            foreach (var action in settings.Actions)
            {
                switch (action.Target)
                {
                    case ActionTarget.Column: 
                        _actionHandlers.Add(new ColumnActionHandler(logger, client, settings, action));
                        break;
                    
                    case ActionTarget.Tag: 
                        _actionHandlers.Add(new TagActionHandler(logger, client, settings, action));
                        break;
                    default: break;
                }
            }
            
        }

        public async Task NotifyAboutStartAsync()
        {
            if (_agileBoardsSettings.Actions == null || !_agileBoardsSettings.Actions.Any())
            {
                _logger.LogWarning("List of actions is empty. Nothing to update.");
                return;
            }

            foreach (var actionHandler in _actionHandlers)
                await actionHandler.HandleStartAsync();
        }

        public async Task NotifyAboutFinishAsync()
        {
            if (_agileBoardsSettings.Actions == null || !_agileBoardsSettings.Actions.Any())
                return;

            foreach (var actionHandler in _actionHandlers)
                await actionHandler.HandleFinishAsync();
        }
        
        public Task NotifyAboutErrorAsync(string errorMessage)
        {
            return Task.CompletedTask;
        }
    }
}