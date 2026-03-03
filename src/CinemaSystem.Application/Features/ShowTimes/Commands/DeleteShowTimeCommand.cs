using CinemaSystem.Application.Common.Models;
using CinemaSystem.Domain.Repositories;
using MediatR;
namespace CinemaSystem.Application.Features.ShowTimes.Commands;
public sealed record DeleteShowTimeCommand(Guid Id) : IRequest<Result<bool>>;
public sealed class DeleteShowTimeCommandHandler(
    IShowTimeRepository showTimeRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<DeleteShowTimeCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeleteShowTimeCommand request, CancellationToken cancellationToken)
    {
        var showTime = await showTimeRepository.GetByIdAsync(request.Id, cancellationToken);
        if (showTime is null)
            return Result<bool>.NotFound($"ShowTime '{request.Id}' not found.");
        showTime.Delete();
        showTimeRepository.Update(showTime);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }
}