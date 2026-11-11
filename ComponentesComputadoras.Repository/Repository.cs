using ComponentesComputadoras.Abstractions;

namespace ComponentesComputadoras.Repository
{
    public interface IRepository<T> : IDbOperation<T> { }

    public class Repository<T> : IRepository<T> where T : class, IEntidad
    {
        private readonly IDbContext<T> _db;
        public Repository(IDbContext<T> db) { _db = db; }

        public Task DeleteAsync(int id) => _db.DeleteAsync(id);
        public Task<IList<T>> GetAllAsync() => _db.GetAllAsync();
        public Task<T?> GetByIdAsync(int id) => _db.GetByIdAsync(id);
        public Task<T> SaveAsync(T entity) => _db.SaveAsync(entity);
    }
}
