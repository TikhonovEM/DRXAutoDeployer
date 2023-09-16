namespace DirectumRXAutoDeployer.Notifiers.Mattermost.Models.Response
{
    public class Reaction
    {
        public string user_id { get; set; }
        public string post_id { get; set; }
        public string emoji_name { get; set; }
        public long create_at { get; set; }
    }
}
