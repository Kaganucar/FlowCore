using FlowCore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowCore.Application.Interfaces
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<List<Category>> GetAllWithProductsAsync(CancellationToken cancellationToken = default);
        Task<Category?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    }
}