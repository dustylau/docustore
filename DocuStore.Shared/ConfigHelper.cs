using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using ConfigurationBuilder = Microsoft.Extensions.Configuration.ConfigurationBuilder;

namespace DocuStore.Shared
{
    public static class ConfigHelper
    {
#if DEBUG
        public static eEnvironment Environment { get; set; } = eEnvironment.Debug;
#elif DEVELOPMENT
        public static eEnvironment Environment { get; set; } = eEnvironment.Development;
#elif TEST
        public static eEnvironment Environment { get; set; } = eEnvironment.Test;
#elif PRODUCTION
        public static eEnvironment Environment { get; set; } = eEnvironment.Production;
#elif RELEASE
        public static eEnvironment Environment { get; set; } = eEnvironment.Production;
#else
        public static eEnvironment Environment { get; set; } = eEnvironment.Debug;
#endif

        public static Dictionary<string, string> AppSettings = new Dictionary<string, string>();

        private static IConfiguration _configuration;

        public static IConfiguration Configuration
        {
            get
            {
                if (_configuration != null)
                    return _configuration;

                var builder = new ConfigurationBuilder();
                
                //builder.SetBasePath(Directory.GetCurrentDirectory());

                //builder.SetBasePath(System.Environment.CurrentDirectory);

                var appSettingsLocation = ConfigFileDirectory;

                if (AppSettings != null && AppSettings.ContainsKey("AppSettingsLocation"))
                    appSettingsLocation = AppSettings["AppSettingsLocation"];

                if (string.IsNullOrWhiteSpace(appSettingsLocation))
                    appSettingsLocation = ConfigFileDirectory;

                builder.SetBasePath(appSettingsLocation);

                builder.AddEnvironmentVariables();
                builder.AddInMemoryCollection();
                builder.AddJsonFile("appsettings.json", false);
                builder.AddJsonFile($"appsettings.{Environment}.json", false);

                _configuration = builder.Build();

                return _configuration;
            }
            set => _configuration = value;
        }

        public static void Reset()
        {
            _configuration = null;
        }

        public static string GetValue(string key, string defaultValue)
        {
            var value = Configuration[key];

            if (string.IsNullOrWhiteSpace(value))
                return defaultValue;

            return value;
        }

        public static string GetValue(string key)
        {
            return GetValue(key, default(string));
        }

        public static void GetValue(string key, Action<string> callback)
        {
            callback(GetValue(key));
        }

        public static void GetValue(string key, string defaultValue, Action<string> callback)
        {
            callback(GetValue(key, defaultValue));
        }

        public static T Get<T>(string key, T defaultValue)
        {
            var stringValue = GetValue(key);
            return stringValue == null ? defaultValue : stringValue.ConvertTo<T>(defaultValue);
        }

        public static T Get<T>(string key)
        {
            return Get<T>(key, default(T));
        }

        public static void Get<T>(string key, Action<T> callback)
        {
            callback(Get<T>(key));
        }

        public static void Get<T>(string key, T defaultValue, Action<T> callback)
        {
            callback(Get<T>(key, defaultValue));
        }

        public static string ConnectionString(string connectionStringName)
        {
            return ConnectionString(connectionStringName, null as string);
        }

        public static string ConnectionString(string connectionStringName, string defaultConnectionStringValue)
        {
            var connectionStrings = Configuration.GetSection("ConnectionStrings");

            if (connectionStrings == null)
                return defaultConnectionStringValue;

            var connectionString = connectionStrings[connectionStringName];

            return connectionString ?? defaultConnectionStringValue;
        }

        public static void ConnectionString(string connectionStringName, Action<string> callback)
        {
            callback(ConnectionString(connectionStringName));
        }

        public static void ConnectionString(string connectionStringName, string defaultConnectionStringValue, Action<string> callback)
        {
            callback(ConnectionString(connectionStringName, defaultConnectionStringValue));
        }

        public static string ConigFilePath => $"{Assembly.GetEntryAssembly()?.Location}.config";

        public static string ConfigFileDirectory => new FileInfo($"{Assembly.GetEntryAssembly()?.Location}.config").DirectoryName;
    }
}