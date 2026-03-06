using FlowCore.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowCore.Infrastructure.Persistence
{
    public class FlowCoreDbContext : DbContext
    {
        public FlowCoreDbContext(DbContextOptions<FlowCoreDbContext> options) : base(options) 
        {
        }
        
        public DbSet<Product> Products => Set<Product>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>().HasQueryFilter(x => !x.IsDeleted);

            modelBuilder.Entity<Product>(b =>
            {
                b.Property(x => x.Name).HasMaxLength(200).IsRequired();
                b.Property(x => x.Sku).HasMaxLength(50).IsRequired();
                b.HasIndex(x => x.Sku).IsUnique();
                b.Property(x => x.Price).HasPrecision(18, 2);
            });
        }
    }
}
