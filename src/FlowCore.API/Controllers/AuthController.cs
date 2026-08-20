using FlowCore.Application.Common;
using FlowCore.Application.DTOs.Auth;
using FlowCore.Application.Features.Auth.Commands.Login;
using FlowCore.Application.Features.Auth.Commands.RefreshToken;
using FlowCore.Application.Features.Auth.Commands.Register;
using FlowCore.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FlowCore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediatR;

        public AuthController(IMediator mediator)
        {
            _mediatR = mediator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var command = new RegisterCommand
            {
                Username = request.Username,
                Email = request.Email,
                Password = request.Password
            };

            var result = await _mediatR.Send(command);
            if(!result.IsSuccess)
                return StatusCode(result.StatusCode, new {error = result.Error});

            return Ok(result.Value);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var command = new LoginCommand
            {
                Email = request.Email,
                Password = request.Password
            };

            var result = await _mediatR.Send(command);
            if(!result.IsSuccess)
                return StatusCode(result.StatusCode, new { error = result.Error });

            return Ok(result.Value);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
        {
            var command = new RefreshTokenCommand
            {
                RefreshToken = request.RefreshToken,
            };

            var result = await _mediatR.Send(command);
            if (!result.IsSuccess)
                return StatusCode(result.StatusCode, new { error = result.Error });

            return Ok(result.Value);
        }
    }
}

