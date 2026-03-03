using CinemaSystem.Domain.Common;
namespace CinemaSystem.Domain.Entities;
public sealed class Seat : BaseEntity
{
    private Seat() { }
    public Guid HallId { get; private set; }
    public string Row { get; private set; } = default!;
    public int Number { get; private set; }
    public CinemaHall Hall { get; private set; } = default!;
    internal static Seat Create(Guid hallId, string row, int number)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(row);
        if (number <= 0) throw new ArgumentOutOfRangeException(nameof(number));
        return new Seat { HallId = hallId, Row = row.ToUpperInvariant(), Number = number };
    }
}