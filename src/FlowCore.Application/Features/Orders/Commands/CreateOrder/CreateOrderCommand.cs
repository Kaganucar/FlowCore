using FlowCore.Application.Common;
using FlowCore.Application.DTOs.Order;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowCore.Application.Features.Orders.Commands.CreateOrder
{
    public class CreateOrderCommand : IRequest<Result<OrderResponse>>
    {
        public Guid UserId { get; set; }
        public List<OrderItemRequest> Items { get; set; } = new();
    }
}
