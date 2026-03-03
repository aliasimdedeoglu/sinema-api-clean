using AutoMapper;
using CinemaSystem.Application.DTOs;
using CinemaSystem.Domain.Entities;
namespace CinemaSystem.Application.Mappings;
public sealed class CinemaMappingProfile : Profile
{
    public CinemaMappingProfile()
    {
        CreateMap<Movie, MovieDto>();
        CreateMap<CinemaHall, CinemaHallDto>()
            .ForMember(dest => dest.Capacity, opt => opt.MapFrom(src => src.Seats.Count));
        CreateMap<Seat, SeatDto>();
        CreateMap<ShowTimeSeat, ShowTimeSeatDto>()
            .ConstructUsing(src => new ShowTimeSeatDto(
                src.Id,
                src.SeatId,
                src.Seat != null ? src.Seat.Row : string.Empty,
                src.Seat != null ? src.Seat.Number : 0,
                src.IsReserved))
            .ForAllMembers(o => o.Ignore());
        CreateMap<ShowTime, ShowTimeDto>()
            .ForMember(dest => dest.MovieTitle, opt => opt.MapFrom(src => src.Movie.Title))
            .ForMember(dest => dest.HallName, opt => opt.MapFrom(src => src.Hall.Name));
        CreateMap<Reservation, ReservationDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.ShowTimeMovieTitle,
                opt => opt.MapFrom(src => src.ShowTime != null ? src.ShowTime.Movie.Title : string.Empty))
            .ForMember(dest => dest.Seats,
                opt => opt.MapFrom(src => src.Seats));
        CreateMap<ReservationSeat, ReservedSeatDto>()
            .ConstructUsing(src => new ReservedSeatDto(
                src.SeatId,
                src.Seat != null ? src.Seat.Row : string.Empty,
                src.Seat != null ? src.Seat.Number : 0))
            .ForAllMembers(o => o.Ignore());
    }
}