using FlowCore.Application.Common;
using FlowCore.Application.DTOs.Auth;
using FlowCore.Application.Interfaces;
using FlowCore.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowCore.Application.Features.Auth.Commands.Register
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<AuthResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _configuration;

        public RegisterCommandHandler(IUnitOfWork unitOfWork, ITokenService tokenService, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
            _configuration = configuration;
        }

        public async Task<Result<AuthResponse>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
             var emailExist = await _unitOfWork.Users.AnyEmailAsync(request.Email);

             if (emailExist)
                return Result<AuthResponse>.Failure("Email already in use", 400);

            var user = new User
            {
                UserName = request.Username,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
            };

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(_configuration.GetValue<int>("Jwt:RefreshTokenExpiryDays", 7));
            await _unitOfWork.SaveChangesAsync();

            return Result<AuthResponse>.Success(new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                UserName = request.Username,
                Role = user.Role.ToString()
            });
        }
    }
}
