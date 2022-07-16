var builder = WebApplication.CreateBuilder(args);
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// builder.Services.AddCors(opt =>{
//     opt.AddDefaultPolicy( p=>{
//         p.WithOrigins("https://localhost:8081");
//     });
// });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// app.UseCors();

app.UseDefaultFiles(new DefaultFilesOptions
{
    DefaultFileNames = new string[]{
        "index.html",
    },
});
app.UseStaticFiles();

app.UseAuthorization();

app.MapControllers();

app.Run();
