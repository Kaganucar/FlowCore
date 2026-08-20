using FlowCore.Application.Common;
using FlowCore.Application.DTOs.Order;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowCore.Application.Features.Orders.Commands.CancelOrder
{
    public class CancelOrderCommand : IRequest<Result<OrderResponse>>
    {
        public Guid Id { get; set; }
        public Guid RequestingUserId { get; set; }
    }
}
