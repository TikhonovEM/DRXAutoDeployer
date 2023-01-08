using System.Threading.Tasks;
using Telegram.Bot;

namespace DirectumRXAutoDeployer.Notifiers.Telegram
{
    public class TelegramBot : INotifier
    {
        private readonly TelegramBotClient _botClient;
        private readonly string _chatId;

        public TelegramBot(TelegramSettings settings)
        {
            _botClient = new TelegramBotClient(settings.Token);
            _chatId = settings.ChatIdentifier;
        }

        public async Task NotifyAboutStartAsync()
        {
            await _botClient.SendTextMessageAsync(_chatId, "Публикация на тестовый стенд. Старт");
        }

        public async Task NotifyAboutFinishAsync()
        {
            await _botClient.SendTextMessageAsync(_chatId, "Публикация на тестовый стенд. Финиш");
        }

        public async Task NotifyAboutErrorAsync(string errorMessage)
        {
            await _botClient.SendTextMessageAsync(_chatId, "Публикация на тестовый стенд. Ошибки публикации");
        }
    }
}