using CinemaSystem.Domain.Common;
namespace CinemaSystem.Domain.Entities;
public sealed class CinemaHall : BaseEntity, ISoftDeletable
{
    private CinemaHall() { }
    public string Name { get; private set; } = default!;
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public void Delete()
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
    }
    private readonly List<Seat> _seats = [];
    public IReadOnlyCollection<Seat> Seats => _seats.AsReadOnly();
    private readonly List<ShowTime> _showTimes = [];
    public IReadOnlyCollection<ShowTime> ShowTimes => _showTimes.AsReadOnly();
    public int Capacity => _seats.Count;
    public static CinemaHall Create(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        return new CinemaHall { Name = name.Trim() };
    }
    public void ConfigureSeats(IEnumerable<(string Row, int Number)> seatLayout)
    {
        if (_seats.Count > 0)
            throw new InvalidOperationException("Hall seats are already configured.");
        foreach (var (row, number) in seatLayout)
            _seats.Add(Seat.Create(Id, row, number));
    }
    public void UpdateName(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Name = name.Trim();
    }
}