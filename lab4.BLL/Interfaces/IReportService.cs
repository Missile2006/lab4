using Museum.BLL.Models;
using Museum.BLL.Models.Reports;
using System.Collections.Generic;

namespace Museum.BLL.Interfaces
{
    public interface IReportService
    {
        IEnumerable<ExhibitionModel> GetCurrentExhibitionsReport();
        VisitsReport GenerateVisitsReport(DateTime startDate, DateTime endDate);
        ToursReport GenerateToursReport(DateTime startDate, DateTime endDate);
        PopularExhibitionsReport GeneratePopularExhibitionsReport(DateTime startDate, DateTime endDate);
    }
}