using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AgileBoards;
using Microsoft.Extensions.Logging;
using Microsoft.OData.Extensions.Client;
using Sungero.IntegrationService;
using Sungero.IntegrationService.Models.Generated.AgileBoards;

namespace DirectumRXAutoDeployer.Notifiers.AgileBoards.ActionHandlers
{
    public class ColumnActionHandler : IActionHandler
    {
        private readonly ILogger _logger;
        private readonly Container _client;
        private readonly AgileBoardSettings _agileBoardsSettings;
        private readonly string _columnFrom;
        private readonly string _columnTo;
        private List<int> _ticketRefIds = new List<int>();

        public ColumnActionHandler(ILogger logger, Container client, AgileBoardSettings agileBoardsSettings, ActionSetting action)
        {
            _logger = logger;
            _client = client;
            _agileBoardsSettings = agileBoardsSettings;
            _columnFrom = action.ColumnFrom;
            _columnTo = action.ColumnTo;
        }
        
        public async Task HandleStartAsync()
        {
            var columnFrom = (await _client.IColumns
                    .Expand("Tickets")
                    .Where(c => c.Name == _columnFrom &&
                                c.BoardId == _agileBoardsSettings.BoardId)
                    .ExecuteAsync<IColumnDto>())
                .FirstOrDefault();

            if (columnFrom == null)
            {
                _logger.LogError("AgileBoardsConnector. Column with name '{0}' not found",  _agileBoardsSettings.ColumnFrom);
                return;
            }

            _ticketRefIds = columnFrom.Tickets.Select(t => t.Id).ToList();

            if (!_ticketRefIds.Any())
                _logger.LogWarning("AgileBoardsConnector. Nothing to move from '{0}'",  _agileBoardsSettings.ColumnFrom);
        }

        public async Task HandleFinishAsync()
        {
            if (_ticketRefIds == null || !_ticketRefIds.Any())
                return;

            var columnTo = (await _client.IColumns.Where(c => c.Name == _columnTo &&
                                                              c.BoardId == _agileBoardsSettings.BoardId).ExecuteAsync<IColumnDto>())
                .FirstOrDefault();
            
            if (columnTo == null)
            {
                _logger.LogError("AgileBoardsConnector. Column with name '{0}' not found",  _agileBoardsSettings.ColumnTo);
                return;
            }

            if (_ticketRefIds.Any())
            {
                try
                {
                    var result = await _client.AgileBoards.MoveTicket(_agileBoardsSettings.AppId,
                            _agileBoardsSettings.BoardId, _ticketRefIds, columnTo.Id, 0)
                        .GetValueAsync();

                    _logger.LogInformation("Tickets with ids '{0}' moved from '{1}' to '{2}'. New ref ids - '{3}'",
                        string.Join(',', _ticketRefIds),
                        _agileBoardsSettings.ColumnFrom,
                        _agileBoardsSettings.ColumnTo,
                        string.Join(',', result.NewRefIds));
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "An error occured while moving tickets");
                }
            }
        }
    }
}