using FlowCore.Application.DTOs.Product;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowCore.Application.Features.Products.Queries.GetInStockProducts
{
    public class GetInStockProductsQuery : IRequest<List<ProductResponse>>
    {
    }
}
