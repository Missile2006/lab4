using AutoMapper;
using Museum.BLL.Models;
using Museum.DAL.Entities;
using Museum.DAL.Interfaces;
using Museum.DAL.UoW;

namespace Museum.BLL.Services
{
    public class VisitService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public VisitService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // CRUD операції
        public void AddVisit(VisitModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (string.IsNullOrWhiteSpace(model.VisitorName))
                throw new ArgumentException("Ім'я відвідувача не може бути порожнім");

            if (model.VisitDate < DateTime.Today)
                throw new ArgumentException("Дата відвідування не може бути в минулому");

            if (model.TicketPrice <= 0)
                throw new ArgumentException("Ціна квитка повинна бути позитивною");

            var exhibition = _unitOfWork.Exhibitions.GetById(model.ExhibitionId);
            if (exhibition == null)
                throw new KeyNotFoundException("Виставку не знайдено");

            var visit = _mapper.Map<Visit>(model);
            _unitOfWork.Visits.Add(visit);
            _unitOfWork.SaveChanges();
        }

        public void DeleteVisit(int id)
        {
            var visit = _unitOfWork.Visits.GetById(id);
            if (visit == null)
                throw new KeyNotFoundException("Відвідування не знайдено");

            _unitOfWork.Visits.Delete(id);
            _unitOfWork.SaveChanges();
        }

        public VisitModel GetVisit(int id)
        {
            var visit = _unitOfWork.Visits.GetById(id);
            return _mapper.Map<VisitModel>(visit);
        }

        public IEnumerable<VisitModel> GetAllVisits()
        {
            var visits = _unitOfWork.Visits.GetAll();
            return _mapper.Map<IEnumerable<VisitModel>>(visits);
        }

        public IEnumerable<VisitModel> SearchVisitsByName(string visitorName)
        {
            if (string.IsNullOrWhiteSpace(visitorName))
                return Enumerable.Empty<VisitModel>();

            var visits = _unitOfWork.Visits.GetByVisitorName(visitorName);
            return _mapper.Map<IEnumerable<VisitModel>>(visits);
        }

        public IEnumerable<VisitModel> SearchVisitsByDateRange(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                throw new ArgumentException("Дата закінчення повинна бути після дати початку");

            var visits = _unitOfWork.Visits.GetByDateRange(startDate, endDate);
            return _mapper.Map<IEnumerable<VisitModel>>(visits);
        }

        public decimal CalculateTotalIncome(DateTime startDate, DateTime endDate)
        {
            var visits = _unitOfWork.Visits.GetByDateRange(startDate, endDate);
            return visits.Sum(v => v.TicketPrice);
        }
    }
}
