using AutoMapper;
using HealthTracker.Dto;
using HealthTracker.Models;

namespace HealthTracker.Infrastructure.Profiles
{
    public class SensorProfile : Profile
    {
        public SensorProfile()
        {
            CreateMap<Sensor, GetSensorDto>()
                .ForMember(dest => dest.DeviceId, opt => opt.MapFrom(src => src.DeviceId))
                .ForMember(dest => dest.SensorType, opt => opt.MapFrom(src => src.SensorType))
                .ForMember(dest => dest.TimeStamp, opt => opt.MapFrom(src => src.TimeStamp))
                .ForMember(dest => dest.AlertState, opt => opt.MapFrom(src => src.AlertState));

            CreateMap<GetSensorDto, Sensor>()
                .ForMember(dest => dest.DeviceId, opt => opt.MapFrom(src => src.DeviceId))
                .ForMember(dest => dest.SensorType, opt => opt.MapFrom(src => src.SensorType))
                .ForMember(dest => dest.TimeStamp, opt => opt.MapFrom(src => src.TimeStamp))
                .ForMember(dest => dest.AlertState, opt => opt.MapFrom(src => src.AlertState));
        }
    }
}
