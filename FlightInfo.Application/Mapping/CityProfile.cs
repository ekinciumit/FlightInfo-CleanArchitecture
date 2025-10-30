using AutoMapper;
using FlightInfo.Domain.Entities;
using FlightInfo.Shared.DTOs;

namespace FlightInfo.Application.Mapping
{
    /// <summary>
    /// City mapping profile for AutoMapper
    /// </summary>
    public class CityProfile : Profile
    {
        public CityProfile()
        {
            // City Entity to CityDto
            CreateMap<City, CityDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code))
                .ForMember(dest => dest.CountryId, opt => opt.MapFrom(src => src.CountryId));

            // CityDto to City Entity
            CreateMap<CityDto, City>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code))
                .ForMember(dest => dest.CountryId, opt => opt.MapFrom(src => src.CountryId));
        }
    }
}


