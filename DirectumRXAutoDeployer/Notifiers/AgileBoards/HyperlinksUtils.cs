using Microsoft.Extensions.DependencyInjection;

namespace DirectumRXAutoDeployer.Notifiers.AgileBoards
{
    public static class HyperlinksUtils
    {

        public static string GetTicket(long boardId, long ticketId)
        {
            var template = ServiceProviderFactory.ServiceProvider.GetService<AgileBoardSettings>()?.TicketHyperlinkTemplate;

            if (string.IsNullOrWhiteSpace(template))
                return string.Empty;

            return string.Format(template, boardId, ticketId);
        }

        public static bool IsConfigured() => !string.IsNullOrWhiteSpace(ServiceProviderFactory.ServiceProvider.GetService<AgileBoardSettings>()?.TicketHyperlinkTemplate);
    }
}
