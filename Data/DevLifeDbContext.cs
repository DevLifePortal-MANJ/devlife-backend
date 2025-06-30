using devlife_backend.Data.Configurations;
using devlife_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace devlife_backend.Data
{
    public class DevLifeDbContext : DbContext
    {
        public DevLifeDbContext(DbContextOptions<DevLifeDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<UserSession> UserSessions { get; set; }
        public DbSet<CasinoBet> CasinoBets { get; set; }
        public DbSet<BugChaseGameSession> BugChaseSessions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply all entity configurations
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new UserSessionConfiguration());
            modelBuilder.ApplyConfiguration(new CasinoBetConfiguration());
            modelBuilder.ApplyConfiguration(new BugChaseGameSessionConfiguration());

            ConfigureGlobalSettings(modelBuilder);
            SeedInitialData(modelBuilder);
        }

        private static void ConfigureGlobalSettings(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                    {
                        if (property.GetColumnType() == null)
                        {
                            property.SetColumnType("timestamp with time zone");
                        }
                    }

                    if (property.ClrType == typeof(decimal) || property.ClrType == typeof(decimal?))
                    {
                        if (property.GetColumnType() == null)
                        {
                            property.SetColumnType("decimal(18,2)");
                        }
                    }
                }
            }
        }

        private static void SeedInitialData(ModelBuilder modelBuilder)
        {
            var demoUsers = new[]
            {
                new User
                {
                    Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    Username = "demo_user",
                    FirstName = "Demo",
                    LastName = "User",
                    BirthDate = new DateTime(1995, 6, 15),
                    ZodiacSign = "Gemini", // English zodiac sign
                    TechStack = "React, JavaScript, Node.js",
                    ExperienceLevel = "Middle",
                    TotalPoints = 1000,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true,
                    Bio = "Demo user for testing DevLife Portal games",
                    PreferredLanguage = "georgian"
                },
                new User
                {
                    Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    Username = "leo_dev",
                    FirstName = "Leo",
                    LastName = "Developer",
                    BirthDate = new DateTime(1992, 8, 15),
                    ZodiacSign = "Leo", // English zodiac sign
                    TechStack = "Python, Django, PostgreSQL",
                    ExperienceLevel = "Senior",
                    TotalPoints = 1500,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true,
                    Bio = "Senior Python developer with Leo confidence",
                    PreferredLanguage = "georgian"
                },
                new User
                {
                    Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                    Username = "scorpio_coder",
                    FirstName = "Ana",
                    LastName = "Coder",
                    BirthDate = new DateTime(1988, 11, 5),
                    ZodiacSign = "Scorpio", // English zodiac sign
                    TechStack = "C#, .NET, Azure",
                    ExperienceLevel = "Senior",
                    TotalPoints = 2000,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true,
                    Bio = "Intense Scorpio developer with .NET expertise",
                    PreferredLanguage = "georgian"
                }
            };

            modelBuilder.Entity<User>().HasData(demoUsers);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var userEntries = ChangeTracker.Entries<User>()
                .Where(e => e.State == EntityState.Modified);

            foreach (var entry in userEntries)
            {
                entry.Entity.LastActivity = DateTime.UtcNow;
            }

            var bugChaseEntries = ChangeTracker.Entries<BugChaseGameSession>()
                .Where(e => e.State == EntityState.Modified &&
                           e.Entity.IsCompleted &&
                           e.Entity.CompletedAt == null);

            foreach (var entry in bugChaseEntries)
            {
                entry.Entity.CompletedAt = DateTime.UtcNow;
                entry.Entity.Duration = DateTime.UtcNow - entry.Entity.StartedAt;
            }

            return await base.SaveChangesAsync(cancellationToken);
        }

        public string GetConnectionInfo()
        {
            return Database.GetConnectionString()?.Replace("Password=", "Password=***") ?? "No connection string";
        }

        public async Task<bool> DatabaseExistsAsync()
        {
            try
            {
                return await Database.CanConnectAsync();
            }
            catch
            {
                return false;
            }
        }

        public async Task<Dictionary<string, object>> GetDatabaseHealthAsync()
        {
            var health = new Dictionary<string, object>();

            try
            {
                health["CanConnect"] = await Database.CanConnectAsync();
                health["UserCount"] = await Users.CountAsync();
                health["CasinoBetCount"] = await CasinoBets.CountAsync();
                health["BugChaseSessionCount"] = await BugChaseSessions.CountAsync();

                var connectionInfo = Database.GetConnectionString();
                if (!string.IsNullOrEmpty(connectionInfo))
                {
                    var parts = connectionInfo.Split(';');
                    var hostPart = parts.FirstOrDefault(p => p.StartsWith("Host="));
                    var dbPart = parts.FirstOrDefault(p => p.StartsWith("Database="));

                    health["Host"] = hostPart?.Split('=')[1] ?? "Unknown";
                    health["DatabaseName"] = dbPart?.Split('=')[1] ?? "Unknown";
                }

                health["Status"] = "Healthy";
            }
            catch (Exception ex)
            {
                health["Status"] = "Unhealthy";
                health["Error"] = ex.Message;
            }

            return health;
        }
    }
}