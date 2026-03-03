using AutoMapper;
using CinemaSystem.Application.Common.Models;
using CinemaSystem.Application.DTOs;
using CinemaSystem.Domain.Entities;
using CinemaSystem.Domain.Repositories;
using FluentValidation;
using MediatR;
namespace CinemaSystem.Application.Features.Halls.Commands;
public sealed record CreateCinemaHallCommand(
    string Name,
    IReadOnlyList<SeatLayoutItem> SeatLayout) : IRequest<Result<CinemaHallDto>>;
public sealed record SeatLayoutItem(string Row, int Number);
public sealed class CreateCinemaHallCommandValidator : AbstractValidator<CreateCinemaHallCommand>
{
    public CreateCinemaHallCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Hall name is required.")
            .MaximumLength(100);
        RuleFor(x => x.SeatLayout)
            .NotEmpty().WithMessage("At least one seat must be configured.")
            .Must(seats => seats.Count <= 1000).WithMessage("A hall cannot have more than 1000 seats.");
        RuleForEach(x => x.SeatLayout).ChildRules(seat =>
        {
            seat.RuleFor(s => s.Row).NotEmpty().MaximumLength(5);
            seat.RuleFor(s => s.Number).GreaterThan(0);
        });
    }
}
public sealed class CreateCinemaHallCommandHandler(
    ICinemaHallRepository hallRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper) : IRequestHandler<CreateCinemaHallCommand, Result<CinemaHallDto>>
{
    public async Task<Result<CinemaHallDto>> Handle(CreateCinemaHallCommand request, CancellationToken cancellationToken)
    {
        var hall = CinemaHall.Create(request.Name);
        hall.ConfigureSeats(request.SeatLayout.Select(s => (s.Row, s.Number)));
        await hallRepository.AddAsync(hall, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<CinemaHallDto>.Success(mapper.Map<CinemaHallDto>(hall));
    }
}