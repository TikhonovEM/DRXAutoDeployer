using System.Threading.Tasks;

namespace DirectumRXAutoDeployer.Notifiers
{
    public interface IMessengerNotifier : INotifier
    {
        Task SendMessage(string message);
    }
}