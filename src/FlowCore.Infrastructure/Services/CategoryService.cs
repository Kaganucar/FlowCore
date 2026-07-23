using FlowCore.Application.Common;
using FlowCore.Application.DTOs.Category;
using FlowCore.Application.Interfaces;
using FlowCore.Domain.Entities;
using FlowCore.Domain.Exceptions;
using FlowCore.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowCore.Infrastructure.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<CategoryResponse>> GetAllAsync()
        {
            var categories = await _unitOfWork.Categories.GetAllWithProductsAsync();

            return categories.Select(c => new CategoryResponse
            {
                Id = c.Id,
                Name = c.Name,
                ProductCount = c.Products.Count
            }).ToList();
        }

        public async Task<Result<CategoryResponse>> GetByIdAsync(Guid id)
        {
            var categories = await _unitOfWork.Categories.GetAllWithProductsAsync();
            var category = categories.FirstOrDefault(c => c.Id == id);

            if (category == null)
                return Result<CategoryResponse>.Failure($"Category {id} not found", 404);


           var response = new CategoryResponse
            {
                Id = category.Id,
                Name = category.Name,
                ProductCount = category.Products.Count
            };

            return Result<CategoryResponse>.Success(response);
        }

        public async Task<Result<CategoryResponse>> CreateAsync(CreateCategoryRequest request)
        {
            var categoryExists = await _unitOfWork.Categories.AnyAsync(c => c.Name == request.Name);

            if (categoryExists)
                return Result<CategoryResponse>.Failure("Category already exists", 400);


            var category = new Category { Name = request.Name };

            await _unitOfWork.Categories.AddAsync(category);
            await _unitOfWork.SaveChangesAsync();

            var response = new CategoryResponse
            {
                Id = category.Id,
                Name = category.Name,
                ProductCount = 0
            };
            return Result<CategoryResponse>.Success(response);
        }

        public async Task<Result<bool>> DeleteAsync(Guid id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
                
            if (category == null)
                return Result<bool>.Failure($"Category {id} not found", 404);

            _unitOfWork.Categories.Remove(category);
            await _unitOfWork.SaveChangesAsync();

            return Result<bool>.Success(true);
        }
        
    }
}
