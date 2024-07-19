using OrderSystem.Core.Entities;
using OrderSystem.Core.Repositories;

namespace OrderSystem.Core
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity;
        Task<int> CompleteAsync();

    }
}
