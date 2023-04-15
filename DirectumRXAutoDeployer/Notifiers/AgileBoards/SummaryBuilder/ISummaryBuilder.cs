using System.Collections.Generic;

namespace DirectumRXAutoDeployer.Notifiers.AgileBoards.SummaryBuilder
{
    public interface ISummaryBuilder
    {
        string GetSummaryText(IEnumerable<TicketInfo> tickets);
    }
}