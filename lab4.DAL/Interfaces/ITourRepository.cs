using Museum.DAL.Entities;

namespace Museum.DAL.Interfaces
{
    public interface ITourRepository
    {
        // Базові CRUD операції
        Tour GetById(int id);
        IEnumerable<Tour> GetAll();
        void Add(Tour tour);
        void Update(Tour tour);
        void Delete(int id);

        // Спеціалізовані методи (ліниве завантаження)
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