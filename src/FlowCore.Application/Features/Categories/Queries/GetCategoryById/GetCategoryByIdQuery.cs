using FlowCore.Application.Common;
using FlowCore.Application.DTOs.Category;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowCore.Application.Features.Categories.Queries.GetCategoryById
{
    public class GetCategoryByIdQuery : IRequest<Result<CategoryResponse>>
    {
        public Guid Id { get; set; }
    }
}
