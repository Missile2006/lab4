using Microsoft.EntityFrameworkCore;
using Museum.DAL.Context;
using Museum.DAL.Entities;
using Museum.DAL.Interfaces;

namespace Museum.DAL.Repositories
{
    public class TourRepository : ITourRepository
    {
        private readonly MuseumContext _context;

        public TourRepository(MuseumContext context)
        {
            _context = context;
        }

        public Tour GetById(int id)
        {
            return _context.Tours.Find(id);
        }

        public IEnumerable<Tour> GetAll()
        {
            return _context.Tours.ToList();
        }

        public IEnumerable<Tour> GetByExhibitionId(int exhibitionId)
        {
            return _context.Tours
                .Where(t => t.ExhibitionId == exhibitionId)
                .ToList();
        }

        public IEnumerable<Tour> GetByGuideName(string guideName)
        {
            return _context.Tours
                .Where(t => t.GuideName.Contains(guideName))
                .ToList();
        }

        public IEnumerable<Tour> GetPrivateTours()
        {
            return _context.Tours
                .Where(t => t.IsPrivate)
                .ToList();
        }

        public IEnumerable<Tour> GetScheduledTours()
        {
            return _context.Tours
                .Where(t => t.TourDate >= DateTime.Today)
                .OrderBy(t => t.TourDate)
                .ToList();
        }

        public void Add(Tour tour)
        {
            _context.Tours.Add(tour);
        }

        public void Update(Tour tour)
        {
            _context.Entry(tour).State = EntityState.Modified;
        }

        public void Delete(int id)
        {
            var tour = _context.Tours.Find(id);
            if (tour != null)
            {
                _context.Tours.Remove(tour);
            }
        }
    }
}