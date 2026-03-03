using CinemaSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace CinemaSystem.Infrastructure.Persistence.Configurations;
public sealed class MovieConfiguration : IEntityTypeConfiguration<Movie>
{
    public void Configure(EntityTypeBuilder<Movie> builder)
    {
        builder.HasKey(m => m.Id);
        builder.Property(m => m.RowVersion)
            .IsRowVersion() // Maps to SQL Server ROWVERSION column — auto-incremented by DB
            .IsConcurrencyToken(); // EF adds WHERE RowVersion = @version to all UPDATEs
        builder.Property(m => m.Title)
            .IsRequired()
            .HasMaxLength(200);
        builder.Property(m => m.Description)
            .HasMaxLength(2000);
        builder.Property(m => m.Genre)
            .IsRequired()
            .HasMaxLength(100);
        builder.HasQueryFilter(m => !m.IsDeleted);
        builder.HasIndex(m => m.Title);
        builder.HasMany(m => m.ShowTimes)
            .WithOne(st => st.Movie)
            .HasForeignKey(st => st.MovieId)
            .OnDelete(DeleteBehavior.Restrict); // Prevent accidental cascade delete
    }
}