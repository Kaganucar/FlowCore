using FlowCore.Application.DTOs.Category;
using FlowCore.Application.DTOs.Product;
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
    public class ProductService : IProductService
    {
        private readonly AppDbContext _context;

        public ProductService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProductResponse>> GetProductsAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .Select(p => new ProductResponse
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    Stock = p.Stock,
                    CategoryName = p.Category.Name,
                    CreatedAt = p.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<ProductResponse> GetByIdAsync(Guid id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id)
                ?? throw new AppException($"Product {id} not found", 404);

            return new ProductResponse
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                CategoryName = product.Category.Name,
                CreatedAt = product.CreatedAt
            };
        }

        public async Task<ProductResponse> CreateAsync(CreateProductRequest request)
        {
            var categoryExists = await _context.Categories.AnyAsync(c => c.Id == request.CategoryId);
            if (!categoryExists)
                throw new AppException($"Caategory {request.CategoryId} not found", 404);

            var product = new Product
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                Stock = request.Stock,
                CategoryId = request.CategoryId

            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            await _context.Entry(product).Reference(p=> p.Category).LoadAsync();

            return new ProductResponse
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                CategoryName = product.Category.Name,
                CreatedAt = product.CreatedAt
            };
        }

        public async Task<ProductResponse> UpdateAsync(Guid id, UpdateProductRequest request)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id)
                ?? throw new AppException($"Product {id} not found", 404);

            if(!string.IsNullOrWhiteSpace(request.Name)) product.Name = request.Name;
            if(!string.IsNullOrWhiteSpace(request.Description)) product.Description = request.Description;
            if (request.Price.HasValue) product.Price = request.Price.Value;
            if (request.Stock.HasValue) product.Price = request.Stock.Value;

            await _context.SaveChangesAsync();

            return new ProductResponse
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                CategoryName = product.Category.Name,
                CreatedAt = product.CreatedAt
            };
        }

        public async Task DeleteAsync(Guid id)
        {
            var product = await _context.Products.FindAsync(id)
                ?? throw new AppException($"Product {id} not found", 404);

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }
    }
}
