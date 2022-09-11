using System.Threading.Tasks;

namespace DirectumRXAutoDeployer.Notifiers
{
    public interface INotifier
    {
        Task NotifyAboutStartAsync();

        Task NotifyAboutFinishAsync();

        Task NotifyAboutErrorAsync(string errorMessage);
    }
}