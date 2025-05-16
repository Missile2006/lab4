using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Museum.DAL.Entities;

namespace Museum.DAL.Interfaces
{
    public interface IGenericRepository<TModel> where TModel : class
    {
        TModel GetById(int id);
        IEnumerable<TModel> GetAll();
        void Add(TModel model);
        void Update(TModel model);
        void Delete(int id);
    }
}
