using FluentValidation;
namespace CinemaSystem.Application.Features.Movies.Commands;
public sealed class CreateMovieCommandValidator : AbstractValidator<CreateMovieCommand>
{
    public CreateMovieCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Movie title is required.")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters.");
        RuleFor(x => x.DurationMinutes)
            .GreaterThan(0).WithMessage("Duration must be positive.")
            .LessThanOrEqualTo(600).WithMessage("Duration seems unrealistically long.");
        RuleFor(x => x.ReleaseDate)
            .NotEmpty();
        RuleFor(x => x.Genre)
            .NotEmpty().WithMessage("Genre is required.")
            .MaximumLength(100);
        RuleFor(x => x.Description)
            .MaximumLength(2000);
    }
}