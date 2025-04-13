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
            _unitOfWork.Tours.Add(tour);
            _unitOfWork.SaveChanges();
        }

        // Додати екскурсію
        public void AddTour(Tour tour)
        {
            if (tour == null)
                throw new ArgumentNullException(nameof(tour), "Екскурсія не може бути null");

            if (string.IsNullOrWhiteSpace(tour.GuideName))
                throw new ArgumentException("Ім’я гіда не може бути порожнім");

            if (tour.TourDate < DateTime.Today)
                throw new ArgumentException("Дата екскурсії не може бути в минулому");

            if (tour.Price <= 0)
                throw new ArgumentException("Ціна повинна бути додатною");

            var exhibition = _unitOfWork.Exhibitions.GetById(tour.ExhibitionId);
            if (exhibition == null)
                throw new KeyNotFoundException("Експозицію не знайдено");

            _unitOfWork.Tours.Add(tour);
            _unitOfWork.SaveChanges();
        }

        // Видалити екскурсію
        public void DeleteTour(int id)
        {
            var tour = _unitOfWork.Tours.GetById(id);
            if (tour == null)
                throw new KeyNotFoundException("Екскурсію не знайдено");

            _unitOfWork.Tours.Delete(id);
            _unitOfWork.SaveChanges();
        }

        // Отримати екскурсію за ID
        public Tour GetTour(int id)
        {
            return _unitOfWork.Tours.GetById(id);
        }

        // Отримати всі екскурсії
        public IEnumerable<Tour> GetAllTours()
        {
            return _unitOfWork.Tours.GetAll();
        }

        // Отримати всі екскурсії певної експозиції
        public IEnumerable<Tour> GetExhibitionTours(int exhibitionId)
        {
            return _unitOfWork.Tours.GetByExhibitionId(exhibitionId);
        }

        // Отримати всі приватні екскурсії
        public IEnumerable<Tour> GetPrivateTours()
        {
            return _unitOfWork.Tours.GetPrivateTours();
        }

        // Отримати всі заплановані екскурсії
        public IEnumerable<Tour> GetScheduledTours()
        {
            return _unitOfWork.Tours.GetScheduledTours();
        }

        // Пошук екскурсій за ім’ям гіда
        public IEnumerable<Tour> SearchToursByGuide(string guideName)
        {
            if (string.IsNullOrWhiteSpace(guideName))
                return Enumerable.Empty<Tour>();

            return _unitOfWork.Tours.GetByGuideName(guideName);
        }

        // Порахувати прибуток від екскурсій за вказаний період
        public decimal CalculateTourIncome(DateTime startDate, DateTime endDate)
        {
            var tours = _unitOfWork.Tours.GetAll()
                .Where(t => t.TourDate >= startDate && t.TourDate <= endDate)
                .ToList();

            return tours.Sum(t => t.Price);
        }
    }
}
