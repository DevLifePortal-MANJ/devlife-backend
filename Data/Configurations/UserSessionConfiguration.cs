using devlife_backend.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace devlife_backend.Data.Configurations
{
    public class UserSessionConfiguration : IEntityTypeConfiguration<UserSession>
    {
        public void Configure(EntityTypeBuilder<UserSession> builder)
        {
            builder.ToTable("user_sessions");

            builder.HasKey(s => s.Id);
            builder.Property(s => s.Id).HasColumnName("id");

            builder.Property(s => s.UserId).HasColumnName("user_id");
            builder.Property(s => s.SessionToken).HasColumnName("session_token");
            builder.Property(s => s.CreatedAt).HasColumnName("created_at");
            builder.Property(s => s.ExpiresAt).HasColumnName("expires_at");
            builder.Property(s => s.IsActive).HasColumnName("is_active");

            builder.HasIndex(s => s.SessionToken).IsUnique();

            builder.Property(s => s.CreatedAt)
                .HasColumnType("timestamp with time zone");
            builder.Property(s => s.ExpiresAt)
                .HasColumnType("timestamp with time zone");
        }
    }
}
