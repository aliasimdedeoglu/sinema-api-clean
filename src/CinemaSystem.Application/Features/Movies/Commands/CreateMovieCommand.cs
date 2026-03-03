using AutoMapper;
using CinemaSystem.Application.Common.Models;
using CinemaSystem.Application.DTOs;
using CinemaSystem.Domain.Entities;
using CinemaSystem.Domain.Repositories;
using MediatR;
namespace CinemaSystem.Application.Features.Movies.Commands;
public sealed record CreateMovieCommand(
    string Title,
    string Description,
    int DurationMinutes,
    DateOnly ReleaseDate,
    string Genre) : IRequest<Result<MovieDto>>;
public sealed class CreateMovieCommandHandler(
    IMovieRepository movieRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper) : IRequestHandler<CreateMovieCommand, Result<MovieDto>>
{
    public async Task<Result<MovieDto>> Handle(CreateMovieCommand request, CancellationToken cancellationToken)
    {
        var movie = Movie.Create(
            request.Title,
            request.Description,
            request.DurationMinutes,
            request.ReleaseDate,
            request.Genre);
        await movieRepository.AddAsync(movie, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<MovieDto>.Success(mapper.Map<MovieDto>(movie));
    }
}