using FlowCore.Application.Interfaces;
using FlowCore.Domain.Entities;
using FlowCore.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowCore.Infrastructure.Repositories
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(AppDbContext context) : base(context) { }
        public async Task<List<Category>> GetAllWithProductsAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Categories
                .Include(p => p.Products)
                .ToListAsync(cancellationToken);
        }
        public async Task<Category?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            return await _context.Categories
                .Include(p => p.Products)
                .FirstOrDefaultAsync(c => EF.Functions.ILike(c.Name, name),cancellationToken);
        }
    }
}
