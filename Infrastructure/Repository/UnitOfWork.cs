using System.Collections;
using Application.Interfaces;
using Domain.Entity;
using Persistence;
using Persistence.Repository;

namespace Infrastructure.Repository
{
    public class UnitOfWork(DataContext _context) : IUnitOfWork
    {
        private Hashtable _repositories; // dùng để lưu trữ các thể hiện của repository, tránh việc tạo 2 repository mỗi khi gọi
        /*
            Ví dụ: Khi call api lấy danh sách banner
            1, Lấy số lượng banner sẽ phải gọi lệnh:
                var totalItem = await _unitOfWork.Repository<Banner>().CountAsync(countSpec, cancellationToken);
                => lúc này đã tạo ra 1 thể hiện (instance) của Repository Banner
            2, Sau đó lại lấy danh sách banner phân trang theo đặc tả:
                var banners = await _unitOfWork.Repository<Banner>().ListAsync(spec, cancellationToken);
                => không tạo ra thể hiện mới của Repository Banner mà sử dụng cái trước đó
                Cách lấy là tìm theo key

        */

        public async Task<int> Complete()
        {
            // Lệnh save changes sẽ được thực hiện ở đây để kết thúc 1 transactions
            return await _context.SaveChangesAsync();
        }

        // Giải phóng tài nguyên khi cần
        public void Dispose()
        {
            _context.Dispose();
        }

        // Gọi hoặc khởi tạo 1 repository
        public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity
        {
            if (_repositories == null) _repositories = new Hashtable(); // check hashtable nếu null thì tạo mới

            var type = typeof(TEntity).Name; // lấy ra type của entity, cái này được dùng để làm Key

            // nếu trong hashtable chưa có key này sẽ tạo mới 1 repo và lưu vào hastable theo key
            if (!_repositories.ContainsKey(type))
            {
                var repositoryType = typeof(GenericRepository<>);
                var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(TEntity)), _context);

                _repositories.Add(type, repositoryInstance);
            }
            // trả về repo kem kiểu của no
            return (IGenericRepository<TEntity>)_repositories[type];
        }
    }
}