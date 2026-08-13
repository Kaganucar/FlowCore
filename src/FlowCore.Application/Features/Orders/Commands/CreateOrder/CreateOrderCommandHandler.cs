using FlowCore.Application.Common;
using FlowCore.Application.DTOs.Order;
using FlowCore.Application.Interfaces;
using FlowCore.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowCore.Application.Features.Orders.Commands.CreateOrder
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand ,Result<OrderResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateOrderCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<OrderResponse>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);
            if (user == null)
                return Result<OrderResponse>.Failure("User not found", 404);

            var productIds = request.Items.Select(p => p.ProductId).ToList();
            var products = await _unitOfWork.Products.FindAsync(p => productIds.Contains(p.Id));
            var productDict = products.ToDictionary(p => p.Id);

            var orderItems = new List<OrderItem>();

            foreach (var item in request.Items)
            {
                if (!productDict.TryGetValue(item.ProductId, out var product))
                    return Result<OrderResponse>.Failure("Product not found", 404);

                if(product.Stock < item.Quantity)
                {
                    return Result<OrderResponse>.Failure($"Insufficient stock for {product.Name}", 400);
                }

                orderItems.Add(new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = product.Price
                });

                product.Stock -= item.Quantity;
                _unitOfWork.Products.Update(product);
            }

            var order = new Order
            {
                UserId = user.Id,
                OrderDate = DateTime.UtcNow,
                OrderItems = orderItems,
            };

            order.TotalAmount = order.OrderItems.Sum(i=> i.Quantity * i.UnitPrice);

            await _unitOfWork.Orders.AddAsync(order);
            await _unitOfWork.SaveChangesAsync();

            var response = new OrderResponse
            {
                Id = order.Id,
                UserId = order.UserId,
                UserName = user.UserName,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                Status = order.Status.ToString(),
                Items = order.OrderItems.Select(i => new OrderItemResponse
                {
                    ProductId = i.ProductId,
                    ProductName = productDict[i.ProductId].Name,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                }).ToList()
            };

            return Result<OrderResponse>.Success(response);

        }
    }
}
