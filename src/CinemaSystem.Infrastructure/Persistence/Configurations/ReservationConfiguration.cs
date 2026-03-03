using CinemaSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace CinemaSystem.Infrastructure.Persistence.Configurations;
public sealed class ShowTimeConfiguration : IEntityTypeConfiguration<ShowTime>
{
    public void Configure(EntityTypeBuilder<ShowTime> builder)
    {
        builder.HasKey(st => st.Id);
        builder.Property(st => st.RowVersion)
            .IsRowVersion()
            .IsConcurrencyToken();
        builder.Property(st => st.TicketPrice)
            .HasColumnType("decimal(10,2)")
            .IsRequired();
        builder.Property(st => st.StartTime)
            .IsRequired();
        builder.Property(st => st.EndTime)
            .IsRequired();
        builder.HasQueryFilter(st => !st.IsDeleted);
        builder.HasIndex(st => new { st.HallId, st.StartTime, st.EndTime });
        builder.HasMany(st => st.Reservations)
            .WithOne(r => r.ShowTime)
            .HasForeignKey(r => r.ShowTimeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
public sealed class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        builder.HasKey(r => r.Id);
        builder.Property(r => r.RowVersion)
            .IsRowVersion()
            .IsConcurrencyToken();
        builder.Property(r => r.Status)
            .HasConversion<string>() // Store enum as string — readable in DB, survives reordering
            .HasMaxLength(20);
        builder.Property(r => r.TotalPrice)
            .HasColumnType("decimal(10,2)");
        builder.HasIndex(r => r.UserId); // Common query: get reservations by user
        builder.HasMany(r => r.Seats)
            .WithOne(rs => rs.Reservation)
            .HasForeignKey(rs => rs.ReservationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
public sealed class ReservationSeatConfiguration : IEntityTypeConfiguration<ReservationSeat>
{
    public void Configure(EntityTypeBuilder<ReservationSeat> builder)
    {
        builder.HasKey(rs => new { rs.ReservationId, rs.SeatId });
        builder.HasOne(rs => rs.Seat)
            .WithMany()
            .HasForeignKey(rs => rs.SeatId)
            .OnDelete(DeleteBehavior.Restrict); // Don't cascade-delete seats when a reservation is cancelled
    }
}