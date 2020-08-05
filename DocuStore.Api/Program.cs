using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lamar.Microsoft.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DocuStore.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseLamar()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();

                    webBuilder.ConfigureServices(services =>
                    {
                        JsonConvert.DefaultSettings = () => new JsonSerializerSettings()
                        {
                            Formatting = Formatting.Indented,
                            ContractResolver = new CamelCasePropertyNamesContractResolver(),
                            PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        };

                        // This is important, the call to AddControllers()
                        // cannot be made before the usage of ConfigureWebHostDefaults
                        services
                            .AddControllers()
                            .AddNewtonsoftJson();
                    });
                });
    }
}
