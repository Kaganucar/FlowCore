using FlowCore.Application.DTOs.Order;
using FlowCore.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowCore.Application.Features.Orders.Queries.GetAllOrders
{
    public class GetAllOrdersQueryHandler : IRequestHandler<GetAllOrdersQuery, List<OrderResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllOrdersQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<OrderResponse>> Handle(GetAllOrdersQuery request, CancellationToken cancellationToken)
        {
            var orders = await _unitOfWork.Orders.GetAllWithDetailsAsync();
            
            return orders.Select(o => new OrderResponse
            {
                Id = o.Id,
                UserId = o.UserId,
                UserName = o.User?.UserName?? string.Empty,
                OrderDate = o.OrderDate,
                TotalAmount = o.TotalAmount,
                Status = o.Status.ToString(),
                Items = o.OrderItems.Select(i => new OrderItemResponse
                {
                    ProductId = i.ProductId,
                    ProductName = i.Product?.Name ?? string.Empty,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                }).ToList()
            }).ToList();
        }
    }
}
