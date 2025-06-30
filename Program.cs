using devlife_backend.Extensions;
using devlife_backend.Hubs;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

Console.WriteLine("🚀 DevLife Portal starting...");
Console.WriteLine($"🌍 Environment: {builder.Environment.EnvironmentName}");

// 🚀 TEMPORARY CORS FIX - Add this before other services
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddAllDevLifeServices(builder.Configuration);

var app = builder.Build();

// 🚀 ENABLE CORS FIRST
app.UseCors();

app.ConfigureDevLifeApp();

await app.InitializeDatabaseAsync();

Console.WriteLine("✅ DevLife Portal ready!");
Console.WriteLine("📍 API: http://localhost:5000");
Console.WriteLine("📚 Swagger: http://localhost:5000/swagger");
Console.WriteLine("🔌 SignalR Hub: ws://localhost:5000/gameHub");

app.Run();