using FlowCore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowCore.Application.Specifications
{
    public class ProductsWithCategorySpec : BaseSpecification<Product>
    {
        public ProductsWithCategorySpec()
        {
            AddInclude(p=> p.Category);
        }
    }
}
