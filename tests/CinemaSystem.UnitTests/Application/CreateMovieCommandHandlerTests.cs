using AutoMapper;
using CinemaSystem.Application.Features.Movies.Commands;
using CinemaSystem.Application.Mappings;
using CinemaSystem.Domain.Entities;
using CinemaSystem.Domain.Repositories;
using FluentAssertions;
using Moq;
namespace CinemaSystem.UnitTests.Application;
public sealed class CreateMovieCommandHandlerTests
{
    private readonly Mock<IMovieRepository> _movieRepoMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly IMapper _mapper;
    public CreateMovieCommandHandlerTests()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<CinemaMappingProfile>());
        _mapper = config.CreateMapper();
    }
    [Fact]
    public async Task Handle_ValidCommand_ShouldAddMovieAndReturnSuccess()
    {
        var command = new CreateMovieCommand(
            "Test Movie", "A description", 120,
            new DateOnly(2026, 6, 1), "Action");
        _movieRepoMock.Setup(r => r.AddAsync(It.IsAny<Movie>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        var handler = new CreateMovieCommandHandler(_movieRepoMock.Object, _unitOfWorkMock.Object, _mapper);
        var result = await handler.Handle(command, CancellationToken.None);
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Title.Should().Be("Test Movie");
        _movieRepoMock.Verify(r => r.AddAsync(It.IsAny<Movie>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
    [Fact]
    public async Task Handle_WhenRepositoryThrows_ShouldPropagate()
    {
        var command = new CreateMovieCommand("Movie", "Desc", 90, new DateOnly(2026, 1, 1), "Drama");
        _movieRepoMock.Setup(r => r.AddAsync(It.IsAny<Movie>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("DB error"));
        var handler = new CreateMovieCommandHandler(_movieRepoMock.Object, _unitOfWorkMock.Object, _mapper);
        var act = async () => await handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<InvalidOperationException>();
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}