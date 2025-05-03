using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Museum.DAL.Context;

namespace Museum.DAL.Repositories
{
    public class GenericRepository<TModel> : IGenericRepository<TModel> where TModel : class
    {
        protected readonly MuseumContext _context;
        protected readonly DbSet<TModel> _dbSet;

        public GenericRepository(MuseumContext context)
        {
            _context = context;
            _dbSet = _context.Set<TModel>();
        }

        public virtual TModel GetById(int id)
        {
            return _dbSet.Find(id);
        }

        public virtual IEnumerable<TModel> GetAll()
        {
            return _dbSet.ToList();
        }

        public virtual void Add(TModel model)
        {
            _dbSet.Add(model);
        }

        public virtual void Update(TModel model)
        {
            _context.Entry(model).State = EntityState.Modified;
        }

        public virtual void Delete(int id)
        {
            var model = GetById(id);
            if (model != null)
            {
                _dbSet.Remove(model);
            }
        }
    }
}
