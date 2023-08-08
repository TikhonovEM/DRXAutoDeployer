using System.Threading.Tasks;
using Telegram.Bot;

namespace DirectumRXAutoDeployer.Notifiers.Telegram
{
    public class TelegramBot : IMessengerNotifier
    {
        private readonly TelegramBotClient _botClient;
        private readonly string _chatId;
        private readonly string _startMessage;
        private readonly string _finishMessage;
        private readonly string _errorMessage;

        public string Name { get; } = "Telegram";

        public TelegramBot(TelegramSettings settings)
        {
            _botClient = new TelegramBotClient(settings.Token);
            _chatId = settings.ChatIdentifier;
            _startMessage = settings.StartMessage;
            _finishMessage = settings.FinishMessage;
            _errorMessage = settings.ErrorMessage;
        }

        public async Task NotifyAboutStartAsync()
        {
            await _botClient.SendTextMessageAsync(_chatId, _startMessage);
        }

        public async Task NotifyAboutFinishAsync()
        {
            await _botClient.SendTextMessageAsync(_chatId, _finishMessage);
        }

        public async Task NotifyAboutErrorAsync(string errorMessage)
        {
            await _botClient.SendTextMessageAsync(_chatId, string.IsNullOrEmpty(errorMessage) ? _errorMessage : errorMessage);
        }

        public async Task SendMessage(string message)
        {
            await _botClient.SendTextMessageAsync(_chatId, message);
        }
    }
}