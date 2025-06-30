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
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                app.UseSwagger(c =>
                {
                    c.RouteTemplate = "swagger/{documentName}/swagger.json";
                });

                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "DevLife Portal API v1.0");
                    c.RoutePrefix = "swagger";
                    c.DocumentTitle = "🎮 DevLife Portal API Documentation";

                    c.DefaultModelsExpandDepth(-1); 
                    c.DefaultModelExpandDepth(2);
                    c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
                    c.EnableDeepLinking();
                    c.EnableFilter();
                    c.EnableTryItOutByDefault();
                    c.DisplayRequestDuration();
                });
            }
            
            if (!app.Environment.IsDevelopment())
            {
                app.UseHttpsRedirection();
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseCors("DevLifePolicy");

            app.UseSession();

            app.ConfigureSystemEndpoints();
            app.ConfigureAuthEndpoints();
            app.ConfigureGameEndpoints();

            app.MapHub<GameHub>("/gameHub", options =>
            {
                options.AllowStatefulReconnects = false;
                options.TransportMaxBufferSize = 64 * 1024;
                options.ApplicationMaxBufferSize = 64 * 1024;
            });

            app.MapGet("/signalr/test", () => Results.Ok(new
            {
                message = "SignalR is configured!",
                hubEndpoint = "/gameHub",
                negotiateEndpoint = "/gameHub/negotiate",
                timestamp = DateTime.UtcNow,
                instructions = new[]
                {
                    "1. Connect to ws://localhost:5000/gameHub",
                    "2. Call 'Ping' method to test connection",
                    "3. Listen for 'Pong' response"
                }
            }))
            .WithTags("SignalR")
            .WithName("SignalRTest")
            .WithSummary("Test SignalR Configuration");

            Console.WriteLine("✅ SignalR Hub mapped to /gameHub");
            Console.WriteLine("🔗 SignalR test endpoint: /signalr/test");
            Console.WriteLine("📚 Swagger documentation: /swagger");

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
                documentation = "/swagger",
                health = "/health",
                games = new[] {
                    "🎰 Code Casino - /games/casino/*",
                    "🏃 Bug Chase Game - /games/bug-chase/*",
                    "🔥 Code Roasting - /games/roasting/*",
                    "🔍 Code Analyzer - /games/analyzer/*",
                    "💑 Dev Dating Room - /games/dating/*",
                    "🏃 Meeting Escape - /games/escape/*"
                },
                authentication = new[] {
                    "POST /auth/register - Register new user",
                    "POST /auth/login - Login with username",
                    "GET /auth/me - Get current user",
                    "POST /auth/logout - Logout"
                }
            }))
            .WithTags("System")
            .WithName("GetApiInfo")
            .WithSummary("API Information")
            .WithDescription("Get comprehensive information about the DevLife Portal API");

            app.MapGet("/health", () => Results.Ok(new
            {
                status = "Healthy",
                timestamp = DateTime.UtcNow,
                version = "1.0.0",
                environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
                uptime = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss UTC")
            }))
            .WithTags("System")
            .WithName("HealthCheck")
            .WithSummary("Health Check")
            .WithDescription("Check if the API is running and healthy");

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
                            welcomeMessage = $"გამარჯობა {user.FirstName}! 🎮",
                            gamesAvailable = 6,
                            lastActivity = user.LastActivity
                        }
                    });
                }
                catch (Exception ex)
                {
                    return Results.Problem($"Dashboard error: {ex.Message}");
                }
            })
            .WithTags("Dashboard")
            .WithName("GetDashboard")
            .WithSummary("User Dashboard")
            .WithDescription("Get user dashboard with horoscope, points, and welcome message");

            return app;
        }

        private static WebApplication ConfigureAuthEndpoints(this WebApplication app)
        {
            var auth = app.MapGroup("/auth").WithTags("🔐 Authentication");

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
            })
            .WithName("RegisterUser")
            .WithSummary("Register New User")
            .WithDescription("Register a new user with automatic zodiac sign calculation based on birth date");

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
            })
            .WithName("LoginUser")
            .WithSummary("User Login")
            .WithDescription("Login with username (no password required) - creates session-based authentication");

            auth.MapPost("/logout", (HttpContext context) =>
            {
                context.Session.Clear();
                return Results.Ok(new { success = true, message = "Logged out successfully" });
            })
            .WithName("LogoutUser")
            .WithSummary("User Logout")
            .WithDescription("Logout current user and clear session");

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
                        data = new
                        {
                            user.Id,
                            user.Username,
                            user.FirstName,
                            user.LastName,
                            user.ZodiacSign,
                            user.TotalPoints,
                            user.ExperienceLevel,
                            user.TechStack,
                            user.CreatedAt,
                            user.LastLogin
                        }
                    });
                }
                catch (Exception ex)
                {
                    return Results.Problem($"Failed to get user: {ex.Message}");
                }
            })
            .WithName("GetCurrentUser")
            .WithSummary("Get Current User")
            .WithDescription("Get detailed information about the currently authenticated user");

            return app;
        }

        private static WebApplication ConfigureGameEndpoints(this WebApplication app)
        {
            var casino = app.MapGroup("/games/casino").WithTags("🎰 Code Casino");

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
            })
            .WithName("GetCasinoSnippets")
            .WithSummary("Get Code Snippets for Betting")
            .WithDescription("Get two code snippets - one correct, one buggy. Players bet on which is correct.");

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
            })
            .WithName("PlaceCasinoBet")
            .WithSummary("Place Casino Bet")
            .WithDescription("Bet points on code snippets. Win 2x if correct, lose bet if wrong. Includes zodiac luck multiplier!");

            var bugChase = app.MapGroup("/games/bug-chase").WithTags("🏃 Bug Chase Game");

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
            })
            .WithName("StartBugChaseGame")
            .WithSummary("Start Bug Chase Game")
            .WithDescription("Start a new Bug Chase endless runner game session. Returns session ID for tracking.");

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
            })
            .WithName("EndBugChaseGame")
            .WithSummary("End Bug Chase Game")
            .WithDescription("End Bug Chase game session and save final score, distance, and bugs collected.");

            var roasting = app.MapGroup("/games/roasting").WithTags("🔥 Code Roasting");

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
            })
            .WithName("GetRoastingChallenges")
            .WithSummary("Get Coding Challenges")
            .WithDescription("Get available coding challenges for submission and roasting (FizzBuzz, Palindrome, etc.)");

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
            })
            .WithName("SubmitCodeForRoasting")
            .WithSummary("Submit Code for AI Roasting")
            .WithDescription("Submit your code solution and get hilariously brutal (but constructive) AI feedback in Georgian!");

            var analyzer = app.MapGroup("/games/analyzer").WithTags("🔍 Code Personality Analyzer");

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
            })
            .WithName("AnalyzeGitHubRepo")
            .WithSummary("Analyze GitHub Repository")
            .WithDescription("Analyze GitHub repository to determine developer personality type (Chaotic Debugger, Architecture Wizard, etc.)");

            analyzer.MapGet("/sample", async (CodeAnalyzerService analyzerService) =>
            {
                try
                {
                    var sampleRequest = new GitHubAnalysisRequest { Username = "demo", Repository = "sample" };
                    var result = await analyzerService.AnalyzeRepositoryAsync(Guid.NewGuid(), sampleRequest);
                    return Results.Ok(new { success = true, data = result });
                }
                catch (Exception ex)
                {
                    return Results.Problem($"Sample analysis failed: {ex.Message}");
                }
            })
            .WithName("GetSampleAnalysis")
            .WithSummary("Sample Code Analysis")
            .WithDescription("Get sample code analysis result to see what the analyzer produces");

            var dating = app.MapGroup("/games/dating").WithTags("💑 Dev Dating Room");

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
            })
            .WithName("GetDatingProfiles")
            .WithSummary("Get Developer Dating Profiles")
            .WithDescription("Get developer profiles for swiping - includes tech stack, experience level, and zodiac compatibility");

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
            })
            .WithName("SwipeOnProfile")
            .WithSummary("Swipe Left or Right")
            .WithDescription("Swipe on developer profiles - like or dislike based on tech compatibility and zodiac signs");

            var escape = app.MapGroup("/games/escape").WithTags("🏃 Meeting Escape Generator");

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
            })
            .WithName("GetExcuseCategories")
            .WithSummary("Get Excuse Categories")
            .WithDescription("Get available categories for meeting excuses (technical, personal, creative)");

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
            })
            .WithName("GenerateCustomExcuse")
            .WithSummary("Generate Custom Meeting Excuse")
            .WithDescription("Generate creative excuse for escaping boring meetings - includes believability score!");

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
            })
            .WithName("GenerateRandomExcuse")
            .WithSummary("Generate Random Meeting Excuse")
            .WithDescription("Generate completely random excuse from any category - perfect for emergency escapes!");

            Console.WriteLine("✅ All game endpoints configured with comprehensive Swagger documentation");
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