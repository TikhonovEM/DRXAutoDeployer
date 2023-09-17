using Microsoft.Extensions.Logging;
using Microsoft.OData.Extensions.Client;
using Newtonsoft.Json;
using Sungero.IntegrationService;
using Sungero.IntegrationService.Models.Generated.AgileBoards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DirectumRXAutoDeployer.Notifiers.AgileBoards.ActionHandlers
{
    public class AddTagActionHandler : TagActionHandler
    {
        private readonly List<long> _ticketsToAddTag = new();

        private const string AddUriTemplate = "ITickets({0})/TicketsTags";

        public AddTagActionHandler(ILogger logger, Container client, AgileBoardSettings agileBoardsSettings, ActionSetting action) : base(logger, client, agileBoardsSettings, action)
        {
        }

        public override void HandleStartTagAction(IColumnDto columnFrom)
        {
            var ticketsToAddTag = columnFrom.Tickets.Where(t => t.Ticket != null &&
                                                                t.Ticket.TicketsTags.All(tt => tt?.TicketTag?.Name != _action.TagName))
                
                .ToList();

            if (!ticketsToAddTag.Any())
            {
                _logger.LogWarning("TagActionHandler. There are not tickets to add tag '{0}' in column '{1}'", _action.TagName, _action.ColumnFrom);
                return;
            }

            _ticketsToAddTag.AddRange(ticketsToAddTag.Select(t => t.Ticket.Id));
            _ticketInfos.AddRange(ticketsToAddTag.Select(t => new TicketInfo(t.Ticket.Name, HyperlinksUtils.GetTicket(_agileBoardsSettings.BoardId, t.Ticket.Id))).ToList());
        }

        public override async Task HandleFinishTagActionAsync(HttpClient httpClient)
        {
            if (!_ticketsToAddTag.Any())
                return;

            var tag = (await _client.ITags.Where(t => t.Name == _action.TagName).ExecuteAsync<ITagDto>())
                .FirstOrDefault();

            if (tag == null)
            {
                _logger.LogWarning("TagActionHandler. Tag with name '{0}' not found in database", _action.TagName);
                return;
            }

            
            var successfulAddedIds = new List<long>();
            foreach (var ticketId in _ticketsToAddTag)
            {
                try
                {
                    var addUri = string.Format(AddUriTemplate, ticketId);
                    var ticketIdObject = new { Id = ticketId };
                    var tagIdObject = new { Id = tag.Id };
                    var requestObject = new { Ticket = ticketIdObject, TicketTag = tagIdObject };
                    var request = new StringContent(JsonConvert.SerializeObject(requestObject), Encoding.UTF8, "application/json");
                    var response = await httpClient.PostAsync(addUri, request);
                    if (response.IsSuccessStatusCode)
                        successfulAddedIds.Add(ticketId);
                }
                catch
                {
                    _logger.LogWarning("TagActionHandler. Cannot add tag to ticket {0}", ticketId);
                }
            }

            _logger.LogInformation("TagActionHandler. Tag successfully added to tickets {0}", string.Join(";", successfulAddedIds));
        }
    }
}
