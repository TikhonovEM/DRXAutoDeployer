namespace DirectumRXAutoDeployer.Notifiers.Mattermost.Models.Response
{
    public class Acknowledgement
    {
        public string user_id { get; set; }
        public string post_id { get; set; }
        public long acknowledged_at { get; set; }
    }
}
