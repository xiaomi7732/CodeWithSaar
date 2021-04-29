using System;
using System.Dynamic;
using System.Linq;
using System.Text.Json;
using JWTAuth.AspNetCore.WebAPI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace JWT.Example.WithSQLDB
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
            services.AddScoped<UserDBContext>();
            services.AddScoped<UserService>();
            services.AddScoped<RoleService>();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "example_sqldb", Version = "v1" });
            });

            services.AddJWTAuth(opt =>
            {
                opt.OnValidateUserInfo = async (loginJson, p) =>
                {
                    UserLogin userLogin = JsonSerializer.Deserialize<UserLogin>(loginJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    UserService userService = p.GetRequiredService<UserService>();
                    User validUser = await userService.GetValidUserAsync(userLogin.UserName, userLogin.Password).ConfigureAwait(false);
                    return new UserInfo(validUser.Name, userLogin);
                };

                opt.OnValidateRoleInfo = async (userInfo, p) =>
                {
                    UserService userService = p.GetRequiredService<UserService>();
                    User theUser = await userService.GetUserByNameAsync(userInfo.Name);
                    return (await userService.GetRoles(theUser).ConfigureAwait(false)).Select(r => r.Name);
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "example_sqldb v1"));
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
