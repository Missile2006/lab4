using System;
using System.Collections.Generic;
using Museum.DAL.Entities;

namespace Museum.DAL.Interfaces
{
    public interface IScheduleRepository
    {
        Schedule GetById(int id);
        IEnumerable<Schedule> GetAll();
        IEnumerable<Schedule> GetByExhibitionId(int exhibitionId);
        IEnumerable<Schedule> GetAvailableSchedules(DateTime date);

        void Add(Schedule schedule);
        void Update(Schedule schedule);
        void Delete(int id);
    }
}