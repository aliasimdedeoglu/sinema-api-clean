using CinemaSystem.Application.Common.Models;
using CinemaSystem.Application.DTOs;
using CinemaSystem.Domain.Entities;
using CinemaSystem.Domain.Repositories;
using FluentValidation;
using MediatR;
namespace CinemaSystem.Application.Features.ShowTimes.Commands;
public sealed record CreateShowTimeCommand(
    Guid MovieId,
    Guid HallId,
    DateTime StartTime,
    decimal TicketPrice) : IRequest<Result<ShowTimeDto>>;
public sealed class CreateShowTimeCommandValidator : AbstractValidator<CreateShowTimeCommand>
{
    public CreateShowTimeCommandValidator()
    {
        RuleFor(x => x.MovieId).NotEmpty();
        RuleFor(x => x.HallId).NotEmpty();
        RuleFor(x => x.StartTime).GreaterThan(DateTime.UtcNow).WithMessage("ShowTime must be in the future.");
        RuleFor(x => x.TicketPrice).GreaterThan(0).WithMessage("Ticket price must be positive.");
    }
}
public sealed class CreateShowTimeCommandHandler(
    IMovieRepository movieRepository,
    ICinemaHallRepository hallRepository,
    IShowTimeRepository showTimeRepository,
    IShowTimeSeatRepository showTimeSeatRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateShowTimeCommand, Result<ShowTimeDto>>
{
    public async Task<Result<ShowTimeDto>> Handle(CreateShowTimeCommand request, CancellationToken cancellationToken)
    {
        var movie = await movieRepository.GetByIdAsync(request.MovieId, cancellationToken);
        if (movie is null)
            return Result<ShowTimeDto>.NotFound($"Movie '{request.MovieId}' not found.");
        var hall = await hallRepository.GetByIdWithSeatsAsync(request.HallId, cancellationToken);
        if (hall is null)
            return Result<ShowTimeDto>.NotFound($"CinemaHall '{request.HallId}' not found.");
        if (hall.Seats.Count == 0)
            return Result<ShowTimeDto>.Failure("Cannot schedule a showtime for a hall with no seats configured.");
        var existingShowTimes = await showTimeRepository.GetActiveByHallIdAsync(request.HallId, cancellationToken);
        var showTime = ShowTime.Create(
            request.MovieId,
            request.HallId,
            request.StartTime,
            movie.DurationMinutes,
            request.TicketPrice,
            existingShowTimes);
        await showTimeRepository.AddAsync(showTime, cancellationToken);
        var showTimeSeats = hall.Seats
            .Select(s => ShowTimeSeat.Create(showTime.Id, s.Id))
            .ToList();
        await showTimeSeatRepository.AddRangeAsync(showTimeSeats, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        var result = new ShowTimeDto(
            showTime.Id,
            showTime.MovieId,
            movie.Title,
            showTime.HallId,
            hall.Name,
            showTime.StartTime,
            showTime.EndTime,
            showTime.TicketPrice);
        return Result<ShowTimeDto>.Success(result);
    }
}