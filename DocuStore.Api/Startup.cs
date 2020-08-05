using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocuStore.Core.Interfaces;
using DocuStore.IOC;
using Lamar;
//using Microsoft.AspNet.OData.Builder;
//using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
//using Microsoft.OData.Edm;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace DocuStore.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            Bootstrapper = new Bootstrapper();
        }

        public Bootstrapper Bootstrapper { get; }
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureContainer(ServiceRegistry services)
        {
            Bootstrapper.Configuration(services);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "DocuStore API",
                    Description = "A storage based file system API",
                    Contact = new OpenApiContact
                    {
                        Name = "Dusty Lau",
                        Email = "dusty.lau@gmail.com",
                        Url = new Uri("https://www.linkedin.com/in/dusty-lau-a6929718/"),
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Use under GPLv3",
                        Url = new Uri("https://www.gnu.org/licenses/gpl-3.0.en.html"),
                    }
                });
            });

            //services.AddOData();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var fileSystem = app.ApplicationServices.GetService<IFileSystem>();

            fileSystem.RunMigrations().Wait();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "DocStore API");
                c.RoutePrefix = string.Empty;
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                /*
                if (UseODataEdm)
                {
                    endpoints.MapODataRoute("odata", "odata", GetEdmModel());
                }
                else
                {
                    endpoints.EnableDependencyInjection();
                }
                */
            });
        }

        /*
        public bool UseODataEdm { get; set; } = false;

        private static IEdmModel GetEdmModel()
        {
            ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
            builder.EntitySet<IFileSystemItem>("FileSystemItems");
            return builder.GetEdmModel();
        }
        */
    }
}
