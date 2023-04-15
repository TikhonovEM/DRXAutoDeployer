using Microsoft.VisualBasic;

namespace DirectumRXAutoDeployer.Notifiers.AgileBoards
{
    public class TicketInfo
    {
        public string Name { get; set; }
        public string Hyperlink { get; set; }

        public override string ToString()
        {
            return string.IsNullOrWhiteSpace(Hyperlink) ? Name : $"{Name} ({Hyperlink})";
        }
    }
}