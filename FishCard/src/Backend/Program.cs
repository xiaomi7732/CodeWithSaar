using CodeWithSaar.FishCard;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddFishServices();
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

app.UseAuthorization();

app.MapControllers();

app.Run();
