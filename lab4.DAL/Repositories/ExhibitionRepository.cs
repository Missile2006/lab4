using Microsoft.EntityFrameworkCore;
using Museum.DAL.Context;
using Museum.DAL.Entities;
using Museum.DAL.Interfaces;

namespace Museum.DAL.Repositories
{
    public class ExhibitionRepository : IExhibitionRepository
    {
        private readonly MuseumContext _context;

        public ExhibitionRepository(MuseumContext context)
        {
            _context = context;
        }

        public Exhibition GetById(int id)
        {
            return _context.Exhibitions.Find(id);
        }

        public IEnumerable<Exhibition> GetAll()
        {
            return _context.Exhibitions.ToList();
        }

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

        public void Add(Exhibition exhibition)
        {
            _context.Exhibitions.Add(exhibition);
        }

        public void Update(Exhibition exhibition)
        {
            _context.Entry(exhibition).State = EntityState.Modified;
        }

        public void Delete(int id)
        {
            var exhibition = _context.Exhibitions.Find(id);
            if (exhibition != null)
            {
                _context.Exhibitions.Remove(exhibition);
            }
        }
    }
}