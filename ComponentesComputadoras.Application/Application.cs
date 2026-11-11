using ComponentesComputadoras.Abstractions;
using ComponentesComputadoras.Repository;

namespace ComponentesComputadoras.Application
{
    public interface IApplication<T> : IDbOperation<T> { }

    public class Application<T> : IApplication<T>
    {
        private readonly IRepository<T> _repositorio;
        public Application(IRepository<T> repositorio) { _repositorio = repositorio; }

        public Task DeleteAsync(int id) => _repositorio.DeleteAsync(id);
        public Task<IList<T>> GetAllAsync() => _repositorio.GetAllAsync();
        public Task<T?> GetByIdAsync(int id) => _repositorio.GetByIdAsync(id);
        public Task<T> SaveAsync(T entity) => _repositorio.SaveAsync(entity);
    }
}
