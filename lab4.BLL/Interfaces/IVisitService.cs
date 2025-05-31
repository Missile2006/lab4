using Museum.BLL.Models;
using System;
using System.Collections.Generic;

namespace Museum.BLL.Interfaces
{
    public interface IVisitService
    {
        void AddVisit(VisitModel model);
        void DeleteVisit(int id);
        VisitModel GetVisit(int id);
        IEnumerable<VisitModel> GetAllVisits();
        IEnumerable<VisitModel> SearchVisitsByName(string visitorName);
        IEnumerable<VisitModel> SearchVisitsByDateRange(DateTime startDate, DateTime endDate);
        decimal CalculateTotalIncome(DateTime startDate, DateTime endDate);
    }
}