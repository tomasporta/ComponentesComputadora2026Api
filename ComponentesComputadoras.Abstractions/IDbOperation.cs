namespace ComponentesComputadoras.Abstractions
{
    public interface IDbOperation<T>
    {
        Task<T> SaveAsync(T entity);
        Task<IList<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);
        Task DeleteAsync(int id);
    }
}
