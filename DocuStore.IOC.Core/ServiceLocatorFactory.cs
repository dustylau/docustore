using System;
using CommonServiceLocator;
using DocuStore.IOC.Core.Interfaces;

namespace DocuStore.IOC.Core
{
    public class ServiceLocatorFactory : IFactory
    {
        public T Create<T>()
        {
            return ServiceLocator.Current.GetInstance<T>();
        }

        public T Create<T>(Action<T> initialization)
        {
            var instance = Create<T>();
            initialization(instance);
            return instance;
        }
    }
}