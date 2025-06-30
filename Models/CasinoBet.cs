using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace devlife_backend.Models
{
    [Table("casino_bets")]
    public class CasinoBet
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public int BetAmount { get; set; }

        [Required]
        public int ChosenOption { get; set; }

        [Required]
        public int CorrectOption { get; set; }

        [Required]
        public bool IsWin { get; set; }

        [Required]
        public int PointsChange { get; set; }

        [Required]
        [MaxLength(50)]
        public string SnippetId { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string TechStack { get; set; } = string.Empty;

        [Column("placed_at")]
        public DateTime PlacedAt { get; set; } = DateTime.UtcNow;

        [Column("zodiac_sign")]
        public string ZodiacSign { get; set; } = string.Empty;

        [Column("luck_multiplier")]
        public double LuckMultiplier { get; set; } = 1.0;

        public virtual User User { get; set; } = null!;
    }
}