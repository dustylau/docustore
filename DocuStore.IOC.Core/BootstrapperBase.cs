using System;
using CommonServiceLocator;
using DocuStore.IOC.Core.Interfaces;
using Lamar;

namespace DocuStore.IOC.Core
{
    public class BootstrapperBase<T> : IBootstrapperExtended where T : IBootstrapperExtended, new()
    {
        private static bool _hasStarted;

        private static volatile Action<ServiceRegistry> _defaultConfiguration;
        private static readonly object DefaultConfigurationLock = new object();

        public static Action<ServiceRegistry> DefaultConfiguration
        {
            get
            {
                lock (DefaultConfigurationLock)
                {
                    return _defaultConfiguration;
                }
            }
            set
            {
                lock (DefaultConfigurationLock)
                {
                    _defaultConfiguration = value;
                }
            }
        }

        public static void ConfigureDefaults(Action<ServiceRegistry> configuration)
        {
            DefaultConfiguration = configuration;
        }

        public static IContainer Bootstrap()
        {
            return Bootstrap(null);
        }

        public static IContainer Bootstrap(Action<ServiceRegistry> additionalConfiguration)
        {
            var bootstrapper = new T();
            bootstrapper.BootstrapContainer(additionalConfiguration);
            return bootstrapper.Container;
        }

        public static void BootstrapAndSetLocatorProvider()
        {
            BootstrapAndSetLocatorProvider(null);
        }

        public static void BootstrapAndSetLocatorProvider(Action<ServiceRegistry> additionalConfiguration)
        {
            ServiceLocator.SetLocatorProvider(LamarServiceLocator.CreateProvider(Bootstrap(additionalConfiguration)));
        }

        public static void Restart()
        {
            if (_hasStarted) return;

            Bootstrap();

            _hasStarted = true;
        }

        public static void RestartAndSetLocatorProvider()
        {
            if (_hasStarted) return;

            BootstrapAndSetLocatorProvider();

            _hasStarted = true;
        }

        public static void Restart(Action<ServiceRegistry> additionalConfiguration)
        {
            if (_hasStarted) return;

            Bootstrap(additionalConfiguration);

            _hasStarted = true;
        }

        public static void RestartAndSetLocatorProvider(Action<ServiceRegistry> additionalConfiguration)
        {
            if (_hasStarted) return;

            BootstrapAndSetLocatorProvider(additionalConfiguration);

            _hasStarted = true;
        }

        public static void Reset()
        {
            _hasStarted = false;
            _defaultConfiguration = null;
        }

        private IContainer _container;
        public virtual IContainer Container
        {
            get
            {
                return _container;
            }
            set { _container = value; }
        }

        private Action<ServiceRegistry> _configuration;
        public virtual Action<ServiceRegistry> Configuration
        {
            get
            {
                return GetConfiguration();
            }
            set { _configuration = value; }
        }

        public virtual void BootstrapContainer()
        {
            BootstrapContainer(null);
        }

        public virtual void BootstrapContainer(Action<ServiceRegistry> additionalConfiguration)
        {
            var serviceRegistry = new ServiceRegistry();

            Configuration?.Invoke(serviceRegistry);

            additionalConfiguration?.Invoke(serviceRegistry);

            _container = new Container(serviceRegistry);
        }

        public virtual void Configure(Action<ServiceRegistry> configuration)
        {
            Configuration = configuration;
        }

        public virtual Action<ServiceRegistry> GetConfiguration()
        {
            return _configuration ?? DefaultConfiguration;
        }

        public IContainer GetContainer()
        {
            return Container;
        }
    }
}