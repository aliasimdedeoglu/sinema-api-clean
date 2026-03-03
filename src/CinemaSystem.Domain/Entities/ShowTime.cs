using CinemaSystem.Domain.Common;
using CinemaSystem.Domain.Exceptions;
namespace CinemaSystem.Domain.Entities;
public sealed class ShowTime : BaseEntity, ISoftDeletable
{
    private const int TransitionBufferMinutes = 15;
    private ShowTime() { }
    public Guid MovieId { get; private set; }
    public Guid HallId { get; private set; }
    public DateTime StartTime { get; private set; }
    public DateTime EndTime { get; private set; }
    public decimal TicketPrice { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public void Delete()
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
    }
    public Movie Movie { get; private set; } = default!;
    public CinemaHall Hall { get; private set; } = default!;
    private readonly List<Reservation> _reservations = [];
    public IReadOnlyCollection<Reservation> Reservations => _reservations.AsReadOnly();
    private readonly List<ShowTimeSeat> _showTimeSeats = [];
    public IReadOnlyCollection<ShowTimeSeat> ShowTimeSeats => _showTimeSeats.AsReadOnly();
    public static ShowTime Create(Guid movieId, Guid hallId, DateTime startTime,
        int movieDurationMinutes, decimal ticketPrice,
        IEnumerable<ShowTime> existingShowTimesInHall)
    {
        if (startTime < DateTime.UtcNow)
            throw new ArgumentException("ShowTime cannot be scheduled in the past.");
        if (ticketPrice <= 0)
            throw new ArgumentOutOfRangeException(nameof(ticketPrice));
        var endTime = startTime.AddMinutes(movieDurationMinutes + TransitionBufferMinutes);
        CheckForOverlap(hallId, startTime, endTime, existingShowTimesInHall);
        return new ShowTime
        {
            MovieId = movieId,
            HallId = hallId,
            StartTime = startTime,
            EndTime = endTime,
            TicketPrice = ticketPrice
        };
    }
    private static void CheckForOverlap(Guid hallId, DateTime start, DateTime end,
        IEnumerable<ShowTime> existing)
    {
        var conflict = existing.FirstOrDefault(st =>
            st.HallId == hallId &&
            start < st.EndTime &&
            end > st.StartTime);
        if (conflict is not null)
            throw new ShowTimeOverlapException(hallId, conflict.StartTime, conflict.EndTime);
    }
}