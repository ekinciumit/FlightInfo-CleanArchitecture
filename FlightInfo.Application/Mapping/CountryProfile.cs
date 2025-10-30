using AutoMapper;
using FlightInfo.Domain.Entities;
using FlightInfo.Shared.DTOs;

namespace FlightInfo.Application.Mapping
{
    /// <summary>
    /// Country mapping profile for AutoMapper
    /// </summary>
    public class CountryProfile : Profile
    {
        public CountryProfile()
        {
            // Country Entity to CountryDto
            CreateMap<Country, CountryDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));

            // CountryDto to Country Entity
            CreateMap<CountryDto, Country>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));
        }
    }
}


