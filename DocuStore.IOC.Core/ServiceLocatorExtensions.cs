using System;
using CommonServiceLocator;
using DocuStore.IOC.Core.Interfaces;
using Lamar;

namespace DocuStore.IOC.Core
{
    public static class ServiceLocatorExtensions
    {
        public static IHasContainer GetHasContainer(this IServiceLocator locator)
        {
            var hasContainer = locator as IHasContainer;

            if (hasContainer == null)
                throw new Exception("Locator does not implement IHasContainer.");

            return hasContainer;
        }

        public static void GetHasContainer(this IServiceLocator locator, Action<IHasContainer> callback)
        {
            callback(GetHasContainer(locator));
        }

        public static IContainer GetContainer(this IServiceLocator locator)
        {
            return GetHasContainer(locator).Container;
        }

        public static void GetContainer(this IServiceLocator locator, Action<IContainer> callback)
        {
            callback(GetContainer(locator));
        }

        public static T GetInstance<T>(this IServiceLocator locator, Action<T> configuration)
        {
            var instance = locator.GetInstance<T>();

            if (configuration != null)
                configuration(instance);

            return instance;
        }
    }
}