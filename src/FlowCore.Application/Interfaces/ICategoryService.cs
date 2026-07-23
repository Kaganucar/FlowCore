using FlowCore.Application.Common;
using FlowCore.Application.DTOs.Category;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowCore.Application.Interfaces
{
    public interface ICategoryService
    {
        Task<List<CategoryResponse>> GetAllAsync();
        Task<Result<CategoryResponse>> GetByIdAsync(Guid id);
        Task<Result<CategoryResponse>> CreateAsync(CreateCategoryRequest request);
        Task<Result<bool>> DeleteAsync(Guid id);
    }
}
