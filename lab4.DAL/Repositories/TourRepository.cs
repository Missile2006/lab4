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

        /// <summary>
        /// Базовий метод отримання туру (ліниве завантаження)
        /// Пов'язані дані Exhibition не завантажуються одразу
        /// Використовувати, коли не потрібні дані про виставку
        /// </summary>
        public Tour GetById(int id)
        {
            return _context.Tours.Find(id);
        }

        /// <summary>
        /// Отримання туру з жадібним завантаженням Exhibition
        /// Використовувати, коли потрібні дані про виставку
        /// </summary>
        public Tour GetByIdWithExhibition(int id)
        {
            return _context.Tours
                .Include(t => t.Exhibition)
                .FirstOrDefault(t => t.TourId == id);
        }

        /// <summary>
        /// Базовий метод отримання всіх турів (ліниве завантаження)
        /// Використовувати для списків, де не потрібні деталі виставок
        /// </summary>
        public IEnumerable<Tour> GetAll()
        {
            return _context.Tours.ToList();
        }

        /// <summary>
        /// Отримання турів з жадібним завантаженням Exhibition
        /// Використовувати, коли потрібен список турів з даними про виставки
        /// </summary>
        public IEnumerable<Tour> GetAllWithExhibitions()
        {
            return _context.Tours
                .Include(t => t.Exhibition)
                .ToList();
        }

        /// <summary>
        /// Отримання турів для виставки (ліниве завантаження)
        /// </summary>
        public IEnumerable<Tour> GetByExhibitionId(int exhibitionId)
        {
            return _context.Tours
                .Where(t => t.ExhibitionId == exhibitionId)
                .ToList();
        }

        /// <summary>
        /// Отримання турів для виставки з жадібним завантаженням Exhibition
        /// </summary>
        public IEnumerable<Tour> GetByExhibitionIdWithExhibition(int exhibitionId)
        {
            return _context.Tours
                .Where(t => t.ExhibitionId == exhibitionId)
                .Include(t => t.Exhibition)
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