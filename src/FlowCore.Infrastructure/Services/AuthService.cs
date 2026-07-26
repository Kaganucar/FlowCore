using FlowCore.Application.DTOs.Auth;
using FlowCore.Application.Interfaces;
using FlowCore.Domain.Entities;
using FlowCore.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using FlowCore.Domain.Exceptions;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlowCore.Application.Common;
using Microsoft.AspNetCore.Http.HttpResults;

namespace FlowCore.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _config;

        public AuthService(IUnitOfWork unitOfWork, IConfiguration config)
        {
            _unitOfWork = unitOfWork;
            _config = config;
        }

        public async Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request)
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


            var response = await GenerateAuthResponse(user);

            return Result<AuthResponse>.Success(response);
        }

        public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request)
        {
            var user = await _unitOfWork.Users.GetByEmailAsync(request.Email);

            if (user == null)
                return Result<AuthResponse>.Failure("Invalid credentials", 401);

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                return Result<AuthResponse>.Failure("Invalid credentials", 401);

            var response = await GenerateAuthResponse(user);

            return Result<AuthResponse>.Success(response);
        }

        public async Task<Result<AuthResponse>> RefreshTokenAsync(string refreshToken)
        {
            var user = await _unitOfWork.Users.GetByRefreshTokenAsync(refreshToken);

            if (user == null)
                return Result<AuthResponse>.Failure("Invalid refresh token", 401);

            if (user.RefreshTokenExpiry < DateTime.UtcNow)
                return Result<AuthResponse>.Failure("Refresh token expired", 401);

            var response = await GenerateAuthResponse(user);

            return Result<AuthResponse>.Success(response);
        }

        private async Task<AuthResponse> GenerateAuthResponse(User user)
        {
            var accessToken = GenerateAccessToken(user);
            var refreshToken = GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(
                _config.GetValue<int>("Jwt:RefreshTokenExpiryDays", 7));
            
            await _unitOfWork.SaveChangesAsync();

            return new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                UserName = user.UserName,
                Role = user.Role.ToString()
            };
        }

        private static string GenerateRefreshToken()
        {
            var bytes = System.Security.Cryptography.RandomNumberGenerator.GetBytes(64);
            return Convert.ToBase64String(bytes);
        }

        private string GenerateAccessToken(User user)
        {
            var key = new SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            key.KeyId = "flowcore-key";
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub,user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email,user.Email),
                new Claim("role", user.Role.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_config.GetValue<int>("Jwt:ExpiryMinutes", 60)),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);

        }
    }
}
