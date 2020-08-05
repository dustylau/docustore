using System;

namespace DocuStore.IOC.Core.Interfaces
{
    public interface IFactory<T>
    {
        T Create();
        T Create(Action<T> initialization);
    }

    public interface IFactory
    {
        T Create<T>();
        T Create<T>(Action<T> initialization);
    }
}