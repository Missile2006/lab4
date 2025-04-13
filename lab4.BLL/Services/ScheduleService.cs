using Museum.DAL.Entities;
using Museum.DAL.Interfaces;

namespace Museum.BLL.Services
{
    public class ScheduleService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ScheduleService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // Додати розклад
        public void AddSchedule(Schedule schedule)
        {
            if (schedule == null)
                throw new ArgumentNullException(nameof(schedule), "Розклад не може бути null");

            if (schedule.StartTime >= schedule.EndTime)
                throw new ArgumentException("Час завершення має бути пізніше за час початку");

            var exhibition = _unitOfWork.Exhibitions.GetById(schedule.ExhibitionId);
            if (exhibition == null)
                throw new KeyNotFoundException("Експозицію не знайдено");

            _unitOfWork.Schedules.Add(schedule);
            _unitOfWork.SaveChanges();
        }

        // Видалити розклад за ідентифікатором
        public void DeleteSchedule(int id)
        {
            var schedule = _unitOfWork.Schedules.GetById(id);
            if (schedule == null)
                throw new KeyNotFoundException("Розклад не знайдено");

            _unitOfWork.Schedules.Delete(id);
            _unitOfWork.SaveChanges();
        }

        // Отримати розклад за ідентифікатором
        public Schedule GetSchedule(int id)
        {
            return _unitOfWork.Schedules.GetById(id);
        }

        // Отримати всі розклади
        public IEnumerable<Schedule> GetAllSchedules()
        {
            return _unitOfWork.Schedules.GetAll();
        }

        // Отримати розклади для певної експозиції
        public IEnumerable<Schedule> GetExhibitionSchedules(int exhibitionId)
        {
            return _unitOfWork.Schedules.GetByExhibitionId(exhibitionId);
        }

        // Отримати доступні розклади на певну дату
        public IEnumerable<Schedule> GetAvailableSchedules(DateTime date)
        {
            return _unitOfWork.Schedules.GetAvailableSchedules(date);
        }

        // Перевірити, чи вільний часовий слот
        public bool IsTimeSlotAvailable(int exhibitionId, DateTime date, TimeSpan startTime, TimeSpan endTime)
        {
            var existingSchedules = _unitOfWork.Schedules.GetByExhibitionId(exhibitionId)
                .Where(s => s.Date == date.Date &&
                           ((s.StartTime <= startTime && s.EndTime > startTime) ||
                            (s.StartTime < endTime && s.EndTime >= endTime) ||
                            (s.StartTime >= startTime && s.EndTime <= endTime)))
                .ToList();

            return !existingSchedules.Any();
        }
    }
}
