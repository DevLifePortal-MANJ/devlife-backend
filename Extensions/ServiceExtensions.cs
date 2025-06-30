using devlife_backend.Data;
using devlife_backend.Services;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using StackExchange.Redis;
using System.Text.Json;
using Microsoft.OpenApi.Models;

namespace devlife_backend.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddAllDevLifeServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            return services
                .AddApiServices(configuration)
                .AddDatabaseServices(configuration)
                .AddBusinessServices()
                .AddExternalServices(configuration)
                .AddInfrastructureServices(configuration);
        }

        #region API & Core Services
        private static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.WriteIndented = true;
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                options.JsonSerializerOptions.AllowTrailingCommas = true;
            });

            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "DevLife Portal API",
                    Version = "1.0.0",
                    Description = "🎮 Developer Lifestyle Simulator"
                });

                c.AddServer(new OpenApiServer
                {
                    Url = "http://localhost:5000",
                    Description = "Development Server"
                });

                c.EnableAnnotations();
            });

            Console.WriteLine("✅ API services configured");
            return services;
        }
        #endregion

        #region Database Services  
        private static IServiceCollection AddDatabaseServices(this IServiceCollection services, IConfiguration configuration)
        {
            var isDocker = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true" ||
                          Environment.GetEnvironmentVariable("DEPLOYMENT_MODE")?.ToLower() == "docker";

            // Use configuration instead of environment variables for reliability
            var postgresConnection = isDocker
                ? "Host=postgres;Port=5432;Database=devlife;Username=devlife_user;Password=devlife_password;"
                : configuration.GetConnectionString("DefaultConnection") ??
                  "Host=localhost;Port=6100;Database=devlife;Username=devlife_user;Password=devlife_password;";

            Console.WriteLine($"🔍 Using connection: {postgresConnection.Replace("Password=devlife_password", "Password=***")}");

            services.AddDbContext<DevLifeDbContext>(options =>
            {
                options.UseNpgsql(postgresConnection, npgsqlOptions =>
                {
                    npgsqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(10),
                        errorCodesToAdd: null);
                });

                // 🚀 FIX: Use snake_case naming to match your SQL scripts
                options.UseSnakeCaseNamingConvention();

                if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
                {
                    options.EnableSensitiveDataLogging();
                    options.EnableDetailedErrors();
                }
            });

            // MongoDB configuration stays the same...
            var mongoConnection = isDocker
                ? "mongodb://admin:admin_password@mongodb:27017/devlife?authSource=admin"
                : configuration.GetConnectionString("MongoDB") ??
                  "mongodb://admin:admin_password@localhost:27017/devlife?authSource=admin";

            services.AddSingleton<IMongoClient>(sp =>
            {
                try
                {
                    var client = new MongoClient(mongoConnection);
                    client.GetDatabase("devlife").RunCommand<object>("{ping:1}");
                    Console.WriteLine("✅ MongoDB connected successfully");
                    return client;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ MongoDB connection failed: {ex.Message}");
                    throw;
                }
            });

            services.AddScoped<IMongoDatabase>(sp =>
                sp.GetRequiredService<IMongoClient>().GetDatabase("devlife"));

            // Redis configuration stays the same...
            var redisConnection = isDocker ? "redis:6379" : configuration.GetConnectionString("Redis") ?? "localhost:6200";
            try
            {
                services.AddSingleton<IConnectionMultiplexer>(sp =>
                {
                    var config = ConfigurationOptions.Parse(redisConnection);
                    config.Password = "devlife_password";
                    config.AbortOnConnectFail = false;
                    config.ConnectRetry = 3;
                    return ConnectionMultiplexer.Connect(config);
                });
                Console.WriteLine("✅ Redis configured");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Redis unavailable: {ex.Message} - using memory cache");
                services.AddSingleton<IConnectionMultiplexer?>(sp => null);
            }

            Console.WriteLine($"✅ Database services configured (Docker: {isDocker})");
            return services;
        }
        #endregion

        #region Infrastructure Services
        private static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Session configuration
            var sessionTimeout = int.TryParse(
                Environment.GetEnvironmentVariable("SESSION_TIMEOUT_MINUTES"),
                out var timeout) ? timeout : 30;

            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(sessionTimeout);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.Cookie.Name = "DevLifeSession";
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            });

            // 🚀 FIXED CORS CONFIGURATION
            services.AddCors(options =>
            {
                // Production policy - specific origins with credentials
                options.AddPolicy("DevLifePolicy", policy =>
                {
                    policy.WithOrigins(
                            "http://localhost:4200",
                            "https://localhost:4200",
                            "http://localhost:3000",
                            "https://localhost:3000",
                            "http://localhost:5000",
                            "https://localhost:5000"
                          )
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials();
                });

                // Development policy - specific origins WITHOUT credentials for Swagger
                options.AddPolicy("Development", policy =>
                {
                    policy.WithOrigins(
                            "http://localhost:4200",
                            "https://localhost:4200",
                            "http://localhost:3000",
                            "https://localhost:3000",
                            "http://localhost:5000",
                            "https://localhost:5000",
                            "http://127.0.0.1:5000",
                            "https://127.0.0.1:5000"
                          )
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                    // NOTE: Removed AllowCredentials() to fix CORS issue
                });

                // Swagger-specific policy - very permissive for development
                options.AddPolicy("Swagger", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                    // NOTE: Cannot use AllowCredentials() with AllowAnyOrigin()
                });
            });

            services.AddSignalR(options =>
            {
                options.EnableDetailedErrors = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
                options.KeepAliveInterval = TimeSpan.FromSeconds(15);
                options.ClientTimeoutInterval = TimeSpan.FromSeconds(60);
            });

            Console.WriteLine($"✅ Infrastructure services configured - Session timeout: {sessionTimeout}min");
            return services;
        }
        #endregion

        #region External Services
        private static IServiceCollection AddExternalServices(this IServiceCollection services, IConfiguration configuration)
        {
            var githubClientId = Environment.GetEnvironmentVariable("GITHUB_CLIENT_ID");
            if (!string.IsNullOrEmpty(githubClientId))
            {
                services.AddHttpClient("GitHub", client =>
                {
                    client.BaseAddress = new Uri("https://api.github.com/");
                    client.DefaultRequestHeaders.Add("User-Agent", "DevLife-Portal/1.0");
                    client.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json");
                });
                Console.WriteLine("✅ GitHub API client configured");
            }

            var openAiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
            if (!string.IsNullOrEmpty(openAiKey))
            {
                services.AddHttpClient("OpenAI", client =>
                {
                    client.BaseAddress = new Uri("https://api.openai.com/v1/");
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {openAiKey}");
                });
                Console.WriteLine("✅ OpenAI API client configured");
            }

            services.AddHttpClient("General", client =>
            {
                client.Timeout = TimeSpan.FromSeconds(30);
                client.DefaultRequestHeaders.Add("User-Agent", "DevLife-Portal/1.0");
            });

            return services;
        }
        #endregion

        #region Business Services
        private static IServiceCollection AddBusinessServices(this IServiceCollection services)
        {
            services.AddScoped<SeederService>();
            services.AddScoped<UserService>();
            services.AddScoped<HoroscopeService>();
            services.AddScoped<CasinoService>();
            services.AddScoped<BugChaseService>();
            services.AddScoped<CodeRoastingService>();
            services.AddScoped<CodeAnalyzerService>();
            services.AddScoped<DatingService>();
            services.AddScoped<MeetingEscapeService>();

            Console.WriteLine("✅ Business services registered");
            return services;
        }
        #endregion
    }
}