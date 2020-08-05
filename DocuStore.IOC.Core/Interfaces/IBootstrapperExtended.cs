using System;
using Lamar;

namespace DocuStore.IOC.Core.Interfaces
{
    public interface IBootstrapperExtended : IBootstrapper, IHasContainer, IHasConfiguration
    {
        void BootstrapContainer(Action<ServiceRegistry> additionalConfiguration);
        void Configure(Action<ServiceRegistry> configuration);
        Action<ServiceRegistry> GetConfiguration();
        IContainer GetContainer();
    }
}