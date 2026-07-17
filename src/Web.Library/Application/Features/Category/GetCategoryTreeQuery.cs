using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Web.Library.Application.Features.Category
{
    public sealed record GetCategoryTreeQuery(
            Guid? RootId
        ) : IRequest<IReadOnlyList<CategoryTreeNode>>;
}
