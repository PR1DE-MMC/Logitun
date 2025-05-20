using AutoMapper;
using Logitun.Core.Entities;
using Logitun.Shared.DTOs;

namespace Logitun.Infrastructure.Mapping
{


    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Truck, TruckDto>()
                .ForMember(dest => dest.TruckId, opt => opt.MapFrom(src => src.TruckId))
                .ForMember(dest => dest.PlateNumber, opt => opt.MapFrom(src => src.PlateNumber))
                .ForMember(dest => dest.Model, opt => opt.MapFrom(src => src.Model))
                .ForMember(dest => dest.ManufactureYear, opt => opt.MapFrom(src => src.ManufactureYear))
                .ForMember(dest => dest.CapacityKg, opt => opt.MapFrom(src => src.CapacityKg))
                .ForMember(dest => dest.FuelType, opt => opt.MapFrom(src => src.FuelType))
                .ForMember(dest => dest.LastMaintenanceDate, opt => opt.MapFrom(src => src.LastMaintenanceDate))
                .ReverseMap();

            CreateMap<TimeOffRequest, TimeOffRequestDto>().ReverseMap();

            CreateMap<Location, LocationDto>().ReverseMap();

            CreateMap<Mission, MissionDto>().ReverseMap();
        }
    }

}
