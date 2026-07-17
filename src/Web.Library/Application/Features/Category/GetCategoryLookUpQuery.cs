using MediatR;


namespace Web.Library.Application.Features.Category
{
    public sealed record GetCategoryLookUpQuery(
            Guid? CategoryId
        ) : IRequest<IReadOnlyList<CategoryNode>>;
}
