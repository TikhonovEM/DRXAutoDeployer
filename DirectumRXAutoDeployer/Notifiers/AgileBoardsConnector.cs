using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DirectumRXAutoDeployer.Configuration;
using Sungero.IntegrationService;
using AgileBoards;
using Microsoft.Extensions.Logging;
using Microsoft.OData.Extensions.Client;
using Sungero.IntegrationService.Models.Generated.AgileBoards;
using Sungero.IntegrationService.Models.Generated.ProjectRequirement;

namespace DirectumRXAutoDeployer.Notifiers
{
    public class AgileBoardsConnector : INotifier
    {
        private readonly ILogger<AgileBoardsConnector> _logger;
        private readonly IODataClientFactory _clientFactory;
        private readonly AgileBoardSettings _agileBoardsSettings;

        public AgileBoardsConnector(ILogger<AgileBoardsConnector> logger, AgileBoardSettings settings, IODataClientFactory clientFactory)
        {
            _logger = logger;
            _clientFactory = clientFactory;
            _agileBoardsSettings = settings;
        }

        public Task NotifyAboutStartAsync()
        {
            return Task.CompletedTask;
        }

        public async Task NotifyAboutFinishAsync()
        {
            var client =
                _clientFactory.CreateClient<Container>(new Uri(_agileBoardsSettings.IntegrationServiceUri),
                    "AgileBoards");

            var columnFrom = (await client.IColumns
                    .Expand("Tickets")
                    .Where(c => c.Name == _agileBoardsSettings.ColumnFrom &&
                                                              c.BoardId == _agileBoardsSettings.BoardId)
                    .ExecuteAsync<IColumnDto>())
                .FirstOrDefault();

            if (columnFrom == null)
            {
                _logger.LogError("AgileBoardsConnector. Column with name '{0}' not found",  _agileBoardsSettings.ColumnFrom);
                return;
            }

            var ticketRefIds = columnFrom.Tickets.Select(t => t.Id).ToList();

            if (!ticketRefIds.Any())
            {
                _logger.LogWarning("AgileBoardsConnector. Nothing to move from '{0}'",  _agileBoardsSettings.ColumnFrom);
            }
            
            var columnTo = (await client.IColumns.Where(c => c.Name == _agileBoardsSettings.ColumnTo &&
                                                              c.BoardId == _agileBoardsSettings.BoardId).ExecuteAsync<IColumnDto>())
                .FirstOrDefault();
            
            if (columnTo == null)
            {
                _logger.LogError("AgileBoardsConnector. Column with name '{0}' not found",  _agileBoardsSettings.ColumnTo);
                return;
            }

            try
            {
                var result = await client.AgileBoards.MoveTicket(_agileBoardsSettings.AppId, _agileBoardsSettings.BoardId, ticketRefIds, columnTo.Id, 0)
                    .GetValueAsync();
                
                _logger.LogInformation("Tickets with ids '{0}' moved from '{1}' to '{2}'. New ref ids - '{3}'", 
                    string.Join(',', ticketRefIds), 
                    _agileBoardsSettings.ColumnFrom, 
                    _agileBoardsSettings.ColumnTo,
                    string.Join(',', result.NewRefIds));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occured while moving tickets");
            }
            
            
            
        }

        public Task NotifyAboutErrorAsync(string errorMessage)
        {
            return Task.CompletedTask;
        }
    }
}