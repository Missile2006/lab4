using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Museum.DAL.Context;
using Museum.DAL.Entities;
using Museum.DAL.Interfaces;

namespace Museum.DAL.Repositories
{
    public class ScheduleRepository : IScheduleRepository
    {
        private readonly MuseumContext _context;

        public ScheduleRepository(MuseumContext context)
        {
            _context = context;
        }

        public Schedule GetById(int id)
        {
            return _context.Schedules.Find(id);
        }

        public IEnumerable<Schedule> GetAll()
        {
            return _context.Schedules.ToList();
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
                .OrderBy(s => s.StartTime)
                .ToList();
        }

        public void Add(Schedule schedule)
        {
            _context.Schedules.Add(schedule);
        }

        public void Update(Schedule schedule)
        {
            _context.Entry(schedule).State = EntityState.Modified;
        }

        public void Delete(int id)
        {
            var schedule = _context.Schedules.Find(id);
            if (schedule != null)
            {
                _context.Schedules.Remove(schedule);
            }
        }
    }
}