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

namespace AntiPatterns
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
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "AntiPatterns", Version = "v1" });
            });

            // Anti-pattern #1, register a singleton service not created by service container
            // services.AddSingleton<IOutputter, ConsoleOutputter>();
            // services.AddSingleton<IOutputter>(p => new ConsoleOutputter());
            // services.AddSingleton<IOutputter>(new ConsoleOutputter());

            // Anti-pattern #2, captive dependency
            // services.AddScoped<IOutputter, ConsoleOutputter>();
            // services.AddScoped<ISerializer, Serializer1>();
            // services.AddSingleton<DogReport>();

            // Anti-pattern #3, use async in implementation factory
            // services.AddScoped<IOutputter, ConsoleOutputter>();
            // services.AddScoped<ISerializer, Serializer1>();
            // services.AddScoped<DogReport>(p =>
            // {
            //     Console.WriteLine("Before async operation ...");
            //     ISerializer s = WaitForItAsync(p).Result;
            //     Console.WriteLine("Post async operation...");
            //     return new DogReport(p.GetRequiredService<ISerializer>(), p.GetRequiredService<IOutputter>());
            // });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "AntiPatterns v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        // private async Task<ISerializer> WaitForItAsync(IServiceProvider p)
        // {
        //     await Task.Delay(1000).ConfigureAwait(false);
        //     return p.GetRequiredService<ISerializer>();
        // }
    }
}
