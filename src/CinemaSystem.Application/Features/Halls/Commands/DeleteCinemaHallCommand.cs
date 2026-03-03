using CinemaSystem.Application.Common.Models;
using CinemaSystem.Domain.Repositories;
using MediatR;
namespace CinemaSystem.Application.Features.Halls.Commands;
public sealed record DeleteCinemaHallCommand(Guid Id) : IRequest<Result<bool>>;
public sealed class DeleteCinemaHallCommandHandler(
    ICinemaHallRepository hallRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<DeleteCinemaHallCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeleteCinemaHallCommand request, CancellationToken cancellationToken)
    {
        var hall = await hallRepository.GetByIdAsync(request.Id, cancellationToken);
        if (hall is null)
            return Result<bool>.NotFound($"CinemaHall '{request.Id}' not found.");
        hall.Delete();
        hallRepository.Update(hall);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }
}