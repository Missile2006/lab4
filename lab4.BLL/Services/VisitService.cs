using Museum.DAL.Entities;
using Museum.DAL.Interfaces;

namespace Museum.BLL.Services
{
    public class VisitService
    {
        private readonly IUnitOfWork _unitOfWork;

        public VisitService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void AddVisit(Visit visit)
        {
            if (visit == null)
                throw new ArgumentNullException(nameof(visit));

            if (string.IsNullOrWhiteSpace(visit.VisitorName))
                throw new ArgumentException("Ім'я відвідувача не може бути порожнім");

            if (visit.VisitDate < DateTime.Today)
                throw new ArgumentException("Дата відвідування не може бути в минулому");

            if (visit.TicketPrice <= 0)
                throw new ArgumentException("Ціна квитка повинна бути позитивною");

            var exhibition = _unitOfWork.Exhibitions.GetById(visit.ExhibitionId);
            if (exhibition == null)
                throw new KeyNotFoundException("Виставку не знайдено");

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

        public Visit GetVisit(int id)
        {
            return _unitOfWork.Visits.GetById(id);
        }

        public IEnumerable<Visit> GetAllVisits()
        {
            return _unitOfWork.Visits.GetAll();
        }

        public IEnumerable<Visit> SearchVisitsByName(string visitorName)
        {
            if (string.IsNullOrWhiteSpace(visitorName))
                return Enumerable.Empty<Visit>();

            return _unitOfWork.Visits.GetByVisitorName(visitorName);
        }

        public IEnumerable<Visit> SearchVisitsByDateRange(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                throw new ArgumentException("Дата закінчення повинна бути після дати початку");

            return _unitOfWork.Visits.GetByDateRange(startDate, endDate);
        }

        public decimal CalculateTotalIncome(DateTime startDate, DateTime endDate)
        {
            var visits = _unitOfWork.Visits.GetByDateRange(startDate, endDate);
            return visits.Sum(v => v.TicketPrice);
        }
    }
}
