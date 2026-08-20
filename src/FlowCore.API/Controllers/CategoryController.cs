using FlowCore.Application.DTOs.Category;
using FlowCore.Application.Features.Categories.Commands.CreateCategory;
using FlowCore.Application.Features.Categories.Commands.DeleteCategory;
using FlowCore.Application.Features.Categories.Queries.GetAllCategories;
using FlowCore.Application.Features.Categories.Queries.GetCategoryById;
using FlowCore.Application.Interfaces;
using FlowCore.Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlowCore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CategoryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _mediator.Send(new GetAllCategoriesQuery());
            return Ok(categories);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetCategoryByIdQuery {Id = id});
            if (!result.IsSuccess) 
            {
                return StatusCode(result.StatusCode, new { error = result.Error });
            }

            return Ok(result.Value);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateCategoryRequest request)
        {
            var command = new CreateCategoryCommand
            {
                Name = request.Name,
            };

            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, new { error = result.Error });
            }

            return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var command = new DeleteCategoryCommand { Id = id };

            var result = await _mediator.Send(command);

            if(!result.IsSuccess)
                return StatusCode(result.StatusCode, new { error = result.Error });

            return NoContent();
        }

    }
}
