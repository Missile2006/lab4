using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Museum.DAL.Context;
using Museum.DAL.Entities;
using Museum.DAL.Interfaces;

namespace Museum.DAL.Repositories
{
    public class TourRepository : GenericRepository<Tour>, ITourRepository
    {
        public TourRepository(MuseumContext context) : base(context) { }

        public Tour GetByIdWithExhibition(int id)
        {
            return _context.Tours
                .Include(t => t.Exhibition)
                .FirstOrDefault(t => t.TourId == id);
        }

        public IEnumerable<Tour> GetAllWithExhibitions()
        {
            return _context.Tours
                .Include(t => t.Exhibition)
                .ToList();
        }

        public IEnumerable<Tour> GetByExhibitionId(int exhibitionId)
        {
            return _context.Tours
                .Where(t => t.ExhibitionId == exhibitionId)
                .ToList();
        }

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

    }
}
