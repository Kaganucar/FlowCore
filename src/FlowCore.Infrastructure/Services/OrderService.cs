using FlowCore.Application.DTOs.Order;
using FlowCore.Application.Interfaces;
using FlowCore.Domain.Entities;
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

        public Task<OrderResponse> CancelAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<OrderResponse> CreateAsync(CreateOrderRequest request, Guid userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId)
                ?? throw new AppException("User not found", 404);

            var productIds = request.Items.Select(i => i.ProductId).ToList();

            var products = await _unitOfWork.Products.FindAsync(p => productIds.Contains(p.Id));

            var productDict = products.ToDictionary(p => p.Id);

            var orderItems = new List<OrderItem>();

            foreach (var item in request.Items)
            {
                if (!productDict.TryGetValue(item.ProductId, out var product))
                    throw new AppException("Product not found", 404);

                if(product.Stock < item.Quantity)
                    throw new AppException($"Insufficient stock for {product.Name}", 400);

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

            return new OrderResponse
            {
                Id = order.Id,
                UserId = order.UserId,
                UserName = user.UserName,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                Items = order.OrderItems.Select(i => new OrderItemResponse
                {
                    ProductId = i.ProductId,
                    ProductName = productDict[i.ProductId].Name,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                }).ToList()
            };
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
                Items = order.OrderItems.Select(i => new OrderItemResponse
                {
                    ProductId = i.ProductId,
                    ProductName = i.Product?.Name ?? string.Empty,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                }).ToList()

            }).ToList();
        }

        public async Task<OrderResponse> GetByIdAsync(Guid id)
        {
            var order = await _unitOfWork.Orders.GetByIdWithDetailsAsync(id)
                ?? throw new AppException($"Order {id} not found", 404);

            return new OrderResponse
            {
                Id = order.Id,
                UserId = order.UserId,
                UserName = order.User?.UserName ?? string.Empty,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                Items = order.OrderItems.Select(i => new OrderItemResponse
                {
                    ProductId = i.ProductId,
                    ProductName = i.Product?.Name ?? string.Empty,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                }).ToList()
            };
        }
    }

}
