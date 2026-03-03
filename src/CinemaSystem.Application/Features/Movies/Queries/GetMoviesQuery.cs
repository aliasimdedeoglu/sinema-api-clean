using AutoMapper;
using CinemaSystem.Application.Common.Models;
using CinemaSystem.Application.DTOs;
using CinemaSystem.Domain.Repositories;
using MediatR;
namespace CinemaSystem.Application.Features.Movies.Queries;
public sealed record GetMoviesQuery(int Page = 1, int PageSize = 20)
    : IRequest<Result<PagedResult<MovieDto>>>;
public sealed record GetMovieByIdQuery(Guid Id) : IRequest<Result<MovieDto>>;
public sealed class GetMoviesQueryHandler(
    IMovieRepository movieRepository,
    IMapper mapper) : IRequestHandler<GetMoviesQuery, Result<PagedResult<MovieDto>>>
{
    public async Task<Result<PagedResult<MovieDto>>> Handle(GetMoviesQuery request, CancellationToken cancellationToken)
    {
        var movies = await movieRepository.GetAllAsync(request.Page, request.PageSize, cancellationToken);
        var total = await movieRepository.CountAsync(cancellationToken);
        var dtos = mapper.Map<IReadOnlyList<MovieDto>>(movies);
        return Result<PagedResult<MovieDto>>.Success(
            PagedResult<MovieDto>.Create(dtos, total, request.Page, request.PageSize));
    }
}
public sealed class GetMovieByIdQueryHandler(
    IMovieRepository movieRepository,
    IMapper mapper) : IRequestHandler<GetMovieByIdQuery, Result<MovieDto>>
{
    public async Task<Result<MovieDto>> Handle(GetMovieByIdQuery request, CancellationToken cancellationToken)
    {
        var movie = await movieRepository.GetByIdAsync(request.Id, cancellationToken);
        return movie is null
            ? Result<MovieDto>.NotFound($"Movie '{request.Id}' not found.")
            : Result<MovieDto>.Success(mapper.Map<MovieDto>(movie));
    }
}