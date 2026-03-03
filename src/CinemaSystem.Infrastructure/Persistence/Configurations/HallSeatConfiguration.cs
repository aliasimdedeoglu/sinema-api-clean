using CinemaSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace CinemaSystem.Infrastructure.Persistence.Configurations;
public sealed class CinemaHallConfiguration : IEntityTypeConfiguration<CinemaHall>
{
    public void Configure(EntityTypeBuilder<CinemaHall> builder)
    {
        builder.HasKey(h => h.Id);
        builder.Property(h => h.RowVersion)
            .IsRowVersion()
            .IsConcurrencyToken();
        builder.Property(h => h.Name)
            .IsRequired()
            .HasMaxLength(100);
        builder.HasQueryFilter(h => !h.IsDeleted);
        builder.HasMany(h => h.Seats)
            .WithOne(s => s.Hall)
            .HasForeignKey(s => s.HallId)
            .OnDelete(DeleteBehavior.Cascade); // Seats are owned by hall
        builder.HasMany(h => h.ShowTimes)
            .WithOne(st => st.Hall)
            .HasForeignKey(st => st.HallId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
public sealed class SeatConfiguration : IEntityTypeConfiguration<Seat>
{
    public void Configure(EntityTypeBuilder<Seat> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.RowVersion)
            .IsRowVersion()
            .IsConcurrencyToken();
        builder.Property(s => s.Row)
            .IsRequired()
            .HasMaxLength(5);
        builder.HasIndex(s => new { s.HallId, s.Row, s.Number }).IsUnique();
    }
}