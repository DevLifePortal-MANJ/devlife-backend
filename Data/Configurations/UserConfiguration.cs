using devlife_backend.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace devlife_backend.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("users");

            builder.HasKey(u => u.Id);
            builder.Property(u => u.Id).HasColumnName("id");

            builder.Property(u => u.Username)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnName("username");

            builder.Property(u => u.FirstName)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("first_name");

            builder.Property(u => u.LastName)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("last_name");

            builder.Property(u => u.BirthDate)
                .HasColumnName("birth_date");

            builder.Property(u => u.ZodiacSign)
                .IsRequired()
                .HasMaxLength(20)
                .HasColumnName("zodiac_sign");

            builder.Property(u => u.TechStack)
                .IsRequired()
                .HasMaxLength(200)
                .HasColumnName("tech_stack");

            builder.Property(u => u.ExperienceLevel)
                .IsRequired()
                .HasMaxLength(20)
                .HasColumnName("experience_level");

            builder.Property(u => u.TotalPoints)
                .HasColumnName("total_points");

            builder.Property(u => u.SessionToken)
                .HasColumnName("session_token");

            builder.Property(u => u.Bio)
                .HasColumnName("bio");

            builder.Property(u => u.PreferredLanguage)
                .HasColumnName("preferred_language");

            builder.Property(u => u.IsActive)
                .HasColumnName("is_active");

            builder.Property(u => u.CreatedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("created_at");

            builder.Property(u => u.LastLogin)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("last_login");

            builder.Property(u => u.LastActivity)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("last_activity");

            builder.HasIndex(u => u.Username)
                .IsUnique();
        }
    }
}
