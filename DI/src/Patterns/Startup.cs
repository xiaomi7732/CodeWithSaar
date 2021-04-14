using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DI.ServiceContainerBasics;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace Patterns
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Patterns", Version = "v1" });
            });

            // Anti pattern: Captive Dependency: DogReport will hold ISerializer & IOutputter
            // services.AddScoped<ISerializer, Serializer1>();
            // services.AddScoped<IOutputter, ConsoleOutputter>();
            // services.AddSingleton<DogReport>();

            // Resolve anti pattern
            services.AddSingleton<ISerializer, Serializer1>();
            // Apply factory pattern to IOutputter:
            services.AddScoped<ConsoleOutputter>();
            services.AddSingleton<ConsoleOutputterFactory>();
            // Update DogReport to rely on ConsoleOutputterFactory
            services.AddSingleton<DogReport>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Patterns v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
