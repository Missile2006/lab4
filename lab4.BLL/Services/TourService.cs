using Museum.BLL.Strategies;
using Museum.BLL.Strategies.Models;
using Museum.BLL.Strategies.Pricing;
using Museum.DAL.Entities;
using Museum.DAL.Interfaces;

namespace Museum.BLL.Services
{
    public class TourService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly TourPlanner _tourPlanner;

        public TourService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _tourPlanner = new TourPlanner(new GroupTourPricingStrategy());
        }

        // Методи для планування турів
        public void PlanGroupTour(TourCreationModel model)
        {
            _tourPlanner.SetPricingStrategy(new GroupTourPricingStrategy());
            CreateAndSaveTour(model);
        }

        public void PlanPrivateTour(TourCreationModel model)
        {
            _tourPlanner.SetPricingStrategy(new PrivateTourPricingStrategy());
            CreateAndSaveTour(model);
        }

        private void CreateAndSaveTour(TourCreationModel model)
        {
            var tour = _tourPlanner.CreateTour(model);
            AddTour(tour);
        }

        // CRUD операції
        public void AddTour(Tour tour)
        {
            ValidateTour(tour);
            _unitOfWork.Tours.Add(tour);
            _unitOfWork.SaveChanges();
        }

        public void UpdateTour(Tour tour)
        {
            ValidateTour(tour);
            _unitOfWork.Tours.Update(tour);
            _unitOfWork.SaveChanges();
        }

        public void DeleteTour(int id)
        {
            var tour = GetTour(id, true); // Жадібне завантаження для перевірки залежностей
            if (tour == null)
                throw new KeyNotFoundException("Екскурсію не знайдено");

            _unitOfWork.Tours.Delete(id);
            _unitOfWork.SaveChanges();
        }

        // Методи для отримання даних з різними типами завантаження
        public Tour GetTour(int id, bool eagerLoading = false)
        {
            return eagerLoading
                ? _unitOfWork.Tours.GetByIdWithExhibition(id)
                : _unitOfWork.Tours.GetById(id);
        }

        public Tour GetTourWithExhibition(int id)
        {
            return _unitOfWork.Tours.GetByIdWithExhibition(id);
        }

        public IEnumerable<Tour> GetAllToursWithExhibitions()
        {
            return _unitOfWork.Tours.GetAllWithExhibitions();
        }

        public IEnumerable<Tour> GetAllTours(bool includeExhibitions = false)
        {
            return includeExhibitions
                ? _unitOfWork.Tours.GetAllWithExhibitions()
                : _unitOfWork.Tours.GetAll();
        }

        // Спеціалізовані методи
        public IEnumerable<Tour> GetExhibitionTours(int exhibitionId, bool eagerLoading = false)
        {
            return eagerLoading
                ? _unitOfWork.Tours.GetByExhibitionIdWithExhibition(exhibitionId)
                : _unitOfWork.Tours.GetByExhibitionId(exhibitionId);
        }

        public IEnumerable<Tour> GetPrivateTours(bool eagerLoading = false)
        {
            var tours = _unitOfWork.Tours.GetPrivateTours();
            return eagerLoading
                ? IncludeExhibitions(tours)
                : tours;
        }

        public IEnumerable<Tour> GetScheduledTours(bool eagerLoading = false)
        {
            var tours = _unitOfWork.Tours.GetScheduledTours();
            return eagerLoading
                ? IncludeExhibitions(tours)
                : tours;
        }

        public IEnumerable<Tour> SearchToursByGuide(string guideName, bool eagerLoading = false)
        {
            if (string.IsNullOrWhiteSpace(guideName))
                return Enumerable.Empty<Tour>();

            var tours = _unitOfWork.Tours.GetByGuideName(guideName);
            return eagerLoading
                ? IncludeExhibitions(tours)
                : tours;
        }

        public decimal CalculateTourIncome(DateTime startDate, DateTime endDate)
        {
            return _unitOfWork.Tours.GetAll()
                .Where(t => t.TourDate >= startDate && t.TourDate <= endDate)
                .Sum(t => t.Price);
        }

        // Допоміжні методи
        private IEnumerable<Tour> IncludeExhibitions(IEnumerable<Tour> tours)
        {
            // Жадібне завантаження для існуючої колекції
            var tourIds = tours.Select(t => t.TourId).ToList();
            return _unitOfWork.Tours.GetAllWithExhibitions()
                .Where(t => tourIds.Contains(t.TourId));
        }

        private void ValidateTour(Tour tour)
        {
            if (tour == null)
                throw new ArgumentNullException(nameof(tour), "Екскурсія не може бути null");

            if (string.IsNullOrWhiteSpace(tour.GuideName))
                throw new ArgumentException("Ім'я гіда не може бути порожнім");

            if (tour.TourDate < DateTime.Today)
                throw new ArgumentException("Дата екскурсії не може бути в минулому");

            if (tour.Price <= 0)
                throw new ArgumentException("Ціна повинна бути додатною");

            if (_unitOfWork.Exhibitions.GetById(tour.ExhibitionId) == null)
                throw new KeyNotFoundException("Експозицію не знайдено");
        }
    }
}