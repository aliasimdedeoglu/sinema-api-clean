using AutoMapper;
using CinemaSystem.Application.Common.Models;
using CinemaSystem.Application.DTOs;
using CinemaSystem.Domain.Entities;
using CinemaSystem.Domain.Exceptions;
using CinemaSystem.Domain.Repositories;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
namespace CinemaSystem.Application.Features.Reservations.Commands;
public sealed record ReserveSeatsCommand(
    Guid UserId,
    Guid ShowTimeId,
    IReadOnlyList<Guid> SeatIds) : IRequest<Result<ReservationDto>>;
public sealed class ReserveSeatsCommandValidator : AbstractValidator<ReserveSeatsCommand>
{
    public ReserveSeatsCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.ShowTimeId).NotEmpty();
        RuleFor(x => x.SeatIds)
            .NotEmpty().WithMessage("At least one seat must be selected.")
            .Must(ids => ids.Distinct().Count() == ids.Count)
            .WithMessage("Duplicate seat IDs are not allowed.");
        RuleFor(x => x.SeatIds.Count)
            .LessThanOrEqualTo(10).WithMessage("Cannot reserve more than 10 seats in one transaction.");
    }
}
public sealed class ReserveSeatsCommandHandler(
    IShowTimeRepository showTimeRepository,
    IShowTimeSeatRepository showTimeSeatRepository,
    IReservationRepository reservationRepository,
    IUnitOfWork unitOfWork,
    IDbTransactionManager transactionManager,
    IMapper mapper,
    ILogger<ReserveSeatsCommandHandler> logger) : IRequestHandler<ReserveSeatsCommand, Result<ReservationDto>>
{
    public async Task<Result<ReservationDto>> Handle(ReserveSeatsCommand request, CancellationToken cancellationToken)
    {
        var showTime = await showTimeRepository.GetByIdAsync(request.ShowTimeId, cancellationToken);
        if (showTime is null)
            return Result<ReservationDto>.NotFound($"ShowTime '{request.ShowTimeId}' not found.");
        if (showTime.StartTime <= DateTime.UtcNow)
            return Result<ReservationDto>.Failure("Cannot reserve seats for a showtime that has already started.");
        await using var transaction = await transactionManager.BeginTransactionAsync(cancellationToken);
        try
        {
            var showTimeSeats = await showTimeSeatRepository.GetByShowTimeAndSeatIdsAsync(
                request.ShowTimeId, request.SeatIds, cancellationToken);
            if (showTimeSeats.Count != request.SeatIds.Count)
                return Result<ReservationDto>.NotFound(
                    "One or more seats were not found for this showtime. " +
                    "Ensure the seat IDs belong to this showtime's hall.");
            foreach (var showTimeSeat in showTimeSeats)
                showTimeSeat.Reserve();
            showTimeSeatRepository.UpdateRange(showTimeSeats);
            var seatIds = showTimeSeats.Select(sts => sts.SeatId).ToList();
            var reservation = Reservation.Create(
                request.UserId,
                request.ShowTimeId,
                seatIds,
                showTime.TicketPrice);
            await reservationRepository.AddAsync(reservation, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            logger.LogInformation("Reservation {ReservationId} created for user {UserId}",
                reservation.Id, request.UserId);
            var dto = mapper.Map<ReservationDto>(reservation);
            return Result<ReservationDto>.Success(dto);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            logger.LogWarning(ex,
                "Concurrency conflict while reserving seats for ShowTime {ShowTimeId}.",
                request.ShowTimeId);
            return Result<ReservationDto>.Conflict(
                "One or more seats were just reserved by another user. Please refresh and try again.");
        }
        catch (SeatAlreadyReservedException ex)
        {
            logger.LogWarning("Seat already reserved: {Message}", ex.Message);
            return Result<ReservationDto>.Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error during seat reservation");
            throw;
        }
    }
}