namespace DirectumRXAutoDeployer.Notifiers.Telegram
{
    public class TelegramSettings : MessengerNotifierSettings
    {
        public string Token { get; set; }
        public string ChatIdentifier { get; set; }
    }
}