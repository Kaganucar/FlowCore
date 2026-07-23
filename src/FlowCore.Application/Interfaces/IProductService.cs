using FlowCore.Application.Common;
using FlowCore.Application.DTOs.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowCore.Application.Interfaces
{
    public interface IProductService
    {
        Task<List<ProductResponse>> GetProductsAsync();
        Task<Result<ProductResponse>> GetByIdAsync(Guid id);
        Task<Result<ProductResponse>> CreateAsync(CreateProductRequest request);
        Task<Result<ProductResponse>> UpdateAsync(Guid id, UpdateProductRequest request);
        Task<Result<bool>> DeleteAsync(Guid id);
    }
}
