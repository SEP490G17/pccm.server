using Application.SpecParams;
using Domain.Entity;

namespace Application.Interfaces
{
    public interface IGenericRepository<T> where T : IEntity
    {
        Task<T> GetByIdAsync(int id);
        Task<IReadOnlyList<T>> ListAllAsync();
        Task<T> GetEntityWithSpec(ISpecification<T> spec);
        Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec, CancellationToken cancellationToken);
        IQueryable<T> QueryList(ISpecification<T>? spec);
        Task<int> CountAsync(ISpecification<T> spec, CancellationToken cancellationToken);
        T Add(T entity);
        T Update(T entity);
        T Delete(T entity);
    }
}