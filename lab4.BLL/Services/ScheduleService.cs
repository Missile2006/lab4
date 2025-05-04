using AutoMapper;
using Museum.DAL.Entities;
using Museum.DAL.Interfaces;
using Museum.BLL.Models;
using Museum.DAL.UoW; // припустимо, тут лежить ScheduleModel

namespace Museum.BLL.Services
{
    public class ScheduleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ScheduleService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // Додати розклад
        public void AddSchedule(ScheduleModel scheduleModel)
        {
            if (scheduleModel == null)
                throw new ArgumentNullException(nameof(scheduleModel), "Розклад не може бути null");

            if (scheduleModel.StartTime >= scheduleModel.EndTime)
                throw new ArgumentException("Час завершення має бути пізніше за час початку");

            // Отримуємо виставку за ID
            var exhibition = _unitOfWork.Exhibitions.GetById(scheduleModel.ExhibitionId);

            // Якщо виставки немає — викидаємо помилку з поясненням
            if (exhibition == null)
                throw new KeyNotFoundException($"Експозицію з ID={scheduleModel.ExhibitionId} не знайдено. Перевірте правильність ID.");

            // Якщо все добре — створюємо розклад
            var schedule = _mapper.Map<Schedule>(scheduleModel);
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
        public ScheduleModel GetSchedule(int id)
        {
            var schedule = _unitOfWork.Schedules.GetById(id);
            if (schedule == null)
                throw new KeyNotFoundException("Розклад не знайдено");

            return _mapper.Map<ScheduleModel>(schedule);
        }

        // Отримати всі розклади
        public IEnumerable<ScheduleModel> GetAllSchedules()
        {
            var schedules = _unitOfWork.Schedules.GetAll();
            return _mapper.Map<IEnumerable<ScheduleModel>>(schedules);
        }

        // Отримати розклади для певної експозиції
        public IEnumerable<ScheduleModel> GetExhibitionSchedules(int exhibitionId)
        {
            var schedules = _unitOfWork.Schedules.GetByExhibitionId(exhibitionId);
            return _mapper.Map<IEnumerable<ScheduleModel>>(schedules);
        }

        // Отримати доступні розклади на певну дату
        public IEnumerable<ScheduleModel> GetAvailableSchedules(DateTime date)
        {
            // Отримуємо розклади без сортування в SQL
            var schedules = _unitOfWork.Schedules.GetAvailableSchedules(date);

            // Сортуємо в пам'яті за часом початку
            var sortedSchedules = schedules
                .OrderBy(s => s.StartTime)
                .ToList();

            return _mapper.Map<IEnumerable<ScheduleModel>>(sortedSchedules);
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
