using FlowCore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowCore.Application.Interfaces
{
    public interface IOrderRepository : IRepository<Order>
    {
        Task<List<Order>> GetAllUserOrdersAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<Order?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
