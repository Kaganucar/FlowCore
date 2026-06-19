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
        Task<OrderResponse> GetByIdAsync(Guid id);
        Task<OrderResponse> CreateAsync(CreateOrderRequest request, Guid userId);
        Task<OrderResponse> CancelAsync(Guid id);
    }
}
