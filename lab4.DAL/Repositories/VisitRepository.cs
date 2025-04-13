using Microsoft.EntityFrameworkCore;
using Museum.DAL.Context;
using Museum.DAL.Entities;
using Museum.DAL.Interfaces;

namespace Museum.DAL.Repositories
{
    public class VisitRepository : IVisitRepository
    {
        private readonly MuseumContext _context;

        public VisitRepository(MuseumContext context)
        {
            _context = context;
        }

        public Visit GetById(int id)
        {
            return _context.Visits.Find(id);
        }

        public IEnumerable<Visit> GetAll()
        {
            return _context.Visits.ToList();
        }

        public IEnumerable<Visit> GetByExhibitionId(int exhibitionId)
        {
            return _context.Visits
                .Where(v => v.ExhibitionId == exhibitionId)
                .ToList();
        }

        public IEnumerable<Visit> GetByVisitorName(string visitorName)
        {
            return _context.Visits
                .Where(v => v.VisitorName.Contains(visitorName))
                .ToList();
        }

        public IEnumerable<Visit> GetByDateRange(DateTime startDate, DateTime endDate)
        {
            return _context.Visits
                .Where(v => v.VisitDate >= startDate && v.VisitDate <= endDate)
                .ToList();
        }

        public void Add(Visit visit)
        {
            _context.Visits.Add(visit);
        }

        public void Update(Visit visit)
        {
            _context.Entry(visit).State = EntityState.Modified;
        }

        public void Delete(int id)
        {
            var visit = _context.Visits.Find(id);
            if (visit != null)
            {
                _context.Visits.Remove(visit);
            }
        }
    }
}