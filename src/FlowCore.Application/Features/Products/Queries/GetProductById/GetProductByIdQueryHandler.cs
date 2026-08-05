using FlowCore.Application.Common;
using FlowCore.Application.DTOs.Product;
using FlowCore.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowCore.Application.Features.Products.Queries.GetProductById
{
    public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, Result<ProductResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetProductByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<ProductResponse>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var product = await _unitOfWork.Products.GetByIdWithCategoryAsync(request.Id);

            if (product == null)
            {
                return Result<ProductResponse>.Failure($"Product {request.Id} not found", 404);
            }

            var response = new ProductResponse()
            {
                Id = request.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                CategoryName = product.Category?.Name?? string.Empty,
                CreatedAt = product.CreatedAt,
            };

            return Result<ProductResponse>.Success(response);
        }
    }
}
