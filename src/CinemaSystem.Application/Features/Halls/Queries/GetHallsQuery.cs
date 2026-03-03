using AutoMapper;
using CinemaSystem.Application.Common.Models;
using CinemaSystem.Application.DTOs;
using CinemaSystem.Domain.Repositories;
using MediatR;
namespace CinemaSystem.Application.Features.Halls.Queries;
public sealed record GetHallsQuery : IRequest<Result<IReadOnlyList<CinemaHallDto>>>;
public sealed record GetHallByIdQuery(Guid Id) : IRequest<Result<CinemaHallDto>>;
public sealed class GetHallsQueryHandler(
    ICinemaHallRepository hallRepository,
    IMapper mapper) : IRequestHandler<GetHallsQuery, Result<IReadOnlyList<CinemaHallDto>>>
{
    public async Task<Result<IReadOnlyList<CinemaHallDto>>> Handle(
        GetHallsQuery request, CancellationToken cancellationToken)
    {
        var halls = await hallRepository.GetAllAsync(cancellationToken);
        return Result<IReadOnlyList<CinemaHallDto>>.Success(mapper.Map<IReadOnlyList<CinemaHallDto>>(halls));
    }
}
public sealed class GetHallByIdQueryHandler(
    ICinemaHallRepository hallRepository,
    IMapper mapper) : IRequestHandler<GetHallByIdQuery, Result<CinemaHallDto>>
{
    public async Task<Result<CinemaHallDto>> Handle(
        GetHallByIdQuery request, CancellationToken cancellationToken)
    {
        var hall = await hallRepository.GetByIdAsync(request.Id, cancellationToken);
        return hall is null
            ? Result<CinemaHallDto>.NotFound($"CinemaHall '{request.Id}' not found.")
            : Result<CinemaHallDto>.Success(mapper.Map<CinemaHallDto>(hall));
    }
}