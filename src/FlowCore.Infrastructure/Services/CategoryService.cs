using FlowCore.Application.DTOs.Category;
using FlowCore.Application.Interfaces;
using FlowCore.Domain.Entities;
using FlowCore.Domain.Exceptions;
using FlowCore.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FlowCore.Infrastructure.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly AppDbContext _context;

        public CategoryService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<CategoryResponse>> GetAllAsync()
        {
            return await _context.Categories
                .Include(c => c.Products)
                .Select(c => new CategoryResponse
                {
                    Id = c.Id,
                    Name = c.Name,
                    ProductCount = c.Products.Count
                })
                .ToListAsync();
        }

        public async Task<CategoryResponse> GetByIdAsync(Guid id)
        {
            var category = await _context.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id)
                ?? throw new AppException($"Category {id} not found", 404);

            return new CategoryResponse
            {
                Id = category.Id,
                Name = category.Name,
                ProductCount = category.Products.Count
            };
        }

        public async Task<CategoryResponse> CreateAsync(CreateCategoryRequest request)
        {
            var category = new Category { Name = request.Name };
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return new CategoryResponse { Id = category.Id, Name = category.Name, ProductCount = 0 };
        }

        public async Task DeleteAsync(Guid id)
        {
            var category = await _context.Categories.FindAsync(id)
                ?? throw new AppException($"Category {id} not found", 404);

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }
    }
}
