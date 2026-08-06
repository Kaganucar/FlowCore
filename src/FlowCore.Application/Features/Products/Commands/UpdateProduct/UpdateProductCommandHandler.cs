using FlowCore.Application.Common;
using FlowCore.Application.DTOs.Product;
using FlowCore.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowCore.Application.Features.Products.Commands.UpdateProduct
{
    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Result<ProductResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateProductCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<ProductResponse>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _unitOfWork.Products.GetByIdWithCategoryAsync(request.Id);

            if (product == null)
            {
                return Result<ProductResponse>.Failure($"Product {request.Id} not found", 404);
            }

            if(!string.IsNullOrEmpty(request.Name)) product.Name = request.Name;
            if(!string.IsNullOrEmpty(request.Description)) product.Description = request.Description;
            if(request.Price.HasValue) product.Price = request.Price.Value;
            if(request.Stock.HasValue) product.Stock = request.Stock.Value;

            product.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Products.Update(product);
            await _unitOfWork.SaveChangesAsync();

            var response = new ProductResponse
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                CategoryName = product.Category?.Name ?? string.Empty,
                CreatedAt = product.CreatedAt,
            };

            return Result<ProductResponse>.Success(response);
        }
    }
}
