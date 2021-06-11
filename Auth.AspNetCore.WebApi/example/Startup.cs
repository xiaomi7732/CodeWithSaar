using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace QuickStart.WebAPI
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

            services.AddScoped<UserService>();
            // Add services to support JWT authentication / authorization.
            services.AddJWTAuth(opt =>
            {
                opt.OnValidateUserInfo = async (jsonBody, p) =>
                {
                    UserService userService = p.GetRequiredService<UserService>();
                    return await userService.CreateValidUserAsync(jsonBody).ConfigureAwait(false);
                };

                opt.OnValidateRoleInfo = async (userInfo, p) =>
                 {
                     UserService userService = p.GetRequiredService<UserService>();
                     return await userService.ValidateRolesAsync(userInfo.Name).ConfigureAwait(false);
                 };

                // Optional, to support SignalR authentication
                // opt.OnJWTAuthenticationMessageReceived = context =>
                // {
                //     var accessToken = context.Request.Query["access_token"];

                //     // If the request is for our hub...
                //     var path = context.HttpContext.Request.Path;
                //     if (!string.IsNullOrEmpty(accessToken) &&
                //         // Assuming the folling hub route:
                //         (path.StartsWithSegments("/chathub")))
                //     {
                //         // Read the token out of the query string
                //         context.Token = accessToken;
                //     }
                //     return Task.CompletedTask;
                // };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            // Adding JWT authorization into the pipeline
            app.UseJWTAuth();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
