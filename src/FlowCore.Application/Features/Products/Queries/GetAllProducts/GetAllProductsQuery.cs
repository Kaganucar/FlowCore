using FlowCore.Application.DTOs.Product;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowCore.Application.Features.Products.Queries.GetAllProducts
{
    public class GetAllProductsQuery : IRequest<List<ProductResponse>>
    {
    }
}
