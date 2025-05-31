using AutoMapper;
using Museum.BLL.Models;
using Museum.BLL.Strategies.Models;
using Museum.DAL.Entities;

namespace Museum.BLL.Mapping
{
    public class MuseumProfile : Profile
    {
        public MuseumProfile()
        {
            // Налаштування маппінгу між Exhibition та ExhibitionModel
            CreateMap<Exhibition, ExhibitionModel>()
                .ForMember(dest => dest.Schedules, opt => opt.Ignore()) // Ігноруємо Schedules
                .ForMember(dest => dest.Visits, opt => opt.Ignore());   // Ігноруємо Visits
            CreateMap<ExhibitionModel, Exhibition>();

            // Налаштування маппінгу між Schedule та ScheduleModel
            CreateMap<Schedule, ScheduleModel>()
                .ForMember(dest => dest.ScheduleId, opt => opt.MapFrom(src => src.ScheduleId))
                .ForMember(dest => dest.ExhibitionId, opt => opt.MapFrom(src => src.ExhibitionId))
                .ForMember(dest => dest.ScheduledDate, opt => opt.MapFrom(src => src.Date))
                .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.StartTime))
                .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.EndTime));
            CreateMap<ScheduleModel, Schedule>()
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.ScheduledDate));

            // Налаштування маппінгу між Tour та TourModel
            CreateMap<Tour, TourModel>()
                .ForMember(dest => dest.TourId, opt => opt.MapFrom(src => src.TourId))
                .ForMember(dest => dest.ExhibitionId, opt => opt.MapFrom(src => src.ExhibitionId))
                .ForMember(dest => dest.GuideName, opt => opt.MapFrom(src => src.GuideName))
                .ForMember(dest => dest.TourDate, opt => opt.MapFrom(src => src.TourDate))
                .ForMember(dest => dest.IsPrivate, opt => opt.MapFrom(src => src.IsPrivate))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price)); 
            CreateMap<TourModel, Tour>()
                .ForMember(dest => dest.TourDate, opt => opt.MapFrom(src => src.TourDate))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.GuideName, opt => opt.MapFrom(src => src.GuideName))
                .ForMember(dest => dest.ExhibitionId, opt => opt.MapFrom(src => src.ExhibitionId));

            // Налаштування маппінгу між Visit та VisitModel
            CreateMap<Visit, VisitModel>()
                .ForMember(dest => dest.VisitId, opt => opt.MapFrom(src => src.VisitId))
                .ForMember(dest => dest.ExhibitionId, opt => opt.MapFrom(src => src.ExhibitionId))
                .ForMember(dest => dest.VisitorName, opt => opt.MapFrom(src => src.VisitorName))
                .ForMember(dest => dest.VisitDate, opt => opt.MapFrom(src => src.VisitDate))
                .ForMember(dest => dest.TicketPrice, opt => opt.MapFrom(src => src.TicketPrice)); 
            CreateMap<VisitModel, Visit>();

            // Зворотний маппінг для ScheduleModel -> Schedule
            CreateMap<ScheduleModel, Schedule>()
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.ScheduledDate));

            CreateMap<TourModel, TourCreationModel>()
    .ForMember(dest => dest.ParticipantsCount, opt => opt.MapFrom(src => src.ParticipantsCount))
    .ForMember(dest => dest.GuideName, opt => opt.MapFrom(src => src.GuideName))
    .ForMember(dest => dest.TourDate, opt => opt.MapFrom(src => src.TourDate))
    .ForMember(dest => dest.ExhibitionId, opt => opt.MapFrom(src => src.ExhibitionId));
        }
    }
}
