using FlowCore.Application.DTOs.Category;
using FlowCore.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowCore.Application.Features.Categories.Queries.GetAllCategories
{
    public class GetAllCategoriesQueryHandler : IRequestHandler<GetAllCategoriesQuery, List<CategoryResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllCategoriesQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<CategoryResponse>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
        {
            var categories = await _unitOfWork.Categories.GetAllWithProductsAsync();

            return categories.Select(c => new CategoryResponse
            {
                Id = c.Id,
                Name = c.Name,
                ProductCount = c.Products.Count
            }).ToList();
        }
    }
}
