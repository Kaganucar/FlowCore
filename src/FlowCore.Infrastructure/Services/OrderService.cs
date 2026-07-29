using FlowCore.Application.Common;
using FlowCore.Application.DTOs.Order;
using FlowCore.Application.Interfaces;
using FlowCore.Domain.Entities;
using FlowCore.Domain.Enums;
using FlowCore.Domain.Exceptions;
using FlowCore.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowCore.Infrastructure.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<OrderResponse>> GetAllAsync()
        {
            var orders = await _unitOfWork.Orders.GetAllWithDetailsAsync();

            return orders.Select(order => new OrderResponse
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
            }).ToList();
        }

        public async Task<Result<OrderResponse>> GetByIdAsync(Guid id)
        {
            var order = await _unitOfWork.Orders.GetByIdWithDetailsAsync(id);
            if (order == null)
                return Result<OrderResponse>.Failure($"Order {id} not found", 404);


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

        public async Task<Result<OrderResponse>> CreateAsync(CreateOrderRequest request, Guid userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
                return Result<OrderResponse>.Failure("User not found", 404);

            var productIds = request.Items.Select(i => i.ProductId).ToList();

            var products = await _unitOfWork.Products.FindAsync(p => productIds.Contains(p.Id));

            var productDict = products.ToDictionary(p => p.Id);

            var orderItems = new List<OrderItem>();

            foreach (var item in request.Items)
            {
                if (!productDict.TryGetValue(item.ProductId, out var product))
                    return Result<OrderResponse>.Failure("Product not found", 404);

                if (product.Stock < item.Quantity)
                    return Result<OrderResponse>.Failure($"Insufficient stock for {product.Name}", 400);

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
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                OrderItems = orderItems
            };

            order.TotalAmount = order.OrderItems.Sum(i => i.Quantity * i.UnitPrice);

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
        public async Task<Result<OrderResponse>> CancelAsync(Guid id, Guid requestingUserId)
        {
            var order = await _unitOfWork.Orders.GetByIdWithDetailsAsync(id);
            if (order == null)
                return Result<OrderResponse>.Failure($"Order {id} not found", 404);

            var requestingUser = await _unitOfWork.Users.GetByIdAsync(requestingUserId);
            if (requestingUser == null)
                return Result<OrderResponse>.Failure("User not found", 404);

            var isOwner = order.UserId == requestingUserId;
            var isAdmin = requestingUser.Role == UserRole.Admin;

            if (!isOwner && !isAdmin)
                return Result<OrderResponse>.Failure("You are not allowed to cancel this order", 403);

            if (order.Status == OrderStatus.Cancelled)
                return Result<OrderResponse>.Failure("Order is already cancelled", 400);

            if (order.Status != OrderStatus.Pending)
                return Result<OrderResponse>.Failure("Only pending orders can be cancelled", 400);

            foreach (var item in order.OrderItems)
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
