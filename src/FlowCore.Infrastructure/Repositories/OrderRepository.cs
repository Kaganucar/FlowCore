using FlowCore.Application.Interfaces;
using FlowCore.Domain.Entities;
using FlowCore.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowCore.Infrastructure.Repositories
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        public OrderRepository(AppDbContext context) : base(context) { }
        public async Task<List<Order>> GetAllUserOrdersAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _context.Orders
                 .Include(o => o.OrderItems)
                 .ThenInclude(oi => oi.Product)
                 .Where(o => o.UserId == userId)
                 .ToListAsync(cancellationToken);
        }
        public async Task<Order?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
        }

        public async Task<List<Order>> GetAllWithDetailsAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .ToListAsync(cancellationToken);
        }
    }
}
