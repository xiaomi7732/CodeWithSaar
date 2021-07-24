using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using JWTAuth.AspNetCore.WebAPI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace JWTAuthLib
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
            services.AddJWTAuth(opt =>
            {
                opt.IssuerSigningSecret = "xxxx";
                opt.OnValidateUserInfo = (jsonBody, p) =>
                {
                    DefaultUserLogin login = JsonSerializer.Deserialize<DefaultUserLogin>(jsonBody,
                        new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

                    // TODO: verify the password is valid.
                    UserInfo user = new UserInfo(login.Username, login);
                    return Task.FromResult(user);
                };

                opt.OnValidateRoleInfo = (validUser, p) =>
                {
                    return Task.FromResult<IEnumerable<string>>(new string[] { validUser.Name });
                };
            }, "AnotherSectionName");
            services.AddControllers();
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

            app.UseJWTAuth();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
