using FlowCore.Application.Common;
using FlowCore.Application.DTOs.Auth;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowCore.Application.Features.Auth.Commands.RefreshToken
{
    public class RefreshTokenCommand : IRequest<Result<AuthResponse>>
    {
        public string RefreshToken { get; set; } = string.Empty;
    }
}
