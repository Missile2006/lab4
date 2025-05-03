using Museum.DAL.Entities;
using Museum.DAL.Repositories;

namespace Museum.DAL.Interfaces
{
    public interface IExhibitionRepository : IGenericRepository<Exhibition>
    {
        IEnumerable<Exhibition> GetByTheme(string theme);
        IEnumerable<Exhibition> GetByTargetAudience(string audience);
        IEnumerable<Exhibition> GetCurrentExhibitions(DateTime currentDate);
    }
}
