using devlife_backend.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace devlife_backend.Data.Configurations
{
    public class BugChaseGameSessionConfiguration : IEntityTypeConfiguration<BugChaseGameSession>
    {
        public void Configure(EntityTypeBuilder<BugChaseGameSession> builder)
        {
            builder.ToTable("bug_chase_sessions");

            builder.HasKey(b => b.Id);
            builder.Property(b => b.Id).HasColumnName("id");

            builder.Property(b => b.UserId).HasColumnName("user_id");
            builder.Property(b => b.Score).HasColumnName("score");
            builder.Property(b => b.Distance).HasColumnName("distance");
            builder.Property(b => b.BugsCollected).HasColumnName("bugs_collected");
            builder.Property(b => b.PowerUpsUsed).HasColumnName("power_ups_used");
            builder.Property(b => b.Level).HasColumnName("level");
            builder.Property(b => b.IsCompleted).HasColumnName("is_completed");
            builder.Property(b => b.StartedAt).HasColumnName("started_at");
            builder.Property(b => b.CompletedAt).HasColumnName("completed_at");
            builder.Property(b => b.Duration).HasColumnName("duration");

            builder.HasOne(b => b.User)
                .WithMany(u => u.BugChaseSessions)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(b => b.StartedAt)
                .HasColumnType("timestamp with time zone");

            builder.Property(b => b.CompletedAt)
                .HasColumnType("timestamp with time zone");
        }
    }

}
