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
using Sungero.IntegrationService.Models.Generated.ProjectRequirement;
using Newtonsoft.Json;

namespace DirectumRXAutoDeployer.Notifiers.AgileBoards.ActionHandlers
{
    public class TagActionHandler : IActionHandler
    {
        private struct TicketTagPair
        {
            public TicketTagPair(int ticketId, int ticketTagId)
            {
                TicketId = ticketId;
                TicketTagId = ticketTagId;
            }
            
            public int TicketId { get; }
            public int TicketTagId { get; }
        }
  
        private readonly ILogger _logger;
        private readonly Container _client;
        private readonly AgileBoardSettings _agileBoardsSettings;
        private readonly ActionSetting _action;
        private readonly List<TicketTagPair> _ticketTagsToRemove = new List<TicketTagPair>();
        private readonly List<int> _ticketsToAddTag = new List<int>();

        public TagActionHandler(ILogger logger, Container client, AgileBoardSettings agileBoardsSettings, ActionSetting action)
        {
            _logger = logger;
            _client = client;
            _agileBoardsSettings = agileBoardsSettings;
            _action = action;
        }
        
        public async Task HandleStartAsync()
        {
            if (_action.Type == TagActionType.Remove)
                await HandleStartRemoveActionAsync();
            else
                await HandleStartAddActionAsync();
        }

        private async Task HandleStartRemoveActionAsync()
        {
            var columnFrom = (await _client.IColumns
                    .Expand("Tickets($expand=Ticket($expand=TicketsTags($expand=TicketTag)))")
                    .Where(c => c.Name == _action.ColumnFrom &&
                                c.BoardId == _agileBoardsSettings.BoardId)
                    .ExecuteAsync<IColumnDto>())
                .FirstOrDefault();

            if (columnFrom == null)
            {
                _logger.LogError("TagActionHandler. Column with name '{0}' not found",  _agileBoardsSettings.ColumnFrom);
                return;
            }

            var ticketTagsToRemove = columnFrom.Tickets.SelectMany(t =>
                t.Ticket.TicketsTags.Where(tt => tt?.TicketTag?.Name == _action.TagName)
                    .Select(tt => new TicketTagPair(t.Ticket.Id, tt.Id)))
                .ToList();

            if (!ticketTagsToRemove.Any())
            {
                _logger.LogWarning("TagActionHandler. There are not tickets with tag '{0}' in column '{1}'",  _action.TagName, _action.ColumnFrom);
                return;
            }
            
            _ticketTagsToRemove.AddRange(ticketTagsToRemove);
        }

        private async Task HandleStartAddActionAsync()
        {
            var columnFrom = (await _client.IColumns
                    .Expand("Tickets($expand=Ticket($expand=TicketsTags($expand=TicketTag)))")
                    .Where(c => c.Name == _action.ColumnFrom &&
                                c.BoardId == _agileBoardsSettings.BoardId)
                    .ExecuteAsync<IColumnDto>())
                .FirstOrDefault();
            
            if (columnFrom == null)
            {
                _logger.LogError("TagActionHandler. Column with name '{0}' not found",  _agileBoardsSettings.ColumnFrom);
                return;
            }

            var ticketsToAddTag = columnFrom.Tickets.Where(t => t.Ticket != null && 
                                                                t.Ticket.TicketsTags.All(tt => tt?.TicketTag?.Name != _action.TagName))
                .Select(t => t.Ticket.Id)
                .ToList();

            if (!ticketsToAddTag.Any())
            {
                _logger.LogWarning("TagActionHandler. There are not tickets to add tag '{0}' in column '{1}'",  _action.TagName, _action.ColumnFrom);
                return;
            }
            
            _ticketsToAddTag.AddRange(ticketsToAddTag);
        }

        public async Task HandleFinishAsync()
        {
            // В общем я пока не разобрался, как через ODataClient удалить записи в коллекции, поэтому буду слать обычные http-запросы.

            using var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(_agileBoardsSettings.IntegrationServiceUri.EndsWith("/") ? _agileBoardsSettings.IntegrationServiceUri : _agileBoardsSettings.IntegrationServiceUri + "/");
            
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(Encoding.ASCII.GetBytes(_agileBoardsSettings.Token)));
            
            if (_action.Type == TagActionType.Remove)
                await HandleFinishRemoveActionAsync(httpClient);
            else
                await HandleFinishAddActionAsync(httpClient);
        }

        private async Task HandleFinishRemoveActionAsync(HttpClient httpClient)
        {
            if (!_ticketTagsToRemove.Any())
                return;

            const string deleteUriTemplate = "ITickets({0})/TicketsTags({1})";
            var successfulRemovedIds = new List<int>();
            foreach (var ticketTagToRemove in _ticketTagsToRemove)
            {
                try
                {
                    var deleteUri = string.Format(deleteUriTemplate,
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

        private async Task HandleFinishAddActionAsync(HttpClient httpClient)
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

            const string addUriTemplate = "ITickets({0})/TicketsTags";
            var successfulAddedIds = new List<int>();
            foreach (var ticketId in _ticketsToAddTag)
            {
                try
                {
                    var addUri = string.Format(addUriTemplate, ticketId);
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