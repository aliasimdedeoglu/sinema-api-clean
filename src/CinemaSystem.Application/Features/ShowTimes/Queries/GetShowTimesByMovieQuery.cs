using AutoMapper;
using CinemaSystem.Application.Common.Models;
using CinemaSystem.Application.DTOs;
using CinemaSystem.Domain.Repositories;
using MediatR;
namespace CinemaSystem.Application.Features.ShowTimes.Queries;
public sealed record GetShowTimesByMovieQuery(Guid MovieId) : IRequest<Result<IReadOnlyList<ShowTimeDto>>>;
public sealed class GetShowTimesByMovieQueryHandler(
    IShowTimeRepository showTimeRepository,
    IMovieRepository movieRepository,
    IMapper mapper) : IRequestHandler<GetShowTimesByMovieQuery, Result<IReadOnlyList<ShowTimeDto>>>
{
    public async Task<Result<IReadOnlyList<ShowTimeDto>>> Handle(
        GetShowTimesByMovieQuery request, CancellationToken cancellationToken)
    {
        var movie = await movieRepository.GetByIdAsync(request.MovieId, cancellationToken);
        if (movie is null)
            return Result<IReadOnlyList<ShowTimeDto>>.NotFound($"Movie '{request.MovieId}' not found.");
        var showTimes = await showTimeRepository.GetByMovieIdAsync(request.MovieId, cancellationToken);
        return Result<IReadOnlyList<ShowTimeDto>>.Success(mapper.Map<IReadOnlyList<ShowTimeDto>>(showTimes));
    }
}