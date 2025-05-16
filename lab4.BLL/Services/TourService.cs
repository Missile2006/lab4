using AutoMapper;
using Museum.BLL.Models;
using Museum.DAL.Entities;
using Museum.DAL.Interfaces;
using Museum.BLL.Strategies;
using Museum.BLL.Strategies.Pricing;
using Museum.BLL.Strategies.Models;

namespace Museum.BLL.Services
{
    public class TourService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly TourPlanner _tourPlanner;

        public TourService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _tourPlanner = new TourPlanner(new GroupTourPricingStrategy());
        }

        // Методи для планування турів
        public void PlanGroupTour(TourModel model)
        {
            _tourPlanner.SetPricingStrategy(new GroupTourPricingStrategy());
            CreateAndSaveTour(model);
        }

        public void PlanPrivateTour(TourModel model)
        {
            _tourPlanner.SetPricingStrategy(new PrivateTourPricingStrategy());
            CreateAndSaveTour(model);
        }

        private void CreateAndSaveTour(TourModel model)
        {
            var tourCreationModel = _mapper.Map<TourCreationModel>(model);
            var tour = _tourPlanner.CreateTour(tourCreationModel);
            AddTour(tour); 
        }

        // Перевантажений метод
        private void AddTour(Tour tour)
        {
            ValidateTour(tour);
            _unitOfWork.Tours.Add(tour);
            _unitOfWork.SaveChanges();
        }

        // CRUD операції
        public void AddTour(TourModel model)
        {
            var tour = _mapper.Map<Tour>(model);  
            ValidateTour(tour);
            _unitOfWork.Tours.Add(tour);
            _unitOfWork.SaveChanges();
        }

        public void UpdateTour(TourModel model)
        {
            var tour = _mapper.Map<Tour>(model);  
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
        public TourModel GetTour(int id, bool eagerLoading = false)
        {
            var tour = eagerLoading
                ? _unitOfWork.Tours.GetByIdWithExhibition(id)
                : _unitOfWork.Tours.GetById(id);

            return _mapper.Map<TourModel>(tour);  
        }

        public IEnumerable<TourModel> GetAllTours(bool includeExhibitions = false)
        {
            var tours = includeExhibitions
                ? _unitOfWork.Tours.GetAllWithExhibitions()  
                : _unitOfWork.Tours.GetAll();

            return _mapper.Map<IEnumerable<TourModel>>(tours);  
        }

        public IEnumerable<TourModel> GetExhibitionTours(int exhibitionId, bool eagerLoading = false)
        {
            var tours = eagerLoading
                ? _unitOfWork.Tours.GetByExhibitionIdWithExhibition(exhibitionId) 
                : _unitOfWork.Tours.GetByExhibitionId(exhibitionId);

            return _mapper.Map<IEnumerable<TourModel>>(tours);  
        }

        public IEnumerable<TourModel> GetPrivateTours(bool eagerLoading = false)
        {
            var tours = _unitOfWork.Tours.GetPrivateTours();
            return eagerLoading
                ? IncludeExhibitions(tours)
                : _mapper.Map<IEnumerable<TourModel>>(tours);  
        }

        public IEnumerable<TourModel> GetScheduledTours(bool eagerLoading = false)
        {
            var tours = _unitOfWork.Tours.GetScheduledTours();
            return eagerLoading
                ? IncludeExhibitions(tours)
                : _mapper.Map<IEnumerable<TourModel>>(tours);  
        }

        public IEnumerable<TourModel> SearchToursByGuide(string guideName, bool eagerLoading = false)
        {
            if (string.IsNullOrWhiteSpace(guideName))
                return Enumerable.Empty<TourModel>();

            var tours = _unitOfWork.Tours.GetByGuideName(guideName);
            return eagerLoading
                ? IncludeExhibitions(tours)
                : _mapper.Map<IEnumerable<TourModel>>(tours);  
        }

        public decimal CalculateTourIncome(DateTime startDate, DateTime endDate)
        {
            return _unitOfWork.Tours.GetAll()
                .Where(t => t.TourDate >= startDate && t.TourDate <= endDate)
                .Sum(t => t.Price);
        }

        private IEnumerable<TourModel> IncludeExhibitions(IEnumerable<Tour> tours)
        {
            // Жадібне завантаження для існуючої колекції
            var tourIds = tours.Select(t => t.TourId).ToList();
            var toursWithExhibitions = _unitOfWork.Tours.GetAllWithExhibitions()
                .Where(t => tourIds.Contains(t.TourId));
            return _mapper.Map<IEnumerable<TourModel>>(toursWithExhibitions);  
        }
        public IEnumerable<TourModel> GetAllToursWithExhibitions()
        {
            var tours = _unitOfWork.Tours.GetAllWithExhibitions(); 
            return _mapper.Map<IEnumerable<TourModel>>(tours);
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
