using FlowCore.Application.Common;
using FlowCore.Application.DTOs.Category;
using FlowCore.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowCore.Application.Features.Categories.Queries.GetCategoryById
{
    public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, Result<CategoryResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetCategoryByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<CategoryResponse>> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
        {
            var categories = await _unitOfWork.Categories.GetAllWithProductsAsync();
            var category = categories.FirstOrDefault(c => c.Id == request.Id);

            if (category == null)
            {
                return Result<CategoryResponse>.Failure($"Category {request.Id} not found", 404);
            }

            var response = new CategoryResponse
            {
                Id = request.Id,
                Name = category.Name,
                ProductCount = category.Products.Count
            };

            return Result<CategoryResponse>.Success(response);
        }
    }
}
