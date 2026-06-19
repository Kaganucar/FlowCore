using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowCore.Application.DTOs.Order
{
    public class CreateOrderRequest
    {
        public ICollection<OrderItemRequest> Items { get; set; } = new List<OrderItemRequest>();
    }
}
