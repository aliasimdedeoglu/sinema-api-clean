using CinemaSystem.Application.Common.Models;
using CinemaSystem.Domain.Repositories;
using MediatR;
namespace CinemaSystem.Application.Features.Movies.Commands;
public sealed record DeleteMovieCommand(Guid Id) : IRequest<Result<bool>>;
public sealed class DeleteMovieCommandHandler(
    IMovieRepository movieRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<DeleteMovieCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeleteMovieCommand request, CancellationToken cancellationToken)
    {
        var movie = await movieRepository.GetByIdAsync(request.Id, cancellationToken);
        if (movie is null)
            return Result<bool>.NotFound($"Movie '{request.Id}' not found.");
        movie.Delete();
        movieRepository.Update(movie);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }
}