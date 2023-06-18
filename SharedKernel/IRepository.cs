using System;
using System.Threading.Tasks;

namespace SharedKernel
{
    public interface IRepository<T> : IDisposable where T : Entity
    {
        Task<T> FindByIdAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        bool Exists(Guid id);
        Task<T> AddAsync(T entity);
        void Update(T entity);
    }
}