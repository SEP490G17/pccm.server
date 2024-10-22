using Domain.Entity;

namespace Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<TEntity> Repository<TEntity>() where TEntity : class, IEntity;
        Task<int> Complete();
    }
}