    using FlowCore.Application.Common;
using FlowCore.Application.DTOs.Product;
using FlowCore.Application.Features.Products.Commands.CreateProduct;
using FlowCore.Application.Features.Products.Commands.DeleteProduct;
using FlowCore.Application.Features.Products.Commands.UpdateProduct;
using FlowCore.Application.Features.Products.Queries.GetProductById;
using FlowCore.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlowCore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IMediator _mediator;

        public ProductController(IProductService productService, IMediator mediator)
        {
            _productService = productService;
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _productService.GetProductsAsync();
            return Ok(products);
        }

        [HttpGet("in-stock")]
        public async Task<IActionResult> GetInStock()
        {
            var result = await _productService.GetInStockProductsAsync();
            return Ok(result);
        }   

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetProductByIdQuery { Id = id });

            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, new { error = result.Error });
            }

            return Ok(result.Value);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateProductRequest request)
        {
            var command = new CreateProductCommand
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                Stock = request.Stock,
                CategoryId = request.CategoryId,
            };

            var result = await _mediator.Send(command);

           if(!result.IsSuccess)
                return StatusCode(result.StatusCode, new {error = result.Error});

           return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductRequest request)
        {
            var command = new UpdateProductCommand
            {
                Id = id,
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                Stock = request.Stock,
            };

            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
                return StatusCode(result.StatusCode, new { error = result.Error });

            return Ok(result.Value);
            
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var command = new DeleteProductCommand
            {
                Id = id
            };

            var result = await _mediator.Send(command);
            
            if (!result.IsSuccess)
                return StatusCode(result.StatusCode, new { error = result.Error });

            return NoContent();
        }
    }
}
