using Microsoft.VisualBasic;

namespace DirectumRXAutoDeployer.Notifiers.AgileBoards
{
    public struct TicketInfo
    {
        public string Name { get; }
        public string Hyperlink { get; }

        public TicketInfo(string name, string hyperlink)
        {
            Name = name;
            Hyperlink = hyperlink;
        }

        public override string ToString()
        {
            return string.IsNullOrWhiteSpace(Hyperlink) ? Name : $"{Name} ({Hyperlink})";
        }
    }
}