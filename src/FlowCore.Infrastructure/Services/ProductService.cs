using FlowCore.Application.DTOs.Category;
using FlowCore.Application.DTOs.Product;
using FlowCore.Application.Interfaces;
using FlowCore.Domain.Entities;
using FlowCore.Domain.Exceptions;
using FlowCore.Infrastructure.Persistence;
using FlowCore.Infrastructure.UnitOfWork;
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
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<ProductResponse>> GetProductsAsync()
        {
            var products = await _unitOfWork.Products.GetAllAsync();

            return products.Select(p => new ProductResponse
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Stock = p.Stock,
                CategoryName = p.Category?.Name ?? string.Empty,
                CreatedAt = p.CreatedAt
            }).ToList();
        }

        public async Task<ProductResponse> GetByIdAsync(Guid id)
        {
            var product = await _unitOfWork.Products.GetByIdWithCategoryAsync(id);

            if (product == null)
                throw new AppException($"Product {id} not found", 404);

            return new ProductResponse
            {
                Id = id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                CategoryName = product.Category?.Name ?? string.Empty,
                CreatedAt = product.CreatedAt
            };
        }

        public async Task<ProductResponse> CreateAsync(CreateProductRequest request)
        {
            var categoryExists = await _unitOfWork.Categories.AnyAsync(c => c.Id == request.CategoryId);
            if (!categoryExists)
                throw new AppException($"Category {request.CategoryId} not found", 404);

            var product = new Product
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                Stock = request.Stock,
                CategoryId = request.CategoryId

            };

            await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.SaveChangesAsync();

            var createdProduct = await _unitOfWork.Products.GetByIdWithCategoryAsync(product.Id);

            return new ProductResponse
            {
                Id = createdProduct.Id,
                Name = createdProduct.Name,
                Description = createdProduct.Description,
                Price = createdProduct.Price,
                Stock = createdProduct.Stock,
                CategoryName = createdProduct.Category?.Name ?? string.Empty,
                CreatedAt = createdProduct.CreatedAt
            };
        }

        public async Task<ProductResponse> UpdateAsync(Guid id, UpdateProductRequest request)
        {
            var product = await _unitOfWork.Products.GetByIdWithCategoryAsync(id)
                ?? throw new AppException($"Product {id} not found", 404);

            if(!string.IsNullOrWhiteSpace(request.Name)) product.Name = request.Name;
            if(!string.IsNullOrWhiteSpace(request.Description)) product.Description = request.Description;
            if(request.Price.HasValue) product.Price = request.Price.Value;
            if(request.Stock.HasValue) product.Stock = request.Stock.Value;

            product.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Products.Update(product);
            await _unitOfWork.SaveChangesAsync();

            return new ProductResponse
            {
                Id = id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
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
