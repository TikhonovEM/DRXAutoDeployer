using System.Threading.Tasks;

namespace DirectumRXAutoDeployer.Deploy
{
    public interface IDeployer
    {
        Task<bool> TryDeployAsync();
    }
}