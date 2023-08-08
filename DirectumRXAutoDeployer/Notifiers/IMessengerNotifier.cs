﻿using System.Threading.Tasks;

namespace DirectumRXAutoDeployer.Notifiers
{
    public interface IMessengerNotifier : INotifier
    {
        string Name { get; }
        Task SendMessage(string message);
    }
}