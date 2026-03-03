using AutoMapper;
using CinemaSystem.Application.Common.Models;
using CinemaSystem.Application.DTOs;
using CinemaSystem.Domain.Repositories;
using MediatR;
namespace CinemaSystem.Application.Features.Reservations.Queries;
public sealed record GetMyReservationsQuery(Guid UserId, int Page = 1, int PageSize = 10)
    : IRequest<Result<PagedResult<ReservationDto>>>;
public sealed record GetReservationByIdQuery(Guid ReservationId, Guid RequestingUserId, bool IsAdmin)
    : IRequest<Result<ReservationDto>>;
public sealed class GetMyReservationsQueryHandler(
    IReservationRepository reservationRepository,
    IMapper mapper) : IRequestHandler<GetMyReservationsQuery, Result<PagedResult<ReservationDto>>>
{
    public async Task<Result<PagedResult<ReservationDto>>> Handle(
        GetMyReservationsQuery request, CancellationToken cancellationToken)
    {
        var reservations = await reservationRepository.GetByUserIdAsync(
            request.UserId, request.Page, request.PageSize, cancellationToken);
        var dtos = mapper.Map<IReadOnlyList<ReservationDto>>(reservations);
        return Result<PagedResult<ReservationDto>>.Success(
            PagedResult<ReservationDto>.Create(dtos, dtos.Count, request.Page, request.PageSize));
    }
}
public sealed class GetReservationByIdQueryHandler(
    IReservationRepository reservationRepository,
    IMapper mapper) : IRequestHandler<GetReservationByIdQuery, Result<ReservationDto>>
{
    public async Task<Result<ReservationDto>> Handle(
        GetReservationByIdQuery request, CancellationToken cancellationToken)
    {
        var reservation = await reservationRepository.GetByIdWithSeatsAsync(request.ReservationId, cancellationToken);
        if (reservation is null)
            return Result<ReservationDto>.NotFound($"Reservation '{request.ReservationId}' not found.");
        if (!request.IsAdmin && reservation.UserId != request.RequestingUserId)
            return Result<ReservationDto>.Failure("Access denied.", 403);
        return Result<ReservationDto>.Success(mapper.Map<ReservationDto>(reservation));
    }
}