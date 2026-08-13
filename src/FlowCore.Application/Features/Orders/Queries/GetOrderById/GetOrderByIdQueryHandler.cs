using FlowCore.Application.Common;
using FlowCore.Application.DTOs.Order;
using FlowCore.Application.DTOs.Product;
using FlowCore.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowCore.Application.Features.Orders.Queries.GetOrderById
{
    public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery ,Result<OrderResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetOrderByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<OrderResponse>> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken) 
        {
            var orders = await _unitOfWork.Orders.GetByIdWithDetailsAsync(request.Id);

            if (orders == null)
            {
                return Result<OrderResponse>.Failure($"Order {request.Id} not found", 404);
            }

            var response = new OrderResponse
            {
                Id = orders.Id,
                UserId = orders.UserId,
                UserName = orders.User?.UserName ?? string.Empty,
                OrderDate = orders.OrderDate,
                TotalAmount = orders.TotalAmount,
                Status = orders.Status.ToString(),
                Items = orders.OrderItems.Select(i => new OrderItemResponse
                {
                    ProductId = i.ProductId,
                    ProductName = i.Product?.Name ?? string.Empty,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                }).ToList()
            };

            return Result<OrderResponse>.Success(response);
        }
    }
}
