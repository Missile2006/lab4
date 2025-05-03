using Museum.DAL.Entities;
using Museum.DAL.Repositories;

namespace Museum.DAL.Interfaces
{
    public interface ITourRepository : IGenericRepository<Tour>
    {
        IEnumerable<Tour> GetByExhibitionId(int exhibitionId);
        IEnumerable<Tour> GetByGuideName(string guideName);
        IEnumerable<Tour> GetPrivateTours();
        IEnumerable<Tour> GetScheduledTours();

        /// <summary>
        /// Отримує тур з жадібним завантаженням пов'язаної Exhibition
        /// </summary>
        Tour GetByIdWithExhibition(int id);

        /// <summary>
        /// Отримує всі тури з жадібним завантаженням пов'язаних Exhibition
        /// </summary>
        IEnumerable<Tour> GetAllWithExhibitions();

        /// <summary>
        /// Отримує тури для виставки з жадібним завантаженням Exhibition
        /// </summary>
        IEnumerable<Tour> GetByExhibitionIdWithExhibition(int exhibitionId);
    }
}
