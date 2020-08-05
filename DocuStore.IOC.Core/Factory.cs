using System;
using DocuStore.IOC.Core.Interfaces;
using Lamar;

namespace DocuStore.IOC.Core
{
    public class Factory : IFactory
    {
        private readonly IContainer _container;

        public Factory(IContainer container)
        {
            _container = container;
        }

        public T Create<T>()
        {
            return _container.GetInstance<T>();
        }

        public T Create<T>(Action<T> initialization)
        {
            var instance = Create<T>();

            initialization?.Invoke(instance);

            return instance;
        }
    }

    public class Factory<T> : IFactory<T>
    {
        private readonly IContainer _container;

        public Factory(IContainer container)
        {
            _container = container;
        }

        public T Create()
        {
            return _container.GetInstance<T>();
        }

        public T Create(Action<T> initialization)
        {
            var instance = Create();

            initialization?.Invoke(instance);

            return instance;
        }
    }
}