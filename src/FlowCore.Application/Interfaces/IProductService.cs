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
        Task<ProductResponse> GetByIdAsync(Guid id);
        Task<ProductResponse> CreateAsync(CreateProductRequest request);
        Task<ProductResponse> UpdateAsync(Guid id, UpdateProductRequest request);
        Task DeleteAsync(Guid id);
    }
}
