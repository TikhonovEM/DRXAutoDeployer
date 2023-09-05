using System.Collections.Generic;

namespace DirectumRXAutoDeployer.Notifiers.Mattermost.Models.Request
{
    public class CreatePostRequest
    {
        public string channel_id { get; set; }
        public string message { get; set; }
        public string root_id { get; set; }
        public List<string> file_ids { get; set; }
        public Props props { get; set; }
        public Metadata metadata { get; set; }
    }
}
