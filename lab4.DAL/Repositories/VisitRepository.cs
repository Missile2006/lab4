using System;
using System.Collections.Generic;
using System.Linq;
using Museum.DAL.Context;
using Museum.DAL.Entities;
using Museum.DAL.Interfaces;

namespace Museum.DAL.Repositories
{
    public class VisitRepository : GenericRepository<Visit>, IVisitRepository
    {
        public VisitRepository(MuseumContext context) : base(context) { }

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
    }
}
