using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sungero.IntegrationService;

namespace DirectumRXAutoDeployer.Notifiers.AgileBoards.ActionHandlers
{
    public class TagActionHandler : IActionHandler
    {
        private readonly ILogger _logger;
        private readonly Container _client;
        private readonly AgileBoardSettings _agileBoardsSettings;
        private readonly ActionSetting _action;

        public TagActionHandler(ILogger logger, Container client, AgileBoardSettings agileBoardsSettings, ActionSetting action)
        {
            _logger = logger;
            _client = client;
            _agileBoardsSettings = agileBoardsSettings;
            _action = action;
        }
        
        public async Task HandleStartAsync()
        {
            return;
        }

        public async Task HandleFinishAsync()
        {
            return;
        }
    }
}