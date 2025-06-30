using devlife_backend.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace devlife_backend.Data.Configurations
{
    public class CasinoBetConfiguration : IEntityTypeConfiguration<CasinoBet>
    {
        public void Configure(EntityTypeBuilder<CasinoBet> builder)
        {
            builder.ToTable("casino_bets");

            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).HasColumnName("id");

            builder.Property(c => c.UserId).HasColumnName("user_id");
            builder.Property(c => c.BetAmount).HasColumnName("bet_amount");
            builder.Property(c => c.ChosenOption).HasColumnName("chosen_option");
            builder.Property(c => c.CorrectOption).HasColumnName("correct_option");
            builder.Property(c => c.IsWin).HasColumnName("is_win");
            builder.Property(c => c.PointsChange).HasColumnName("points_change");
            builder.Property(c => c.SnippetId).HasColumnName("snippet_id");
            builder.Property(c => c.TechStack).HasColumnName("tech_stack");
            builder.Property(c => c.PlacedAt).HasColumnName("placed_at");
            builder.Property(c => c.ZodiacSign).HasColumnName("zodiac_sign");
            builder.Property(c => c.LuckMultiplier).HasColumnName("luck_multiplier");

            builder.HasOne(c => c.User)
                .WithMany(u => u.CasinoBets)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(c => c.PlacedAt)
                .HasColumnType("timestamp with time zone");
        }
    }

}