using FlowCore.Application.Common;
using FlowCore.Application.DTOs.Category;
using FlowCore.Application.Interfaces;
using FlowCore.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowCore.Application.Features.Categories.Commands.CreateCategory
{
    public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Result<CategoryResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateCategoryCommandHandler(IUnitOfWork unitOfWork) 
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<CategoryResponse>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            var categoryExists =  await _unitOfWork.Categories.AnyAsync(c=> c.Name == request.Name);

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
    }
}
