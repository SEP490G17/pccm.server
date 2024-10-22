using Application.Interfaces;
using Application.SpecParams;
using Domain.Entity;
using Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repository
{
    public class GenericRepository<T>(DataContext _context) : IGenericRepository<T> where T : BaseEntity
    {


        public async Task<T> GetByIdAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task<IReadOnlyList<T>> ListAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<T> GetEntityWithSpec(ISpecification<T> spec)
        {
            return await ApplySpecification(spec).FirstOrDefaultAsync();
        }

        public async Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec, CancellationToken cancellationToken)
        {
            return await ApplySpecification(spec).ToListAsync(cancellationToken);
        }

        public async Task<int> CountAsync(ISpecification<T> spec, CancellationToken cancellationToken)
        {
            return await ApplySpecification(spec).CountAsync(cancellationToken);
        }

        private IQueryable<T> ApplySpecification(ISpecification<T> spec)
        {
            // Sử dụng đánh giá đặc tả query để tạo câu lệnh phù hợp => apply trả về 1 IQueryable
            return SpecificationEvaluator<T>.GetQuery(_context.Set<T>().AsQueryable(), spec);
        }

        public T Add(T entity)
        {
            _context.Set<T>().Add(entity);
            return _context.Entry(entity).Entity;
        }

        public T Update(T entity)
        {
            _context.Set<T>().Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
            return _context.Entry(entity).Entity;
        }

        public T Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
            return _context.Entry(entity).Entity;
        }

        public IQueryable<T> QueryList(ISpecification<T>? spec)
        {   
            if(spec == null){
                return _context.Set<T>().AsQueryable();
            }
            return ApplySpecification(spec);
        }
    }
}