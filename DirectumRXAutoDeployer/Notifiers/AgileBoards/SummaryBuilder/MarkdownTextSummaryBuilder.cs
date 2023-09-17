using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectumRXAutoDeployer.Notifiers.AgileBoards.SummaryBuilder
{
    public class MarkdownTextSummaryBuilder : ISummaryBuilder
    {
        private const string ListElementFormat = "{0}. [{1}]({2})";

        public string GetSummaryText(IEnumerable<TicketInfo> tickets)
        {
            var sb = new StringBuilder();
            var ticketInfos = tickets.ToList();
            for (var i = 0; i < ticketInfos.Count; i++)
            {
                var ticketInfo = ticketInfos[i];
                sb.AppendFormat(ListElementFormat, i + 1, ticketInfo.Name, ticketInfo.Hyperlink);
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}
