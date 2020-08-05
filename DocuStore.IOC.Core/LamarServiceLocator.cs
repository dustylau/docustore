using System;
using System.Collections.Generic;
using System.Linq;
using CommonServiceLocator;
using DocuStore.IOC.Core.Interfaces;
using Lamar;

namespace DocuStore.IOC.Core
{
    public class LamarServiceLocator : IServiceLocator, IHasContainer
    {
        private readonly IContainer _container;

        public LamarServiceLocator(IContainer container)
        {
            _container = container;
        }

        public IContainer Container
        {
            get { return _container; }
        }

        public IEnumerable<object> GetAllInstances(Type serviceType)
        {
            return _container.GetAllInstances(serviceType).Cast<object>();
        }

        public IEnumerable<TService> GetAllInstances<TService>()
        {
            return _container.GetAllInstances<TService>();
        }

        public object GetInstance(Type serviceType)
        {
            return _container.GetInstance(serviceType);
        }

        public object GetInstance(Type serviceType, string key)
        {
            return _container.GetInstance(serviceType, key);
        }

        public TService GetInstance<TService>()
        {
            return _container.GetInstance<TService>();
        }

        public TService GetInstance<TService>(string key)
        {
            return _container.GetInstance<TService>(key);
        }

        public object GetService(Type serviceType)
        {
            return _container.GetService(serviceType);
        }

        public static ServiceLocatorProvider CreateProvider(IContainer container)
        {
            return () => new LamarServiceLocator(container);
        }

        /*
        public static ServiceLocatorProvider CreateProvider()
        {
            return CreateProvider(new Lamar.Container());
        }

        public static ServiceLocatorProvider NewProvider
        {
            get { return CreateProvider(); }
        }
        */
    }
}