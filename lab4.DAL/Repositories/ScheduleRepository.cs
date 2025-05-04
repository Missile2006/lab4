using Museum.DAL.Context;
using Museum.DAL.Entities;
using Museum.DAL.Interfaces;

namespace Museum.DAL.Repositories
{
    public class ScheduleRepository : GenericRepository<Schedule>, IScheduleRepository
    {
        public ScheduleRepository(MuseumContext context) : base(context)
        {
        }

        public IEnumerable<Schedule> GetByExhibitionId(int exhibitionId)
        {
            return _context.Schedules
                .Where(s => s.ExhibitionId == exhibitionId)
                .ToList();
        }

        public IEnumerable<Schedule> GetAvailableSchedules(DateTime date)
        {
            return _context.Schedules
                .Where(s => s.Date == date.Date)
                .AsEnumerable()
                .OrderBy(s => s.StartTime)
                .ToList();
        }
    }
}
