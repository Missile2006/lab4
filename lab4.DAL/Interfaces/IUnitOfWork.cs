using System;

namespace Museum.DAL.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IExhibitionRepository Exhibitions { get; }
        IScheduleRepository Schedules { get; }
        IVisitRepository Visits { get; }
        ITourRepository Tours { get; }

        int SaveChanges();
    }
}