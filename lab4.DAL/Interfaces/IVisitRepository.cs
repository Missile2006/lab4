using Museum.DAL.Entities;
using Museum.DAL.Repositories;

namespace Museum.DAL.Interfaces
{
    public interface IVisitRepository : IGenericRepository<Visit>
    {
        IEnumerable<Visit> GetByExhibitionId(int exhibitionId);
        IEnumerable<Visit> GetByVisitorName(string visitorName);
        IEnumerable<Visit> GetByDateRange(DateTime startDate, DateTime endDate);
    }
}
