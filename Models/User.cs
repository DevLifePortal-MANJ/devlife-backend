using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace devlife_backend.Models
{
    [Table("users")]
    public class User
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(50)]
        [Column("username")]
        public string Username { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        [Column("first_name")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        [Column("last_name")]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [Column("birth_date")]
        public DateTime BirthDate { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("zodiac_sign")]
        public string ZodiacSign { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        [Column("tech_stack")]
        public string TechStack { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        [Column("experience_level")]
        public string ExperienceLevel { get; set; } = string.Empty;

        [Required]
        [Column("total_points")]
        public int TotalPoints { get; set; } = 100;

        [MaxLength(100)]
        [Column("session_token")]
        public string? SessionToken { get; set; }

        [Required]
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("last_login")]
        public DateTime? LastLogin { get; set; }

        [Column("last_activity")]
        public DateTime? LastActivity { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [MaxLength(500)]
        [Column("bio")]
        public string? Bio { get; set; }

        [MaxLength(20)]
        [Column("preferred_language")]
        public string PreferredLanguage { get; set; } = "georgian";

        [NotMapped]
        public int TotalGamesPlayed =>
            (CasinoBets?.Count ?? 0) + (BugChaseSessions?.Count ?? 0);

        [NotMapped]
        public DateTime LastGamePlayed =>
            new[] {
                CasinoBets?.LastOrDefault()?.PlacedAt,
                BugChaseSessions?.LastOrDefault()?.StartedAt
            }.Where(d => d.HasValue).DefaultIfEmpty(CreatedAt).Max() ?? CreatedAt;

        public virtual ICollection<CasinoBet> CasinoBets { get; set; } = new List<CasinoBet>();
        public virtual ICollection<BugChaseGameSession> BugChaseSessions { get; set; } = new List<BugChaseGameSession>();

        public int CalculateAge()
        {
            var today = DateTime.Today;
            var age = today.Year - BirthDate.Year;
            if (BirthDate.Date > today.AddYears(-age)) age--;
            return age;
        }

        public bool HasPlayedToday()
        {
            return LastActivity?.Date == DateTime.Today;
        }

        public void UpdateActivity()
        {
            LastActivity = DateTime.UtcNow;
        }

        public void AddPoints(int points, string reason = "")
        {
            TotalPoints += points;
            UpdateActivity();
        }

        public bool CanAfford(int points)
        {
            return TotalPoints >= points;
        }
    }
}