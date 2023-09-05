using System.Collections.Generic;

namespace DirectumRXAutoDeployer.Notifiers.Mattermost.Models.Response
{
    public class Metadata
    {
        public List<Embed> embeds { get; set; }
        public List<Emoji> emojis { get; set; }
        public List<File> files { get; set; }
        public Images images { get; set; }
        public List<Reaction> reactions { get; set; }
        public Priority priority { get; set; }
        public List<Acknowledgement> acknowledgements { get; set; }
    }
}
