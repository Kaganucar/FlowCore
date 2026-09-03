using FlowCore.Application.Features.Products.Commands.CreateProduct;
using FlowCore.Application.Interfaces;
using FlowCore.Domain.Entities;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FlowCore.Tests
{
    public class CreateProductCommandHandlerTests
    {
        [Fact]
        public async Task Handle_ShouldCreateProduct_WhenCategoryExists()
        {
            // Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockProductRepo = new Mock<IProductRepository>();
            var mockCategoryRepo = new Mock<ICategoryRepository>();

            mockUnitOfWork.Setup(u => u.Products).Returns(mockProductRepo.Object);
            mockUnitOfWork.Setup(u => u.Categories).Returns(mockCategoryRepo.Object);

            mockCategoryRepo.Setup(r => r.AnyAsync(It.IsAny<Expression<Func<Category, bool>>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var fakeCategory = new Category { Id = Guid.NewGuid(), Name = "Elektronik" };
            var fakeProduct = new Product
            {
                Id = Guid.NewGuid(),
                Name = "Laptop",
                Description = "Test",
                Price = 1000,
                Stock = 5,
                CategoryId = fakeCategory.Id,
                Category = fakeCategory,
            };

            mockProductRepo
                .Setup(r => r.GetByIdWithCategoryAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>
                ()))
                .ReturnsAsync(fakeProduct);

            //Act
            var handler = new CreateProductCommandHandler(mockUnitOfWork.Object);
            var command = new CreateProductCommand
            {
                Name = "Laptop",
                Description = "Test",
                Price = 1000,
                Stock = 5,
                CategoryId = fakeCategory.Id
            };

            var result = await handler.Handle(command, CancellationToken.None);

            //Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("Laptop", result.Value.Name);
            Assert.Equal("Elektronik", result.Value.CategoryName);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenCategoryDoesNotExist()
        {
            //Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockCategoryRepo = new Mock<ICategoryRepository>();

            mockUnitOfWork.Setup(u=> u.Categories).Returns(mockCategoryRepo.Object);

            mockCategoryRepo
                .Setup(r => r.AnyAsync(It.IsAny<Expression<Func<Category, bool>>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var handler = new CreateProductCommandHandler(mockUnitOfWork.Object);
            var command = new CreateProductCommand
            {
                Name = "Laptop",
                CategoryId = Guid.NewGuid()
            };

            //Act
            var result = await handler.Handle(command, CancellationToken.None);

            //Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(404, result.StatusCode);
        }
    }
}
