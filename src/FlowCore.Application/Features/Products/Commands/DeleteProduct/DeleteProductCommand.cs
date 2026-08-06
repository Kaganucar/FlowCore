using FlowCore.Application.Common;
using FlowCore.Application.DTOs.Product;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowCore.Application.Features.Products.Commands.DeleteProduct
{
    public class DeleteProductCommand : IRequest<Result<bool>>
    {
        public Guid Id { get; set; }
    }
}
