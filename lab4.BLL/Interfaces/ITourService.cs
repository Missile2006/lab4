using Museum.BLL.Models;
using System;
using System.Collections.Generic;

namespace Museum.BLL.Interfaces
{
    public interface ITourService
    {
        void PlanGroupTour(TourModel model);
        void PlanPrivateTour(TourModel model);
        void AddTour(TourModel model);
        void UpdateTour(TourModel model);
        void DeleteTour(int id);
        TourModel GetTour(int id, bool eagerLoading = false);
        IEnumerable<TourModel> GetAllTours(bool includeExhibitions = false);
        IEnumerable<TourModel> GetExhibitionTours(int exhibitionId, bool eagerLoading = false);
        IEnumerable<TourModel> GetPrivateTours(bool eagerLoading = false);
        IEnumerable<TourModel> GetScheduledTours(bool eagerLoading = false);
        IEnumerable<TourModel> SearchToursByGuide(string guideName, bool eagerLoading = false);
        decimal CalculateTourIncome(DateTime startDate, DateTime endDate);
    }
}