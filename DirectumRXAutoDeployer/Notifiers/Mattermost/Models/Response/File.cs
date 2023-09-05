namespace DirectumRXAutoDeployer.Notifiers.Mattermost.Models.Response
{
    public class File
    {
        public string id { get; set; }
        public string user_id { get; set; }
        public string post_id { get; set; }
        public int create_at { get; set; }
        public int update_at { get; set; }
        public int delete_at { get; set; }
        public string name { get; set; }
        public string extension { get; set; }
        public int size { get; set; }
        public string mime_type { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public bool has_preview_image { get; set; }
    }
}
