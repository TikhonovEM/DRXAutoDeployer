using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DirectumRXAutoDeployer.Notifiers.AgileBoards.SummaryBuilder
{
    public class PlainTextSummaryBuilder : ISummaryBuilder
    {
        private const string ListElementFormat = "{0}. {1}";
        
        public string GetSummaryText(IEnumerable<TicketInfo> tickets)
        {
            var sb = new StringBuilder();
            var ticketInfos = tickets.ToList();
            for (var i = 0; i < ticketInfos.Count(); i++)
            {
                sb.AppendFormat(ListElementFormat, i + 1, ticketInfos[0]);
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}