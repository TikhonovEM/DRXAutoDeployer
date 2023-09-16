using Microsoft.Extensions.Logging;
using Sungero.IntegrationService;
using Sungero.IntegrationService.Models.Generated.AgileBoards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace DirectumRXAutoDeployer.Notifiers.AgileBoards.ActionHandlers
{
    public class RemoveTagActionHandler : TagActionHandler
    {
        private readonly struct TicketTagPair
        {
            public TicketTagPair(long ticketId, long ticketTagId)
            {
                TicketId = ticketId;
                TicketTagId = ticketTagId;
            }

            public long TicketId { get; }
            public long TicketTagId { get; }
        }

        private readonly List<TicketTagPair> _ticketTagsToRemove = new();

        private const string DeleteUriTemplate = "ITickets({0})/TicketsTags({1})";

        public RemoveTagActionHandler(ILogger logger, Container client, AgileBoardSettings agileBoardsSettings, ActionSetting action) : base(logger, client, agileBoardsSettings, action)
        {
        }

        public override void HandleStartTagAction(IColumnDto columnFrom)
        {
            var ticketTagsToRemove = columnFrom.Tickets.SelectMany(t =>
                t.Ticket.TicketsTags.Where(tt => tt?.TicketTag?.Name == _action.TagName)
                    .Select(tt => new TicketTagPair(t.Ticket.Id, tt.Id)))
                .ToList();

            if (!ticketTagsToRemove.Any())
            {
                _logger.LogWarning("TagActionHandler. There are not tickets with tag '{0}' in column '{1}'", _action.TagName, _action.ColumnFrom);
                return;
            }

            _ticketTagsToRemove.AddRange(ticketTagsToRemove);
        }

        public override async Task HandleFinishTagActionAsync(HttpClient httpClient)
        {
            if (!_ticketTagsToRemove.Any())
                return;

            
            var successfulRemovedIds = new List<long>();
            foreach (var ticketTagToRemove in _ticketTagsToRemove)
            {
                try
                {
                    var deleteUri = string.Format(DeleteUriTemplate,
                        ticketTagToRemove.TicketId, ticketTagToRemove.TicketTagId);
                    var response = await httpClient.DeleteAsync(deleteUri);
                    if (response.IsSuccessStatusCode)
                        successfulRemovedIds.Add(ticketTagToRemove.TicketId);
                }
                catch
                {
                    _logger.LogWarning("TagActionHandler. Cannot remove tag from ticket {0}", ticketTagToRemove.TicketId);
                }
            }

            _logger.LogInformation("TagActionHandler. Tag successfully removed from tickets {0}", string.Join(";", successfulRemovedIds));
        }
    }
}
