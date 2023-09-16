﻿using System.Collections.Generic;

namespace DirectumRXAutoDeployer.Notifiers.Mattermost.Models.Response
{
    public class CreatePostResponse
    {
        public string id { get; set; }
        public long create_at { get; set; }
        public long update_at { get; set; }
        public long delete_at { get; set; }
        public long edit_at { get; set; }
        public string user_id { get; set; }
        public string channel_id { get; set; }
        public string root_id { get; set; }
        public string original_id { get; set; }
        public string message { get; set; }
        public string type { get; set; }
        public Props props { get; set; }
        public string hashtag { get; set; }
        public List<string> file_ids { get; set; }
        public string pending_post_id { get; set; }
        public Metadata metadata { get; set; }
    }
}