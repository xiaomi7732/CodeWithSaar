using JWTAuth.AspNetCore.WebAPI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace AuthExample.ConsumeNuGet
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
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "AuthExample.ConsumeNuGet", Version = "v1" });
            });

            services.AddScoped<UserService>();
            // Adding services to suppport JWT authentication.
            services.AddJWTAuth(opt =>
            {
                opt.OnValidateUserInfo = (jsonPayload, serviceProvider) =>
                    serviceProvider.GetRequiredService<UserService>().ValidUserAsync(jsonPayload);

                opt.OnValidateRoleInfo = (userInfo, serviceProvider) =>
                    serviceProvider.GetRequiredService<UserService>().ValidateRolesAsync(userInfo);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "AuthExample.ConsumeNuGet v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseJWTAuth();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
