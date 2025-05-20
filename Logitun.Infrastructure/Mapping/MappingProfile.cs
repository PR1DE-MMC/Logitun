using AutoMapper;
using Logitun.Core.Entities;
using Logitun.Shared.DTOs;

namespace Logitun.Infrastructure.Mapping
{


    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Truck, TruckDto>().ReverseMap();

            CreateMap<TimeOffRequest, TimeOffRequestDto>().ReverseMap();

            CreateMap<Location, LocationDto>().ReverseMap();

            CreateMap<Mission, MissionDto>().ReverseMap();
        }
    }

}
