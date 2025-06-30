using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace devlife_backend.Models
{
    [Table("bug_chase_sessions")]
    public class BugChaseGameSession
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid UserId { get; set; }

        [Column("score")]
        public int Score { get; set; } = 0;

        [Column("distance")]
        public int Distance { get; set; } = 0;

        [Column("bugs_collected")]
        public int BugsCollected { get; set; } = 0;

        [Column("power_ups_used")]
        public int PowerUpsUsed { get; set; } = 0;

        [Column("level")]
        public int Level { get; set; } = 1;

        [Column("is_completed")]
        public bool IsCompleted { get; set; } = false;

        [Column("started_at")]
        public DateTime StartedAt { get; set; } = DateTime.UtcNow;

        [Column("completed_at")]
        public DateTime? CompletedAt { get; set; }

        [Column("duration")]
        public TimeSpan Duration { get; set; } = TimeSpan.Zero;

        public virtual User User { get; set; } = null!;
    }
}