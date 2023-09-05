using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DirectumRXAutoDeployer.Notifiers.Mattermost
{
    public class MattermostClient : IMessengerNotifier
    {

        private readonly HttpClient _httpClient;
        private readonly MattermostSettings _settings;
        private string _rootId;

        private const string PostsUri = "api/v4/posts";

        public string Name { get; } = "Mattermost";

        public MattermostClient(MattermostSettings settings)
        {
            _settings = settings;

            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(_settings.Server);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _settings.Token);
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
            var postRequest = new Models.Request.CreatePostRequest()
            {
                channel_id = _settings.ChannelId,
                message = message,
                root_id = _settings.OneThread ? _rootId : null
            };

            var httpResponse = await _httpClient.PostAsJsonAsync(PostsUri, postRequest);

            if (_settings.OneThread)
            {
                var postResponse = JsonConvert.DeserializeObject<Models.Response.CreatePostResponse>(await httpResponse.Content.ReadAsStringAsync());
                _rootId = postResponse?.root_id;
            }

        }
    }
}
