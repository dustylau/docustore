using System;
using Lamar;

namespace DocuStore.IOC.Core.Interfaces
{
    public interface IHasConfiguration
    {
        Action<ServiceRegistry> Configuration { get; set; }
    }
}