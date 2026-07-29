using FlowCore.Application.Common;
using FlowCore.Application.DTOs.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowCore.Application.Interfaces
{
    public interface IOrderService
    {
        Task<List<OrderResponse>> GetAllAsync();
        Task<Result<OrderResponse>> GetByIdAsync(Guid id);
        Task<Result<OrderResponse>> CreateAsync(CreateOrderRequest request, Guid userId);
        Task<Result<OrderResponse>> CancelAsync(Guid id, Guid requestingUserId);
    }
}
