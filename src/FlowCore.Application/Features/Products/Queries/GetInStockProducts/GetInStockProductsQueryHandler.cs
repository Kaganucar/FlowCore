using FlowCore.Application.DTOs.Product;
using FlowCore.Application.Interfaces;
using FlowCore.Application.Specifications;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowCore.Application.Features.Products.Queries.GetInStockProducts
{
    public class GetInStockProductsQueryHandler : IRequestHandler<GetInStockProductsQuery, List<ProductResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetInStockProductsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<ProductResponse>> Handle(GetInStockProductsQuery request, CancellationToken cancellationToken)
        {
            var products = await _unitOfWork.Products.ListAsync(new ProductsInStockSpec());

            return products.Select(p=> new ProductResponse
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Stock = p.Stock,
                CategoryName = p.Category?.Name?? string.Empty,
                CreatedAt = p.CreatedAt,
            }).ToList();
        }
    }
}
