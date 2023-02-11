namespace DirectumRXAutoDeployer.Notifiers.Telegram
{
    public class TelegramSettings
    {
        public string Token { get; set; }
        public string ChatIdentifier { get; set; }
        public string StartMessage { get; set; }
        public string FinishMessage { get; set; }
        public string ErrorMessage { get; set; }
    }
}