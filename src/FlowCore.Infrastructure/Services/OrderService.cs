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
        private readonly AppDbContext _context;

        public OrderService(AppDbContext context)
        {
            _context = context;
        }

        public Task<OrderResponse> CancelAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<OrderResponse> CreateAsync(CreateOrderRequest request, Guid userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId)
                ?? throw new AppException("User not found", 404);

            var productIds = request.Items.Select(i => i.ProductId).ToList();

            var products = await _context.Products
                .Where(p=> productIds.Contains(p.Id))
                .ToListAsync();

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
            }

            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                OrderItems = orderItems
            };

            order.TotalAmount = order.OrderItems.Sum(i => i.Quantity * i.UnitPrice);

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

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

        public Task<List<OrderResponse>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<OrderResponse> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }
    }

}
