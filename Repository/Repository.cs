using MVCLearningsPOC.Data;
using MVCLearningsPOC.Repository.Interface;

namespace MVCLearningsPOC.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly EmployeeDbContext _context;

        public Repository(EmployeeDbContext context)
        {
            _context = context;
        }

        public IEnumerable<T> GetAll()
        {
            return _context.Set<T>().ToList();
        }

        public T GetById(int id)
        {
            return _context.Set<T>().Find(id);
        }

        public void Add(T entity)
        {
            _context.Set<T>().Add(entity);
        }

        public void Update(T entity)
        {
            _context.Set<T>().Update(entity);
        }

        public void Delete(int id)
        {
            var entity = GetById(id);
            if (entity != null)
                _context.Set<T>().Remove(entity);
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}

