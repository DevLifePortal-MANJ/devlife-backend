// Extensions/AppExtensions.cs - COMPLETE VERSION
using devlife_backend.Data;
using devlife_backend.Models.Request;
using devlife_backend.Services;
using devlife_backend.Hubs;
using Microsoft.EntityFrameworkCore;

namespace devlife_backend.Extensions
{
    public static class AppExtensions
    {
        public static WebApplication ConfigureDevLifeApp(this WebApplication app)
        {
            // Development configuration
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "DevLife Portal API v1.0");
                    c.RoutePrefix = "swagger";
                    c.DocumentTitle = "DevLife Portal API";
                    c.EnableTryItOutByDefault();
                });
            }

            // Core middleware pipeline (order is important!)
            if (!app.Environment.IsDevelopment())
            {
                app.UseHttpsRedirection();
            }

            app.UseSession();
            app.UseRouting();

            // Configure endpoints
            app.ConfigureSystemEndpoints();
            app.ConfigureAuthEndpoints();
            app.ConfigureGameEndpoints();

            // SignalR Hub
            app.MapHub<GameHub>("/gameHub");

            return app;
        }

        private static WebApplication ConfigureSystemEndpoints(this WebApplication app)
        {
            app.MapGet("/", () => Results.Ok(new
            {
                name = "DevLife Portal API",
                version = "1.0.0",
                description = "🎮 Developer Lifestyle Simulator",
                status = "Running",
                timestamp = DateTime.UtcNow,
                games = new[] {
                    "🎰 Code Casino",
                    "🏃 Bug Chase Game",
                    "🔥 Code Roasting",
                    "🔍 Code Analyzer",
                    "💑 Dev Dating Room",
                    "🏃 Meeting Escape"
                }
            }));

            app.MapGet("/health", () => Results.Ok(new
            {
                status = "Healthy",
                timestamp = DateTime.UtcNow,
                version = "1.0.0",
                environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
            }));

            app.MapGet("/dashboard", async (HttpContext context, UserService userService, HoroscopeService horoscopeService) =>
            {
                var userId = context.GetUserId();
                if (userId == null) return Results.Unauthorized();

                try
                {
                    var user = await userService.GetUserByIdAsync(userId.Value);
                    var horoscope = await horoscopeService.GetTodayHoroscopeAsync(user.ZodiacSign);

                    return Results.Ok(new
                    {
                        success = true,
                        data = new
                        {
                            user = new { user.Id, user.Username, user.FirstName, user.ZodiacSign, user.TotalPoints },
                            horoscope,
                            welcomeMessage = $"გამარჯობა {user.FirstName}! 🎮"
                        }
                    });
                }
                catch (Exception ex)
                {
                    return Results.Problem($"Dashboard error: {ex.Message}");
                }
            });

            return app;
        }

        private static WebApplication ConfigureAuthEndpoints(this WebApplication app)
        {
            var auth = app.MapGroup("/auth");

            auth.MapPost("/register", async (RegisterRequest request, UserService userService) =>
            {
                try
                {
                    var result = await userService.RegisterAsync(request);
                    return Results.Ok(new { success = true, data = result });
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest(new { success = false, error = ex.Message });
                }
                catch (Exception ex)
                {
                    return Results.Problem($"Registration failed: {ex.Message}");
                }
            });

            auth.MapPost("/login", async (LoginRequest request, UserService userService, HttpContext context) =>
            {
                try
                {
                    var result = await userService.LoginAsync(request.Username);
                    context.SetUserSession(result.User.Id, result.SessionToken);
                    return Results.Ok(new { success = true, data = result });
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest(new { success = false, error = ex.Message });
                }
                catch (Exception ex)
                {
                    return Results.Problem($"Login failed: {ex.Message}");
                }
            });

            auth.MapPost("/logout", (HttpContext context) =>
            {
                context.Session.Clear();
                return Results.Ok(new { success = true, message = "Logged out successfully" });
            });

            auth.MapGet("/me", async (HttpContext context, UserService userService) =>
            {
                var userId = context.GetUserId();
                if (userId == null) return Results.Unauthorized();

                try
                {
                    var user = await userService.GetUserByIdAsync(userId.Value);
                    return Results.Ok(new
                    {
                        success = true,
                        data = new { user.Id, user.Username, user.FirstName, user.LastName, user.ZodiacSign, user.TotalPoints }
                    });
                }
                catch (Exception ex)
                {
                    return Results.Problem($"Failed to get user: {ex.Message}");
                }
            });

            return app;
        }

        private static WebApplication ConfigureGameEndpoints(this WebApplication app)
        {
            // 🎰 Casino endpoints
            var casino = app.MapGroup("/games/casino").WithTags("🎰 Casino");

            casino.MapGet("/snippets", async (CasinoService casinoService) =>
            {
                try
                {
                    var snippets = await casinoService.GetCodeSnippetsAsync();
                    return Results.Ok(new { success = true, data = snippets });
                }
                catch (Exception ex)
                {
                    return Results.Problem($"Failed to get snippets: {ex.Message}");
                }
            });

            casino.MapPost("/bet", async (CasinoBetRequest request, CasinoService casinoService, HttpContext context) =>
            {
                var userId = context.GetUserId();
                if (userId == null) return Results.Unauthorized();

                try
                {
                    var result = await casinoService.PlaceBetAsync(userId.Value, request);
                    return Results.Ok(new { success = true, data = result });
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest(new { success = false, error = ex.Message });
                }
                catch (Exception ex)
                {
                    return Results.Problem($"Bet failed: {ex.Message}");
                }
            });

            // 🏃 Bug Chase endpoints
            var bugChase = app.MapGroup("/games/bug-chase").WithTags("🏃 Bug Chase");

            bugChase.MapPost("/start", async (BugChaseService bugChaseService, HttpContext context) =>
            {
                var userId = context.GetUserId();
                if (userId == null) return Results.Unauthorized();

                try
                {
                    var session = await bugChaseService.StartGameAsync(userId.Value);
                    return Results.Ok(new { success = true, data = session });
                }
                catch (Exception ex)
                {
                    return Results.Problem($"Failed to start game: {ex.Message}");
                }
            });

            bugChase.MapPost("/end", async (EndGameRequest request, BugChaseService bugChaseService) =>
            {
                try
                {
                    var result = await bugChaseService.EndGameAsync(request);
                    return Results.Ok(new { success = true, data = result });
                }
                catch (Exception ex)
                {
                    return Results.Problem($"Failed to end game: {ex.Message}");
                }
            });

            // 🔥 Code Roasting endpoints
            var roasting = app.MapGroup("/games/roasting").WithTags("🔥 Roasting");

            roasting.MapGet("/challenges", async (CodeRoastingService roastingService) =>
            {
                try
                {
                    var challenges = await roastingService.GetChallengesAsync();
                    return Results.Ok(new { success = true, data = challenges });
                }
                catch (Exception ex)
                {
                    return Results.Problem($"Failed to get challenges: {ex.Message}");
                }
            });

            roasting.MapPost("/submit", async (CodeSubmissionRequest request, CodeRoastingService roastingService, HttpContext context) =>
            {
                var userId = context.GetUserId();
                if (userId == null) return Results.Unauthorized();

                try
                {
                    var result = await roastingService.SubmitCodeAsync(userId.Value, request);
                    return Results.Ok(new { success = true, data = result });
                }
                catch (Exception ex)
                {
                    return Results.Problem($"Code submission failed: {ex.Message}");
                }
            });

            // 🔍 Code Analyzer endpoints - COMPLETE IMPLEMENTATION
            var analyzer = app.MapGroup("/games/analyzer").WithTags("🔍 Code Analyzer");

            analyzer.MapPost("/analyze", async (GitHubAnalysisRequest request, CodeAnalyzerService analyzerService, HttpContext context) =>
            {
                var userId = context.GetUserId();
                if (userId == null) return Results.Unauthorized();

                try
                {
                    var result = await analyzerService.AnalyzeRepositoryAsync(userId.Value, request);
                    return Results.Ok(new { success = true, data = result });
                }
                catch (Exception ex)
                {
                    return Results.Problem($"Analysis failed: {ex.Message}");
                }
            });

            analyzer.MapGet("/sample", async (CodeAnalyzerService analyzerService) =>
            {
                try
                {
                    // Simple sample analysis for demo
                    var sampleRequest = new GitHubAnalysisRequest { Username = "demo", Repository = "sample" };
                    var result = await analyzerService.AnalyzeRepositoryAsync(Guid.NewGuid(), sampleRequest);
                    return Results.Ok(new { success = true, data = result });
                }
                catch (Exception ex)
                {
                    return Results.Problem($"Sample analysis failed: {ex.Message}");
                }
            });

            // 💑 Dating Service endpoints - COMPLETE IMPLEMENTATION
            var dating = app.MapGroup("/games/dating").WithTags("💑 Dating");

            dating.MapGet("/profiles", async (DatingService datingService, HttpContext context) =>
            {
                var userId = context.GetUserId();
                if (userId == null) return Results.Unauthorized();

                try
                {
                    var profiles = await datingService.GetProfilesAsync(userId.Value);
                    return Results.Ok(new { success = true, data = profiles });
                }
                catch (Exception ex)
                {
                    return Results.Problem($"Failed to get profiles: {ex.Message}");
                }
            });

            dating.MapPost("/swipe", async (SwipeRequest request, DatingService datingService, HttpContext context) =>
            {
                var userId = context.GetUserId();
                if (userId == null) return Results.Unauthorized();

                try
                {
                    var result = await datingService.SwipeAsync(userId.Value, request);
                    return Results.Ok(new { success = true, data = result });
                }
                catch (Exception ex)
                {
                    return Results.Problem($"Swipe failed: {ex.Message}");
                }
            });

            // 🏃 Meeting Escape endpoints - COMPLETE IMPLEMENTATION  
            var escape = app.MapGroup("/games/escape").WithTags("🏃 Meeting Escape");

            escape.MapGet("/categories", async (MeetingEscapeService escapeService) =>
            {
                try
                {
                    var categories = await escapeService.GetCategoriesAsync();
                    return Results.Ok(new { success = true, data = categories });
                }
                catch (Exception ex)
                {
                    return Results.Problem($"Failed to get categories: {ex.Message}");
                }
            });

            escape.MapPost("/generate", async (ExcuseRequest request, MeetingEscapeService escapeService) =>
            {
                try
                {
                    var excuse = await escapeService.GenerateExcuseAsync(request);
                    return Results.Ok(new { success = true, data = excuse });
                }
                catch (Exception ex)
                {
                    return Results.Problem($"Failed to generate excuse: {ex.Message}");
                }
            });

            escape.MapGet("/random", async (MeetingEscapeService escapeService) =>
            {
                try
                {
                    var categories = new[] { "technical", "personal", "creative" };
                    var randomCategory = categories[Random.Shared.Next(categories.Length)];
                    var request = new ExcuseRequest { Category = randomCategory };
                    var excuse = await escapeService.GenerateExcuseAsync(request);
                    return Results.Ok(new { success = true, data = excuse });
                }
                catch (Exception ex)
                {
                    return Results.Problem($"Failed to generate random excuse: {ex.Message}");
                }
            });

            Console.WriteLine("✅ All game endpoints configured");
            return app;
        }

        public static async Task<WebApplication> InitializeDatabaseAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DevLifeDbContext>();

            try
            {
                Console.WriteLine("🔄 Checking database connection...");

                var canConnect = await context.Database.CanConnectAsync();
                Console.WriteLine($"🔍 Can connect to database: {canConnect}");

                if (canConnect)
                {
                    var userCount = await context.Users.CountAsync();
                    Console.WriteLine($"✅ Database ready - Users: {userCount}");

                    // Seed MongoDB data
                    var seeder = scope.ServiceProvider.GetRequiredService<SeederService>();
                    await seeder.SeedAllDataAsync();
                }
                else
                {
                    Console.WriteLine("❌ Cannot connect to database. Make sure Docker containers are running.");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Database check failed: {ex.Message}");
                Console.WriteLine($"🔍 Connection: {context.GetConnectionInfo()}");

                if (!app.Environment.IsDevelopment())
                {
                    throw;
                }
            }

            return app;
        }
    }
}