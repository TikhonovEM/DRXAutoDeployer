using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sungero.IntegrationService;
using Sungero.IntegrationService.Models.Generated.AgileBoards;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.OData.Client;
using Microsoft.OData.Extensions.Client;
using Newtonsoft.Json;
using Microsoft.Extensions.DependencyInjection;

namespace DirectumRXAutoDeployer.Notifiers.AgileBoards.ActionHandlers
{
    public abstract class TagActionHandler : IActionHandler
    {
        protected readonly ILogger _logger;
        protected readonly Container _client;
        protected readonly AgileBoardSettings _agileBoardsSettings;
        protected readonly ActionSetting _action;
        protected readonly List<TicketInfo> _ticketInfos = new List<TicketInfo>();

        public TagActionHandler(ILogger logger, Container client, AgileBoardSettings agileBoardsSettings, ActionSetting action)
        {
            _logger = logger;
            _client = client;
            _agileBoardsSettings = agileBoardsSettings;
            _action = action;
        }

        public async Task HandleStartAsync()
        {
            var columnFrom = (await _client.IColumns
                    .Expand("Tickets($expand=Ticket($expand=TicketsTags($expand=TicketTag)))")
                    .Where(c => c.Name == _action.ColumnFrom &&
                                c.BoardId == _agileBoardsSettings.BoardId)
                    .ExecuteAsync<IColumnDto>())
                .FirstOrDefault();

            if (columnFrom == null)
            {
                _logger.LogError("TagActionHandler. Column with name '{0}' not found", _action.ColumnFrom);
                return;
            }

            HandleStartTagAction(columnFrom);
        }

        public async Task HandleFinishAsync()
        {
            // В общем я пока не разобрался, как через ODataClient удалить записи в коллекции, поэтому буду слать обычные http-запросы.

            using var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(_agileBoardsSettings.IntegrationServiceUri.EndsWith("/") ? _agileBoardsSettings.IntegrationServiceUri : _agileBoardsSettings.IntegrationServiceUri + "/");

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(Encoding.ASCII.GetBytes(_agileBoardsSettings.Token)));

            await HandleFinishTagActionAsync(httpClient);

            if (_ticketInfos.Any() && !string.IsNullOrWhiteSpace(_agileBoardsSettings.SummaryTarget))
            {
                var summaryTarget = (IMessengerNotifier)ServiceProviderFactory.ServiceProvider.GetServices<INotifier>()
                    .FirstOrDefault(n => n is IMessengerNotifier mn && mn.Name == _agileBoardsSettings.SummaryTarget);

                if (summaryTarget != null)
                {
                    var summaryBuilder = SummaryBuilder.SummaryBuilderProvider.GetBuilderByTarget(_agileBoardsSettings.SummaryTarget);
                    var summary = summaryBuilder.GetSummaryText(_ticketInfos);
                    if (!string.IsNullOrWhiteSpace(_action.SummaryHeader))
                        summary = $"{_action.SummaryHeader}{Environment.NewLine}{summary}";
                    await summaryTarget.SendMessage(summary);
                }

            }
        }

        public abstract void HandleStartTagAction(IColumnDto columnFrom);

        public abstract Task HandleFinishTagActionAsync(HttpClient httpClient);

        public static TagActionHandler CreateHandler(ILogger logger, Container client, AgileBoardSettings agileBoardsSettings, ActionSetting action)
        {
            return action.Type switch
            {
                TagActionType.Add => new AddTagActionHandler(logger, client, agileBoardsSettings, action),
                TagActionType.Remove => new RemoveTagActionHandler(logger, client, agileBoardsSettings, action),
                _ => throw new NotImplementedException(),
            };
        }
    }
}