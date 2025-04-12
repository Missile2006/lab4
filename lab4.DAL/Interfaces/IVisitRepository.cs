using System;
using System.Collections.Generic;
using Museum.DAL.Entities;

namespace Museum.DAL.Interfaces
{
    public interface IVisitRepository
    {
        Visit GetById(int id);
        IEnumerable<Visit> GetAll();
        IEnumerable<Visit> GetByExhibitionId(int exhibitionId);
        IEnumerable<Visit> GetByVisitorName(string visitorName);
        IEnumerable<Visit> GetByDateRange(DateTime startDate, DateTime endDate);

        void Add(Visit visit);
        void Update(Visit visit);
        void Delete(int id);
    }
}