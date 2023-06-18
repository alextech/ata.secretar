using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Ata.Investment.Allocation.Application
{
    public class GenericRepository<T, TC> : IRepository<T>
        where T : Entity
        where TC : DbContext
    {
        private readonly TC _dbContext;

        public GenericRepository(TC dbContext)
        {
            _dbContext = dbContext;
        }

        public T FindById(Guid id)
        {
            return _dbContext.Set<T>().FirstOrDefault(e => e.Guid == id);
        }

        public async Task<T> FindByIdAsync(Guid id)
        {
            return await _dbContext.Set<T>().FirstOrDefaultAsync(e => e.Guid == id);
        }

        public bool Exists(Guid id)
        {
            return _dbContext.Set<T>().Any(e => e.Guid == id);
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _dbContext.Set<T>().AnyAsync(e => e.Guid == id);
        }

        public async Task<T> AddAsync(T entity)
        {
            await _dbContext.Set<T>().AddAsync(entity);

            return entity;
        }

        public void Update(T entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
        }

        public void Flush()
        {
            _dbContext.SaveChanges();
        }

        public async Task FlushAsync(CancellationToken cancellationToken = default)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}