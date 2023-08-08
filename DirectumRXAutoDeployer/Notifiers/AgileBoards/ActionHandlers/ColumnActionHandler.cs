using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AgileBoards;
using Microsoft.Extensions.DependencyInjection;
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
        private List<long> _ticketRefIds = new List<long>();
        private List<TicketInfo> _ticketInfos = new List<TicketInfo>();

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
                    .Expand("Tickets($expand=Ticket)")
                    .Where(c => c.Name == _columnFrom &&
                                c.BoardId == _agileBoardsSettings.BoardId)
                    .ExecuteAsync<IColumnDto>())
                .FirstOrDefault();

            if (columnFrom == null)
            {
                _logger.LogError("ColumnActionHandler. Column with name '{0}' not found", _columnFrom);
                return;
            }

            _ticketRefIds = columnFrom.Tickets.Select(t => t.Id).ToList();
            _ticketInfos = columnFrom.Tickets.Select(t => new TicketInfo(t.Ticket.Name, null)).ToList();

            if (!_ticketRefIds.Any())
                _logger.LogWarning("ColumnActionHandler. Nothing to move from '{0}'", _columnFrom);
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
                _logger.LogError("ColumnActionHandler. Column with name '{0}' not found", _columnTo);
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
                        _columnFrom,
                        _columnTo,
                        string.Join(',', result.NewRefIds));
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "An error occured while moving tickets");
                }

                if (!string.IsNullOrWhiteSpace(_agileBoardsSettings.SummaryTarget))
                {
                    var summaryTarget = (IMessengerNotifier)ServiceProviderFactory.ServiceProvider.GetServices<INotifier>()
                        .FirstOrDefault(n => n is IMessengerNotifier mn && mn.Name == _agileBoardsSettings.SummaryTarget);

                    if (summaryTarget != null)
                    {
                        var summaryBuilder = SummaryBuilder.SummaryBuilderProvider.GetBuilderByTarget(_agileBoardsSettings.SummaryTarget);
                        var summary = summaryBuilder.GetSummaryText(_ticketInfos);
                        await summaryTarget.SendMessage(summary);
                    }
                }
                
            }


        }
    }
}