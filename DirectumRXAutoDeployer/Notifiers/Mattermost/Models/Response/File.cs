namespace DirectumRXAutoDeployer.Notifiers.Mattermost.Models.Response
{
    public class File
    {
        public string id { get; set; }
        public string user_id { get; set; }
        public string post_id { get; set; }
        public long create_at { get; set; }
        public long update_at { get; set; }
        public long delete_at { get; set; }
        public string name { get; set; }
        public string extension { get; set; }
        public long size { get; set; }
        public string mime_type { get; set; }
        public long width { get; set; }
        public long height { get; set; }
        public bool has_preview_image { get; set; }
    }
}
