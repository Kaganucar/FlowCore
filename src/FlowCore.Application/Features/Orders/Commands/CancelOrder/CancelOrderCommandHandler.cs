using FlowCore.Application.Common;
using FlowCore.Application.DTOs.Order;
using FlowCore.Application.Interfaces;
using FlowCore.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowCore.Application.Features.Orders.Commands.CancelOrder
{
    public class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, Result<OrderResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CancelOrderCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<OrderResponse>> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _unitOfWork.Orders.GetByIdWithDetailsAsync(request.Id);
            if (order == null)
                return Result<OrderResponse>.Failure($"Order {request.Id} not found", 404);

            var requestingUser = await _unitOfWork.Users.GetByIdAsync(request.RequestingUserId);
            if (requestingUser == null)
                return Result<OrderResponse>.Failure("User not found", 404);

            var isOwner = order.UserId == request.RequestingUserId;
            var isAdmin = requestingUser.Role == UserRole.Admin;

            if (!isOwner && !isAdmin)
                return Result<OrderResponse>.Failure("You are not allowed to cancel this order", 403);

            if (order.Status == OrderStatus.Cancelled)
                return Result<OrderResponse>.Failure("Order is already cancelled", 400);

            if (order.Status != OrderStatus.Pending)
                return Result<OrderResponse>.Failure("Only pending orders can be cancelled", 400);

            foreach(var item in order.OrderItems)
            {
                item.Product.Stock += item.Quantity;
                _unitOfWork.Products.Update(item.Product);
            }

            order.Status = OrderStatus.Cancelled;
            await _unitOfWork.SaveChangesAsync();

            var response = new OrderResponse
            {
                Id = order.Id,
                UserId = order.UserId,
                UserName = order.User?.UserName ?? string.Empty,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                Status = order.Status.ToString(),
                Items = order.OrderItems.Select(i => new OrderItemResponse
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
