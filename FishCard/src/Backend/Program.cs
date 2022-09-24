using CodeWithSaar.FishCard;
using CodeWithSaar.FishCard.Auth;
using CodeWithSaar.FishCard.DataAccess;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddFishServices();
builder.Services.TryAddFishcardAuthServices();
builder.Services.AddDbContext<UserManagerContext>((p, builder) =>
{
    UserManagerDBOption options = p.GetRequiredService<IOptions<UserManagerDBOption>>().Value;
    builder.UseSqlite(options.ConnectionString);
});

builder.Services.ConfigureOptions<ConfigureJWTBearerOptions>();
builder.Services.AddAuthentication(opt =>
{
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultForbidScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer();


builder.Services.AddCors(config => config.AddDefaultPolicy(policy =>
{
    policy.WithOrigins(new string[]{
        "https://localhost:7015",  // Local debugging
        "https://fishcard.codewithsaar.net", // Hosted on github
    });
}));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
