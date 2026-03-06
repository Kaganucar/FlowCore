using FlowCore.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowCore.Core.Entities
{
    public class Product : BaseEntity
    {
        public string Name { get; set; } = default!;

        public string Sku {  get; set; } = default!;

        public decimal Price { get; set; }

        public int StockQuantity { get; set; }
    }
}
