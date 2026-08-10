using FlowCore.Application.Common;
using FlowCore.Application.DTOs.Category;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowCore.Application.Features.Categories.Commands.CreateCategory
{
    public class CreateCategoryCommand : IRequest<Result<CategoryResponse>>
    {
        public string Name { get; set; } = string.Empty;
    }
}
