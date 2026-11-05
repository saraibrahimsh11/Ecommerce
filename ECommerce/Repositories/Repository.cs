using ECommerce.Data;
using ECommerce.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Update.Internal;
using System.Linq.Expressions;

namespace ECommerce.Repositories
{
    public class Repository<T> :IRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _context;// = new ApplicationDbContext();
        private DbSet<T> _dbSet;
        public Repository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }
        public async Task<EntityEntry<T>> AddAsync(T entity)
        {
            return await _dbSet.AddAsync(entity);
        }

        public async Task<IEnumerable<T>> GetListAsync(
            Expression<Func<T, bool>>? filter = null,
            Expression<Func<T, object>>[]? includes = null,
            bool isTracked = true
            )
        {
            var entities = Query(filter, includes, isTracked);
            return await entities.ToListAsync();
        }

        public async Task<T> GetOneAsync(
            Expression<Func<T, bool>>? filter = null,
            Expression<Func<T, object>>[]? includes = null,
            bool isTracked = true
            )
        {
            var entities = Query(filter, includes, isTracked);
            return await entities.FirstOrDefaultAsync();
        }

        public IQueryable<T> Query(
            Expression<Func<T, bool>>? filter = null,
            Expression<Func<T, object>>[]? includes = null,
            bool isTracked = true
            )
        {
            var entities = _dbSet.AsQueryable();

            // filter 
            if (filter != null)
                entities = entities.Where(filter);

            if (includes != null)
            {
                foreach (var include in includes)
                {
                    entities = entities.Include(include);
                }
            }

            if (!isTracked)
            {
                entities = entities.AsNoTracking();
            }
            return entities;
        }
        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public void Remove(T entity)
        {
            _dbSet.Remove(entity);
        }

        public async Task<int> CommitAsync()
        {
            try
            {
                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return -1;
            }
        }
    }
}
