using Museum.DAL.Entities;
using Museum.DAL.Repositories;

namespace Museum.DAL.Interfaces
{
    public interface IScheduleRepository : IGenericRepository<Schedule>
    {
        IEnumerable<Schedule> GetByExhibitionId(int exhibitionId);
        IEnumerable<Schedule> GetAvailableSchedules(DateTime date);
    }
}
