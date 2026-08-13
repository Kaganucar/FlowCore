using FlowCore.Application.Common;
using FlowCore.Application.DTOs.Order;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowCore.Application.Features.Orders.Queries.GetOrderById
{
    public class GetOrderByIdQuery : IRequest<Result<OrderResponse>>
    {
        public Guid Id { get; set; }
    }
}
