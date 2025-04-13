using Museum.DAL.Entities;

namespace Museum.DAL.Interfaces
{
    public interface IExhibitionRepository
    {
        Exhibition GetById(int id);
        IEnumerable<Exhibition> GetAll();
        IEnumerable<Exhibition> GetByTheme(string theme);
        IEnumerable<Exhibition> GetByTargetAudience(string audience);
        IEnumerable<Exhibition> GetCurrentExhibitions(DateTime currentDate);

        void Add(Exhibition exhibition);
        void Update(Exhibition exhibition);
        void Delete(int id);
    }
}