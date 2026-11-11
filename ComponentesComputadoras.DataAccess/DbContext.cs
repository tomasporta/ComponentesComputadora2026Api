using ComponentesComputadoras.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace ComponentesComputadoras.DataAccess
{
    public class DbContext<T> : IDbContext<T> where T : class, IEntidad
    {
        private readonly DbSet<T> _items;
        private readonly DbDataAccess _ctx;

        public DbContext(DbDataAccess ctx)
        {
            _ctx = ctx;
            _items = _ctx.Set<T>();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _items.FirstOrDefaultAsync(i => i.Id == id);
            if (entity != null)
            {
                _items.Remove(entity);
                await _ctx.SaveChangesAsync();
            }
        }

        public async Task<IList<T>> GetAllAsync()
        {
            return await _items.ToListAsync();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _items.FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<T> SaveAsync(T entity)
        {
            if (entity.Id == 0)
            {
                // Alta: se agrega junto con sus relaciones nuevas (ej: los detalles de una compra)
                await _items.AddAsync(entity);
            }
            else if (_ctx.Entry(entity).State == EntityState.Detached)
            {
                // Edicion de una entidad que no viene de este contexto
                _items.Update(entity);
            }
            // Si la entidad se obtuvo con GetById, EF ya la sigue y detecta solo los cambios
            await _ctx.SaveChangesAsync();
            return entity;
        }
    }
}
