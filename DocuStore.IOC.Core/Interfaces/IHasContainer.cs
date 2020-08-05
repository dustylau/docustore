using Lamar;

namespace DocuStore.IOC.Core.Interfaces
{
    public interface IHasContainer
    {
        IContainer Container { get; }
    }
}