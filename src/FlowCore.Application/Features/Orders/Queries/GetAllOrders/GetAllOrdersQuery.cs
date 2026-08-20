using FlowCore.Application.DTOs.Order;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowCore.Application.Features.Orders.Queries.GetAllOrders
{
    public class GetAllOrdersQuery : IRequest<List<OrderResponse>>
    {
    }
}
