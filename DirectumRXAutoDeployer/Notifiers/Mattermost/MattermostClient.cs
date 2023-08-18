using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace DirectumRXAutoDeployer.Notifiers.Mattermost
{
    public class MattermostClient : IMessengerNotifier
    {

        private readonly HttpClient _httpClient;
        private readonly MattermostSettings _settings;

        private const string PostsUri = "api/v4/posts";

        public string Name { get; } = "Mattermost";

        public MattermostClient(MattermostSettings settings)
        {
            _settings = settings;

            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(_settings.Server);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _settings.Token);
        }

        public async Task NotifyAboutErrorAsync(string errorMessage)
        {
            await SendMessage(_settings.ErrorMessage);
        }

        public async Task NotifyAboutFinishAsync()
        {
            await SendMessage(_settings.FinishMessage);
        }

        public async Task NotifyAboutStartAsync()
        {
            await SendMessage(_settings.StartMessage);
        }

        public async Task SendMessage(string message)
        {
            var messageObject = new { channel_id = _settings.ChannelId, message };
            await _httpClient.PostAsJsonAsync(PostsUri, messageObject);
        }
    }
}
