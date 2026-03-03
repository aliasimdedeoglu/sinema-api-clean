using CinemaSystem.Application.Common.Models;
using CinemaSystem.Domain.Repositories;
using MediatR;
namespace CinemaSystem.Application.Features.Reservations.Commands;
public sealed record CancelReservationCommand(Guid ReservationId, Guid RequestingUserId, bool IsAdmin)
    : IRequest<Result<bool>>;
public sealed class CancelReservationCommandHandler(
    IReservationRepository reservationRepository,
    IShowTimeSeatRepository showTimeSeatRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<CancelReservationCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(CancelReservationCommand request, CancellationToken cancellationToken)
    {
        var reservation = await reservationRepository.GetByIdWithSeatsAsync(request.ReservationId, cancellationToken);
        if (reservation is null)
            return Result<bool>.NotFound($"Reservation '{request.ReservationId}' not found.");
        if (!request.IsAdmin && reservation.UserId != request.RequestingUserId)
            return Result<bool>.Failure("You are not allowed to cancel this reservation.", 403);
        reservation.Cancel();
        var seatIds = reservation.Seats.Select(rs => rs.SeatId).ToList();
        var showTimeSeats = await showTimeSeatRepository
            .GetByShowTimeAndSeatIdsAsync(reservation.ShowTimeId, seatIds, cancellationToken);
        foreach (var sts in showTimeSeats)
            sts.Release();
        reservationRepository.Update(reservation);
        showTimeSeatRepository.UpdateRange(showTimeSeats);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }
}