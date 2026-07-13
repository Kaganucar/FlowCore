using FlowCore.Application.Interfaces;
using FlowCore.Infrastructure.Persistence;
using FlowCore.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowCore.Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        private IProductRepository? _products;
        private ICategoryRepository? _categories;
        private IOrderRepository? _orders;
        private IUserRepository? _users;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }
        
        public void Dispose()
        {
            _context.Dispose();
        }

        public IProductRepository Products => _products ??= new ProductRepository(_context);
        public ICategoryRepository Categories => _categories ??= new CategoryRepository(_context);
        public IOrderRepository Orders => _orders ??= new OrderRepository(_context);
        public IUserRepository Users => _users ??= new UserRepository(_context);
    }
}
