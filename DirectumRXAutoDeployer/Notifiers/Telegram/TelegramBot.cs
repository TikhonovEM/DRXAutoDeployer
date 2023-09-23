using System.Runtime;
using System.Threading.Tasks;
using Telegram.Bot;

namespace DirectumRXAutoDeployer.Notifiers.Telegram
{
    public class TelegramBot : IMessengerNotifier
    {
        private readonly TelegramBotClient _botClient;
        private readonly TelegramSettings _settings;

        public string Name { get; } = "Telegram";

        public TelegramBot(TelegramSettings settings)
        {
            _settings = settings;
            _botClient = new TelegramBotClient(_settings.Token);
        }

        public async Task NotifyAboutStartAsync()
        {
            await _botClient.SendTextMessageAsync(_settings.ChatIdentifier, _settings.StartMessage);
        }

        public async Task NotifyAboutFinishAsync()
        {
            await _botClient.SendTextMessageAsync(_settings.ChatIdentifier, _settings.FinishMessage);
        }

        public async Task NotifyAboutErrorAsync(string errorMessage)
        {
            await _botClient.SendTextMessageAsync(_settings.ChatIdentifier, string.IsNullOrEmpty(errorMessage) ? _settings.ErrorMessage : errorMessage);
        }

        public async Task SendMessage(string message)
        {
            await _botClient.SendTextMessageAsync(_settings.ChatIdentifier, message);
        }

        public async Task NotifyAboutStartPullFromGit()
        {
            if (!string.IsNullOrWhiteSpace(_settings.StartPullFromGitMessage))
                await SendMessage(_settings.StartPullFromGitMessage);
        }

        public async Task NotifyAboutFinishPullFromGit()
        {
            if (!string.IsNullOrWhiteSpace(_settings.FinishPullFromGitMessage))
                await SendMessage(_settings.FinishPullFromGitMessage);
        }

        public async Task NotifyAboutStartBuildPackage()
        {
            if (!string.IsNullOrWhiteSpace(_settings.StartBuildPackageMessage))
                await SendMessage(_settings.StartBuildPackageMessage);
        }

        public async Task NotifyAboutFinishBuildPackage()
        {
            if (!string.IsNullOrWhiteSpace(_settings.FinishBuildPackageMessage))
                await SendMessage(_settings.FinishBuildPackageMessage);
        }

        public async Task NotifyAboutStartDeployPackage()
        {
            if (!string.IsNullOrWhiteSpace(_settings.StartDeployPackageMessage))
                await SendMessage(_settings.StartDeployPackageMessage);
        }

        public async Task NotifyAboutFinishDeployPackage()
        {
            if (!string.IsNullOrWhiteSpace(_settings.FinishDeployPackageMessage))
                await SendMessage(_settings.FinishDeployPackageMessage);
        }
    }
}