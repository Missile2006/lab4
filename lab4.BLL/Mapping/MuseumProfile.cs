using AutoMapper;
using Dtos.DTO;
using Museum.BLL.Models;
using Museum.BLL.Strategies.Models;
using Museum.DAL.Entities;


namespace Museum.BLL.Mapping
{
    public class MuseumProfile : Profile
    {
        public MuseumProfile()
        {
            // Exhibition
            CreateMap<Exhibition, ExhibitionModel>()
                .ForMember(dest => dest.Schedules, opt => opt.Ignore())
                .ForMember(dest => dest.Visits, opt => opt.Ignore());
            CreateMap<ExhibitionModel, Exhibition>();
            CreateMap<ExhibitionModel, ExhibitionDto>().ReverseMap();

            // Schedule
            CreateMap<Schedule, ScheduleModel>()
                .ForMember(dest => dest.ScheduledDate, opt => opt.MapFrom(src => src.Date));
            CreateMap<ScheduleModel, Schedule>()
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.ScheduledDate));
            CreateMap<ScheduleModel, ScheduleDto>().ReverseMap();

            // Tour
            CreateMap<Tour, TourModel>();
            CreateMap<TourModel, Tour>();
            CreateMap<TourModel, TourDto>().ReverseMap();
            CreateMap<TourModel, TourCreationModel>();

            // Visit
            CreateMap<Visit, VisitModel>();
            CreateMap<VisitModel, Visit>();
            CreateMap<VisitModel, VisitDto>().ReverseMap();
        }
    }
}