using FlowCore.Application.DTOs.Category;
using FlowCore.Application.DTOs.Product;
using FlowCore.Domain.Exceptions;
using FlowCore.Infrastructure.Persistence;
using FlowCore.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowCore.Tests
{
    public class ProductServiceTests
    {
        private AppDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }

        [Fact]
        public async Task CreateAsync_ShouldCreateProduct()
        {
            var context = CreateInMemoryContext();
            var unitOfWork = new FlowCore.Infrastructure.UnitOfWork.UnitOfWork(context);
            var categoryService = new CategoryService(unitOfWork);
            var productService = new ProductService(unitOfWork);

            var category = await categoryService.CreateAsync(new CreateCategoryRequest { Name = "Elektronik" });

            var request = new CreateProductRequest
            {
                Name = "Laptop",
                Description = "Test Ürün",
                Price = 1000,
                Stock = 5,
                CategoryId = category.Value.Id
            };

            var result = await productService.CreateAsync(request);
            Assert.NotNull(result);
            Assert.Equal("Laptop", result.Value.Name);
            Assert.NotEqual(Guid.Empty, result.Value.Id);

        }

        [Fact]
        public async Task GetByIdAsync_ShouldThrowException()
        {
            var context = CreateInMemoryContext();
            var unitOfWork = new FlowCore.Infrastructure.UnitOfWork.UnitOfWork(context);
            var productService = new ProductService(unitOfWork);

            var result = await productService.GetByIdAsync(Guid.NewGuid());

            Assert.False(result.IsSuccess);
            Assert.Equal(404, result.StatusCode);
        }
    }
}
