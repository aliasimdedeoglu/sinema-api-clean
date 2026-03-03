namespace CinemaSystem.Application.DTOs;
public sealed record MovieDto(
    Guid Id,
    string Title,
    string Description,
    int DurationMinutes,
    DateOnly ReleaseDate,
    string Genre);
public sealed record CinemaHallDto(
    Guid Id,
    string Name,
    int Capacity);
public sealed record SeatDto(
    Guid Id,
    string Row,
    int Number);
public sealed record ShowTimeSeatDto(
    Guid ShowTimeSeatId,
    Guid SeatId,
    string Row,
    int Number,
    bool IsReserved);
public sealed record ShowTimeDto(
    Guid Id,
    Guid MovieId,
    string MovieTitle,
    Guid HallId,
    string HallName,
    DateTime StartTime,
    DateTime EndTime,
    decimal TicketPrice);
public sealed record ReservationDto(
    Guid Id,
    Guid UserId,
    Guid ShowTimeId,
    string ShowTimeMovieTitle,
    string Status,
    DateTime CreatedAt,
    decimal TotalPrice,
    IReadOnlyList<ReservedSeatDto> Seats);
public sealed record ReservedSeatDto(
    Guid SeatId,
    string Row,
    int Number);