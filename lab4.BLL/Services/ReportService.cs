﻿using AutoMapper;
using Museum.DAL.Entities;
using Museum.DAL.Interfaces;
using Museum.BLL.Models.Reports;
using Museum.BLL.Models;
using Museum.DAL.UoW;
using Museum.BLL.Interfaces;

namespace Museum.BLL.Services
{
    public class ReportService : IReportService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ReportService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // Звіт про поточні експозиції
        public IEnumerable<ExhibitionModel> GetCurrentExhibitionsReport()
        {
            var exhibitions = _unitOfWork.Exhibitions.GetCurrentExhibitions(DateTime.Now);
            return _mapper.Map<IEnumerable<ExhibitionModel>>(exhibitions);  
        }

        // Звіт про візити за період
        public VisitsReport GenerateVisitsReport(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                throw new ArgumentException("Дата завершення має бути пізніше за дату початку");

            var visits = _unitOfWork.Visits.GetByDateRange(startDate, endDate).ToList();
            var totalIncome = visits.Sum(v => v.TicketPrice);

            var visitsByExhibition = visits
                .GroupBy(v => v.ExhibitionId)
                .Select(g => new ExhibitionVisitStats
                {
                    Exhibition = _mapper.Map<ExhibitionModel>(_unitOfWork.Exhibitions.GetById(g.Key)),  
                    VisitCount = g.Count(),
                    TotalIncome = g.Sum(v => v.TicketPrice)
                })
                .OrderByDescending(x => x.VisitCount);

            return new VisitsReport
            {
                StartDate = startDate,
                EndDate = endDate,
                TotalVisits = visits.Count,
                TotalIncome = totalIncome,
                ExhibitionStats = visitsByExhibition.ToList()
            };
        }

        // Звіт про доходи від екскурсій
        public ToursReport GenerateToursReport(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                throw new ArgumentException("Дата завершення має бути пізніше за дату початку");

            var tours = _unitOfWork.Tours.GetAll()
                .Where(t => t.TourDate >= startDate && t.TourDate <= endDate)
                .ToList();

            var totalIncome = tours.Sum(t => t.Price);
            var privateToursCount = tours.Count(t => t.IsPrivate);
            var groupToursCount = tours.Count(t => !t.IsPrivate);

            var toursByGuide = tours
                .GroupBy(t => t.GuideName)
                .Select(g => new GuideTourStats
                {
                    GuideName = g.Key,
                    TourCount = g.Count(),
                    TotalIncome = g.Sum(t => t.Price)
                })
                .OrderByDescending(x => x.TotalIncome);

            return new ToursReport
            {
                StartDate = startDate,
                EndDate = endDate,
                TotalIncome = totalIncome,
                PrivateToursCount = privateToursCount,
                GroupToursCount = groupToursCount,
                GuideStats = toursByGuide.ToList()
            };
        }

        // Звіт про популярність експозицій
        public PopularExhibitionsReport GeneratePopularExhibitionsReport(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                throw new ArgumentException("Дата завершення має бути пізніше за дату початку");

            var visits = _unitOfWork.Visits.GetByDateRange(startDate, endDate).ToList();
            var tours = _unitOfWork.Tours.GetAll()
                .Where(t => t.TourDate >= startDate && t.TourDate <= endDate)
                .ToList();

            var exhibitions = _unitOfWork.Exhibitions.GetAll();

            var exhibitionsPopularity = exhibitions
                .Select(e => new ExhibitionPopularity
                {
                    Exhibition = _mapper.Map<ExhibitionModel>(e), 
                    VisitsCount = visits.Count(v => v.ExhibitionId == e.ExhibitionId),
                    ToursCount = tours.Count(t => t.ExhibitionId == e.ExhibitionId),
                    TotalScore = visits.Count(v => v.ExhibitionId == e.ExhibitionId) +
                                 tours.Count(t => t.ExhibitionId == e.ExhibitionId) * 2
                })
                .OrderByDescending(e => e.TotalScore)
                .ToList();

            return new PopularExhibitionsReport
            {
                StartDate = startDate,
                EndDate = endDate,
                Exhibitions = exhibitionsPopularity
            };
        }
    }
}
