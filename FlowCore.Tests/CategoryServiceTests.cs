using FlowCore.Application.DTOs.Category;
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
    public class CategoryServiceTests
    {
        private AppDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            
            return new AppDbContext(options);
        }

        [Fact]
        public async Task CreateAsync_ShouldCreateCategory()
        {
            var context = CreateInMemoryContext();
            var unitOfWork = new FlowCore.Infrastructure.UnitOfWork.UnitOfWork(context);
            var service = new CategoryService(unitOfWork);
            var request = new CreateCategoryRequest { Name = "Elektronik" };

            var result = await service.CreateAsync(request);

            Assert.NotNull(result);
            Assert.Equal("Elektronik", result.Name);
            Assert.NotEqual(Guid.Empty, result.Id);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldThrowException_WhenNotFound()
        {
            var context = CreateInMemoryContext();
            var unitOfWork = new FlowCore.Infrastructure.UnitOfWork.UnitOfWork(context);
            var service = new CategoryService(unitOfWork);

            await Assert.ThrowsAsync<AppException>(() =>
            service.GetByIdAsync(Guid.Empty));
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllCategories()
        {
            var context = CreateInMemoryContext();
            var unitOfWork = new FlowCore.Infrastructure.UnitOfWork.UnitOfWork(context);
            var service = new CategoryService(unitOfWork);

            await service.CreateAsync(new CreateCategoryRequest { Name = "Elektronik" });
            await service.CreateAsync(new CreateCategoryRequest { Name = "Giyim" });

            var result = await service.GetAllAsync();

            Assert.Equal(2, result.Count);
        }
    }
}
