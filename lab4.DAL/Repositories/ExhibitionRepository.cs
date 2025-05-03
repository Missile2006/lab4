using System;
using System.Collections.Generic;
using System.Linq;
using Museum.DAL.Context;
using Museum.DAL.Entities;
using Museum.DAL.Interfaces;

namespace Museum.DAL.Repositories
{
    public class ExhibitionRepository : GenericRepository<Exhibition>, IExhibitionRepository
    {
        public ExhibitionRepository(MuseumContext context) : base(context) { }

        public IEnumerable<Exhibition> GetByTheme(string theme)
        {
            return _context.Exhibitions
                .Where(e => e.Theme.Contains(theme))
                .ToList();
        }

        public IEnumerable<Exhibition> GetByTargetAudience(string audience)
        {
            return _context.Exhibitions
                .Where(e => e.TargetAudience.Contains(audience))
                .ToList();
        }

        public IEnumerable<Exhibition> GetCurrentExhibitions(DateTime currentDate)
        {
            return _context.Exhibitions
                .Where(e => e.StartDate <= currentDate && e.EndDate >= currentDate)
                .ToList();
        }
    }
}
