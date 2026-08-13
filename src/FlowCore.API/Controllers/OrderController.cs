using FlowCore.Application.Common;
using FlowCore.Application.DTOs.Order;
using FlowCore.Application.Features.Orders.Commands.CreateOrder;
using FlowCore.Application.Features.Orders.Queries.GetOrderById;
using FlowCore.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FlowCore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IMediator _mediator;

        public OrderController(IOrderService orderService, IMediator mediator)
        {
            _orderService = orderService;   
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var order = await _orderService.GetAllAsync();
            return Ok(order);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetOrderByIdQuery { Id = id});
            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, new { error = result.Error });
            }
            return Ok(result.Value);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateOrderRequest request)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var command = new CreateOrderCommand
            {
                UserId = userId,
                Items = request.Items.ToList()
            };

            var result = await _mediator.Send(command);
            if (!result.IsSuccess)
                return StatusCode(result.StatusCode, new { error = result.Error });

            return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value);
        }

        [HttpPost("{id:guid}/cancel")]
        [Authorize]
        public async Task<IActionResult> Cancel(Guid id)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var result = await _orderService.CancelAsync(id, userId);
            if (!result.IsSuccess)
                return StatusCode(result.StatusCode, new { error = result.Error });

            return Ok(result.Value);
        }
    }
}
