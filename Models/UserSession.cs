using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace devlife_backend.Models
{
    [Table("user_sessions")]
    public class UserSession
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid UserId { get; set; }

        [Required]
        [MaxLength(100)]
        public string SessionToken { get; set; } = string.Empty;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("expires_at")]
        public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddDays(30);

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        public virtual User User { get; set; } = null!;
    }
}