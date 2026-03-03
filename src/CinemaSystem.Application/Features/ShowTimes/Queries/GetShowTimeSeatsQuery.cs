using AutoMapper;
using CinemaSystem.Application.Common.Models;
using CinemaSystem.Application.DTOs;
using CinemaSystem.Domain.Repositories;
using MediatR;
namespace CinemaSystem.Application.Features.ShowTimes.Queries;
public sealed record GetShowTimeSeatsQuery(Guid ShowTimeId)
    : IRequest<Result<IReadOnlyList<ShowTimeSeatDto>>>;
public sealed class GetShowTimeSeatsQueryHandler(
    IShowTimeRepository showTimeRepository,
    IShowTimeSeatRepository showTimeSeatRepository,
    IMapper mapper)
    : IRequestHandler<GetShowTimeSeatsQuery, Result<IReadOnlyList<ShowTimeSeatDto>>>
{
    public async Task<Result<IReadOnlyList<ShowTimeSeatDto>>> Handle(
        GetShowTimeSeatsQuery request, CancellationToken cancellationToken)
    {
        var showTime = await showTimeRepository.GetByIdAsync(request.ShowTimeId, cancellationToken);
        if (showTime is null)
            return Result<IReadOnlyList<ShowTimeSeatDto>>.NotFound(
                $"ShowTime '{request.ShowTimeId}' not found.");
        var showTimeSeats = await showTimeSeatRepository
            .GetByShowTimeIdAsync(request.ShowTimeId, cancellationToken);
        var dtos = mapper.Map<IReadOnlyList<ShowTimeSeatDto>>(showTimeSeats);
        return Result<IReadOnlyList<ShowTimeSeatDto>>.Success(dtos);
    }
}