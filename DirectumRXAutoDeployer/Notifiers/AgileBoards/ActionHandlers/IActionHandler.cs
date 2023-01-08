using System.Collections.Generic;
using System.Threading.Tasks;

namespace DirectumRXAutoDeployer.Notifiers.AgileBoards.ActionHandlers
{
    public interface IActionHandler
    {
        Task HandleStartAsync();

        Task HandleFinishAsync();
    }
}