using MediatR;


namespace Web.Library.Application.Features.Category
{
    public sealed record GetCategoryTreeQuery(
            Guid? CategoryId
        ) : IRequest<IReadOnlyList<CategoryNode>>;
}
