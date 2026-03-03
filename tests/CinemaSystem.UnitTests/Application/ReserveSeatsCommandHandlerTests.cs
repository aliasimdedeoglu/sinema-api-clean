using AutoMapper;
using CinemaSystem.Application.Features.Reservations.Commands;
using CinemaSystem.Application.Mappings;
using CinemaSystem.Domain.Entities;
using CinemaSystem.Domain.Exceptions;
using CinemaSystem.Domain.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
namespace CinemaSystem.UnitTests.Application;
public sealed class ReserveSeatsCommandHandlerTests
{
    private readonly Mock<IShowTimeRepository> _showTimeRepoMock = new();
    private readonly Mock<IShowTimeSeatRepository> _showTimeSeatRepoMock = new();
    private readonly Mock<IReservationRepository> _reservationRepoMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IDbTransactionManager> _transactionManagerMock = new();
    private readonly Mock<IDbTransaction> _transactionMock = new();
    private readonly IMapper _mapper;
    private static readonly Guid UserId = Guid.NewGuid();
    private static readonly Guid ShowTimeId = Guid.NewGuid();
    private static readonly Guid HallId = Guid.NewGuid();
    public ReserveSeatsCommandHandlerTests()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<CinemaMappingProfile>());
        _mapper = config.CreateMapper();
        _transactionManagerMock.Setup(t => t.BeginTransactionAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(_transactionMock.Object);
        _transactionMock.Setup(t => t.CommitAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _transactionMock.Setup(t => t.DisposeAsync())
            .Returns(ValueTask.CompletedTask);
    }
    private ShowTime CreateFakeShowTime()
    {
        var showTime = (ShowTime)Activator.CreateInstance(typeof(ShowTime), true)!;
        typeof(ShowTime).GetProperty("Id")!.SetValue(showTime, ShowTimeId);
        typeof(ShowTime).GetProperty("HallId")!.SetValue(showTime, HallId);
        typeof(ShowTime).GetProperty("StartTime")!.SetValue(showTime, DateTime.UtcNow.AddDays(1));
        typeof(ShowTime).GetProperty("EndTime")!.SetValue(showTime, DateTime.UtcNow.AddDays(1).AddHours(2));
        typeof(ShowTime).GetProperty("TicketPrice")!.SetValue(showTime, 15.00m);
        return showTime;
    }
    private ShowTimeSeat CreateFakeShowTimeSeat(Guid seatId, bool isReserved = false)
    {
        var sts = (ShowTimeSeat)Activator.CreateInstance(typeof(ShowTimeSeat), true)!;
        typeof(ShowTimeSeat).GetProperty("Id")!.SetValue(sts, Guid.NewGuid());
        typeof(ShowTimeSeat).GetProperty("ShowTimeId")!.SetValue(sts, ShowTimeId);
        typeof(ShowTimeSeat).GetProperty("SeatId")!.SetValue(sts, seatId);
        typeof(ShowTimeSeat).GetProperty("IsReserved")!.SetValue(sts, isReserved);
        typeof(ShowTimeSeat).GetProperty("RowVersion")!.SetValue(sts, new byte[] { 0, 0, 0, 1 });
        return sts;
    }
    [Fact]
    public async Task Handle_ValidRequest_ShouldCreateReservationAndReturnSuccess()
    {
        var seatId = Guid.NewGuid();
        var showTime = CreateFakeShowTime();
        var showTimeSeat = CreateFakeShowTimeSeat(seatId);
        _showTimeRepoMock.Setup(r => r.GetByIdAsync(ShowTimeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(showTime);
        _showTimeSeatRepoMock.Setup(r => r.GetByShowTimeAndSeatIdsAsync(
                ShowTimeId, It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([showTimeSeat]);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(2);
        _reservationRepoMock.Setup(r => r.AddAsync(It.IsAny<Reservation>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        var handler = CreateHandler();
        var result = await handler.Handle(
            new ReserveSeatsCommand(UserId, ShowTimeId, [seatId]), CancellationToken.None);
        result.IsSuccess.Should().BeTrue();
        showTimeSeat.IsReserved.Should().BeTrue();
        _transactionMock.Verify(t => t.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        _reservationRepoMock.Verify(r => r.AddAsync(It.IsAny<Reservation>(), It.IsAny<CancellationToken>()), Times.Once);
    }
    [Fact]
    public async Task Handle_ShowTimeNotFound_ShouldReturnNotFoundResult()
    {
        _showTimeRepoMock.Setup(r => r.GetByIdAsync(ShowTimeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ShowTime?)null);
        var handler = CreateHandler();
        var result = await handler.Handle(
            new ReserveSeatsCommand(UserId, ShowTimeId, [Guid.NewGuid()]), CancellationToken.None);
        result.IsFailure.Should().BeTrue();
        result.StatusCode.Should().Be(404);
    }
    [Fact]
    public async Task Handle_ShowTimeSeatAlreadyReserved_ShouldReturnConflictResult()
    {
        var seatId = Guid.NewGuid();
        var showTime = CreateFakeShowTime();
        var showTimeSeat = CreateFakeShowTimeSeat(seatId, isReserved: true); // Already reserved for this showtime
        _showTimeRepoMock.Setup(r => r.GetByIdAsync(ShowTimeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(showTime);
        _showTimeSeatRepoMock.Setup(r => r.GetByShowTimeAndSeatIdsAsync(
                ShowTimeId, It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([showTimeSeat]);
        var handler = CreateHandler();
        var result = await handler.Handle(
            new ReserveSeatsCommand(UserId, ShowTimeId, [seatId]), CancellationToken.None);
        result.IsFailure.Should().BeTrue();
        result.StatusCode.Should().Be(409);
        _transactionMock.Verify(t => t.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
    [Fact]
    public async Task Handle_DbUpdateConcurrencyException_ShouldReturnConflict()
    {
        var seatId = Guid.NewGuid();
        var showTime = CreateFakeShowTime();
        var showTimeSeat = CreateFakeShowTimeSeat(seatId);
        _showTimeRepoMock.Setup(r => r.GetByIdAsync(ShowTimeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(showTime);
        _showTimeSeatRepoMock.Setup(r => r.GetByShowTimeAndSeatIdsAsync(
                ShowTimeId, It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([showTimeSeat]);
        _reservationRepoMock.Setup(r => r.AddAsync(It.IsAny<Reservation>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new DbUpdateConcurrencyException("Row version mismatch"));
        var handler = CreateHandler();
        var result = await handler.Handle(
            new ReserveSeatsCommand(UserId, ShowTimeId, [seatId]), CancellationToken.None);
        result.IsFailure.Should().BeTrue();
        result.StatusCode.Should().Be(409);
        result.Error.Should().Contain("reserved by another user");
        _transactionMock.Verify(t => t.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
    private ReserveSeatsCommandHandler CreateHandler() =>
        new(_showTimeRepoMock.Object,
            _showTimeSeatRepoMock.Object,
            _reservationRepoMock.Object,
            _unitOfWorkMock.Object,
            _transactionManagerMock.Object,
            _mapper,
            NullLogger<ReserveSeatsCommandHandler>.Instance);
}