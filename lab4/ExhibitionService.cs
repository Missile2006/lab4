
using lab4.Models;

namespace lab4
{
    public class ExhibitionService
    {
        private readonly MuseumContext _context;
        public ExhibitionService(MuseumContext context) => _context = context;

        public List<Exhibition> GetAll() => _context.Exhibitions.ToList();
        public Exhibition GetById(int id) => _context.Exhibitions.Find(id);
        public void Add(Exhibition ex) { _context.Exhibitions.Add(ex); _context.SaveChanges(); }
        public void Update(Exhibition ex) { _context.Exhibitions.Update(ex); _context.SaveChanges(); }
        public void Delete(int id)
        {
            var ex = _context.Exhibitions.Find(id);
            if (ex != null) { _context.Exhibitions.Remove(ex); _context.SaveChanges(); }
        }
    }

}
