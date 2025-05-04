using Museum.DAL.Context;
using Museum.DAL.Interfaces;
using Museum.DAL.Repositories;

namespace Museum.DAL.UoW
{
    public class UnitOfWork : IDisposable, IUnitOfWork
    {
        private readonly MuseumContext _context;

        public UnitOfWork(MuseumContext context)
        {
            _context = context;
            Exhibitions = new ExhibitionRepository(_context);
            Schedules = new ScheduleRepository(_context);
            Visits = new VisitRepository(_context);
            Tours = new TourRepository(_context);
        }

        public IExhibitionRepository Exhibitions { get; }
        public IScheduleRepository Schedules { get; }
        public IVisitRepository Visits { get; }
        public ITourRepository Tours { get; }

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed && disposing)
            {
                _context.Dispose();
            }
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}