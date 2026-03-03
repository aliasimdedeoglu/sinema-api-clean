using CinemaSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace CinemaSystem.Infrastructure.Persistence.Configurations;
public sealed class ShowTimeSeatConfiguration : IEntityTypeConfiguration<ShowTimeSeat>
{
    public void Configure(EntityTypeBuilder<ShowTimeSeat> builder)
    {
        builder.HasKey(sts => sts.Id);
        builder.Property(sts => sts.RowVersion)
            .IsRowVersion()
            .IsConcurrencyToken();
        builder.Property(sts => sts.IsReserved)
            .IsRequired()
            .HasDefaultValue(false);
        builder.HasIndex(sts => new { sts.ShowTimeId, sts.SeatId })
            .IsUnique();
        builder.HasOne(sts => sts.ShowTime)
            .WithMany(st => st.ShowTimeSeats)
            .HasForeignKey(sts => sts.ShowTimeId)
            .OnDelete(DeleteBehavior.Cascade); // ShowTime deleted → ShowTimeSeats deleted
        builder.HasOne(sts => sts.Seat)
            .WithMany()
            .HasForeignKey(sts => sts.SeatId)
            .OnDelete(DeleteBehavior.Restrict); // Seat is structural — don't cascade
    }
}