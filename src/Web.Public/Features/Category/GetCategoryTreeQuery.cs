using MediatR;
using Web.Public.Repository;

namespace Web.Public.Features.Category
{
    public sealed record GetCategoryTreeQuery(
            Guid? CategoryId
        ) : IRequest<IReadOnlyList<CategoryNode>>;
}
