using FlowCore.Application.Common;
using FlowCore.Application.DTOs.Product;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowCore.Application.Features.Products.Commands.UpdateProduct
{
    public class UpdateProductCommand :IRequest<Result<ProductResponse>>
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public int? Stock {  get; set; }
    }
}
