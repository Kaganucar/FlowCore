using FlowCore.Application.Common;
using FlowCore.Application.DTOs.Auth;
using FlowCore.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowCore.Application.Features.Auth.Commands.RefreshToken
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<AuthResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _configuration;

        public RefreshTokenCommandHandler(IUnitOfWork unitOfWork, ITokenService tokenService, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
            _configuration = configuration;
        }

        public async Task<Result<AuthResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Users.GetByRefreshTokenAsync(request.RefreshToken);

            if (user == null)
                return Result<AuthResponse>.Failure("Invalid refresh token", 401);

            if (user.RefreshTokenExpiry < DateTime.UtcNow)
                return Result<AuthResponse>.Failure("Refresh token expired", 401);

            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(_configuration.GetValue<int>("Jwt:RefreshTokenExpiryDays", 7));

            await _unitOfWork.SaveChangesAsync();

            return Result<AuthResponse>.Success(new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                UserName = user.UserName,
                Role = user.Role.ToString(),
            });
        }
    }
}
