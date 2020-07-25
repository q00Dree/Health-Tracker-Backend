using AutoMapper;
using HealthTracker.DAL.Entities;
using HealthTracker.Dto;

namespace HealthTracker.Infrastructure.Profiles
{
    public class ControllerProfile : Profile
    {
        public ControllerProfile()
        {
            CreateMap<ControllerDto, Controller>();
            CreateMap<Controller, ControllerDto>();
        }
    }
}
