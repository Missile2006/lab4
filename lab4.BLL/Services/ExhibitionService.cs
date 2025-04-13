using Museum.DAL.Entities;
using Museum.DAL.Interfaces;

namespace Museum.BLL.Services
{
    public class ExhibitionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ExhibitionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void AddExhibition(Exhibition exhibition)
        {
            if (exhibition == null)
                throw new ArgumentNullException(nameof(exhibition), "Експозиція не може бути null");

            if (string.IsNullOrWhiteSpace(exhibition.Title))
                throw new ArgumentException("Назва не може бути порожньою");

            if (exhibition.StartDate >= exhibition.EndDate)
                throw new ArgumentException("Дата завершення має бути пізніше за дату початку");

            _unitOfWork.Exhibitions.Add(exhibition);
            _unitOfWork.SaveChanges();
        }

        public void UpdateExhibition(Exhibition exhibition)
        {
            if (exhibition == null)
                throw new ArgumentNullException(nameof(exhibition), "Експозиція не може бути null");

            var existingExhibition = _unitOfWork.Exhibitions.GetById(exhibition.ExhibitionId);
            if (existingExhibition == null)
                throw new KeyNotFoundException("Експозицію не знайдено");

            _unitOfWork.Exhibitions.Update(exhibition);
            _unitOfWork.SaveChanges();
        }

        public void DeleteExhibition(int id)
        {
            var exhibition = _unitOfWork.Exhibitions.GetById(id);
            if (exhibition == null)
                throw new KeyNotFoundException("Експозицію не знайдено");

            // Перевірка наявності пов’язаних даних
            bool hasRelatedData = _unitOfWork.Schedules.GetByExhibitionId(id).Any() ||
                                  _unitOfWork.Visits.GetByExhibitionId(id).Any() ||
                                  _unitOfWork.Tours.GetByExhibitionId(id).Any();

            if (hasRelatedData)
                throw new InvalidOperationException("Неможливо видалити експозицію, оскільки існують пов’язані розклади, візити або екскурсії");

            _unitOfWork.Exhibitions.Delete(id);
            _unitOfWork.SaveChanges();
        }

        public Exhibition GetExhibition(int id)
        {
            return _unitOfWork.Exhibitions.GetById(id);
        }

        public IEnumerable<Exhibition> GetAllExhibitions()
        {
            return _unitOfWork.Exhibitions.GetAll();
        }

        public IEnumerable<Exhibition> GetCurrentExhibitions()
        {
            return _unitOfWork.Exhibitions.GetCurrentExhibitions(DateTime.Now);
        }

        public IEnumerable<Exhibition> SearchExhibitionsByTheme(string theme)
        {
            if (string.IsNullOrWhiteSpace(theme))
                return Enumerable.Empty<Exhibition>();

            return _unitOfWork.Exhibitions.GetByTheme(theme);
        }

        public IEnumerable<Exhibition> SearchExhibitionsByAudience(string audience)
        {
            if (string.IsNullOrWhiteSpace(audience))
                return Enumerable.Empty<Exhibition>();

            return _unitOfWork.Exhibitions.GetByTargetAudience(audience);
        }
    }
}
