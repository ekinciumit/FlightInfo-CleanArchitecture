using AutoMapper;
using FlightInfo.Domain.Entities;
using FlightInfo.Shared.DTOs;

namespace FlightInfo.Application.Mapping
{
    /// <summary>
    /// Flight mapping profile for AutoMapper
    /// </summary>
    public class FlightProfile : Profile
    {
        public FlightProfile()
        {
            // Flight Entity to FlightDto
            CreateMap<Flight, FlightDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.FlightNumber, opt => opt.MapFrom(src => src.FlightNumber))
                .ForMember(dest => dest.Origin, opt => opt.MapFrom(src => src.Origin))
                .ForMember(dest => dest.Destination, opt => opt.MapFrom(src => src.Destination))
                .ForMember(dest => dest.DepartureTime, opt => opt.MapFrom(src => src.DepartureTime))
                .ForMember(dest => dest.ArrivalTime, opt => opt.MapFrom(src => src.ArrivalTime))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.AircraftType, opt => opt.MapFrom(src => src.AircraftType))
                .ForMember(dest => dest.Capacity, opt => opt.MapFrom(src => src.Capacity));

            // FlightDto to Flight Entity
            CreateMap<FlightDto, Flight>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.FlightNumber, opt => opt.MapFrom(src => src.FlightNumber))
                .ForMember(dest => dest.Origin, opt => opt.MapFrom(src => src.Origin))
                .ForMember(dest => dest.Destination, opt => opt.MapFrom(src => src.Destination))
                .ForMember(dest => dest.DepartureTime, opt => opt.MapFrom(src => src.DepartureTime))
                .ForMember(dest => dest.ArrivalTime, opt => opt.MapFrom(src => src.ArrivalTime))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.AircraftType, opt => opt.MapFrom(src => src.AircraftType))
                .ForMember(dest => dest.Capacity, opt => opt.MapFrom(src => src.Capacity));
        }
    }
}


