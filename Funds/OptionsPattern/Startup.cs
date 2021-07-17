using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace HelloOptions
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
            // Demo 1, bind the value
            // ApplicationOptions opts = new ApplicationOptions();
            // IConfigurationSection applicationSection = Configuration.GetSection("Application");
            // applicationSection.Bind(opts);
            // Console.WriteLine(opts.Name);

            // Demo 2, shorthands: bind + return
            // ApplicationOptions opts = Configuration
            //     .GetSection(ApplicationOptions.SectionName)
            //     .Get<ApplicationOptions>();
            // Console.WriteLine(opts.Name);

            // Demo 3, register the options to DI

            // Method 1:
            // services.AddOptions<ApplicationOptions>()
            //     .Bind(Configuration.GetSection(ApplicationOptions.SectionName))
            //     .ValidateDataAnnotations();

            // Method 2:
            // services.AddOptions<ApplicationOptions>()
            //     .Configure<IConfiguration, ILogger<ApplicationOptions>>((opt, config, logger) =>
            //         {
            //             config.GetSection(ApplicationOptions.SectionName).Bind(opt);
            //             logger.LogInformation(opt.Name);
            //         });

            // Demo 4, binding to an IEnumerable
            // AnimalInfoOptions opts = Configuration
            //     .GetSection(ApplicationOptions.SectionName)
            //     .GetSection("AnimalInfo")
            //     .Get<AnimalInfoOptions>();
            // foreach(string animal in opts.KnownAnimals)
            // {
            //     Console.WriteLine($"{animal}:{opts.AnimalSpeech[animal]}");
            // }

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "HelloOptions", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "HelloOptions v1"));
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
