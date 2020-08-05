using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using DocStore.Repository.Sql;
using DocuStore.Core.Interfaces;
using DocuStore.Core.Shared;
using DocuStore.IOC.Core;
using DocuStore.IOC.Core.Interfaces;
using DocuStore.Shared;
using Lamar;
using Microsoft.Extensions.Logging;
using Serilog;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace DocuStore.IOC
{
    public class Bootstrapper : BootstrapperBase<Bootstrapper>
    {
        public ISettings Settings { get; } = new Settings();

        public Action<ILoggerFactory> ConfigureLogger { get; set; } = factory =>
        {
        };

        public Action<ILoggingBuilder> ConfigureLoggingBuilder { get; set; } = builder =>
        {
        };

        public Func<ILoggerFactory, ILogger> CreateLogger { get; set; } = factory => factory.CreateLogger("DocuStore");

        public Bootstrapper()
        {
            Configure(configuration =>
            {
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .Enrich.FromLogContext()
                    .WriteTo.Debug()
                    .WriteTo.Console()
                    .CreateLogger();

                var loggerFactory = LoggerFactory
                    .Create(ConfigureLoggingBuilder)
                    .AddSerilog(Log.Logger);

                ConfigureLogger.Invoke(loggerFactory);

                configuration.ForSingletonOf<ILoggerFactory>().Use(loggerFactory);

                var logger = CreateLogger == null ? loggerFactory.CreateLogger("DocuStore") : CreateLogger(loggerFactory);

                configuration.ForSingletonOf<ISettings>().Use(Settings);

                configuration.ForSingletonOf<ILogger>().Use(logger);

                configuration.ForSingletonOf<IFactory>().Use<ServiceLocatorFactory>();

                configuration.ForSingletonOf<IFileSystem>().Use<FileSystem>();

                /*
                configuration.Scan(scanner =>
                {
                    scanner.AssemblyContainingType<IFileSystem>();
                    scanner.IncludeNamespaceContainingType<IFileSystem>();
                    scanner.AssemblyContainingType<FileSystem>();
                    scanner.IncludeNamespaceContainingType<FileSystem>();
                    scanner.SingleImplementationsOfInterface();
                    scanner.WithDefaultConventions();
                });
                */
            });
        }
    }
}
