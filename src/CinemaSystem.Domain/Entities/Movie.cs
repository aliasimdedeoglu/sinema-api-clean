using CinemaSystem.Domain.Common;
using CinemaSystem.Domain.Exceptions;
namespace CinemaSystem.Domain.Entities;
public sealed class Movie : BaseEntity, ISoftDeletable
{
    private Movie() { } // EF Core constructor
    public string Title { get; private set; } = default!;
    public string Description { get; private set; } = default!;
    public int DurationMinutes { get; private set; }
    public DateOnly ReleaseDate { get; private set; }
    public string Genre { get; private set; } = default!;
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    private readonly List<ShowTime> _showTimes = [];
    public IReadOnlyCollection<ShowTime> ShowTimes => _showTimes.AsReadOnly();
    public static Movie Create(string title, string description, int durationMinutes,
        DateOnly releaseDate, string genre)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        if (durationMinutes <= 0)
            throw new ArgumentOutOfRangeException(nameof(durationMinutes), "Duration must be positive.");
        return new Movie
        {
            Title = title.Trim(),
            Description = description?.Trim() ?? string.Empty,
            DurationMinutes = durationMinutes,
            ReleaseDate = releaseDate,
            Genre = genre.Trim()
        };
    }
    public void Update(string title, string description, int durationMinutes, string genre)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        if (durationMinutes <= 0)
            throw new ArgumentOutOfRangeException(nameof(durationMinutes));
        Title = title.Trim();
        Description = description?.Trim() ?? string.Empty;
        DurationMinutes = durationMinutes;
        Genre = genre.Trim();
    }
    public void Delete()
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
    }
}