using System;
using System.Collections.Generic;
using Museum.DAL.Entities;

namespace Museum.DAL.Interfaces
{
    public interface ITourRepository
    {
        Tour GetById(int id);
        IEnumerable<Tour> GetAll();
        IEnumerable<Tour> GetByExhibitionId(int exhibitionId);
        IEnumerable<Tour> GetByGuideName(string guideName);
        IEnumerable<Tour> GetPrivateTours();
        IEnumerable<Tour> GetScheduledTours();

        void Add(Tour tour);
        void Update(Tour tour);
        void Delete(int id);
    }
}