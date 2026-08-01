using FlowCore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowCore.Application.Specifications
{
    public class ProductsInStockSpec : BaseSpecification<Product>
    {
        public ProductsInStockSpec() : base(p=> p.Stock > 0)
        {
            AddInclude(p => p.Category);
            ApplyOrderBy(p=> p.Name);
        }
    }
}
