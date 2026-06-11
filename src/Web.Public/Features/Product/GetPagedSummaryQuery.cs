using MediatR;
using Web.Public.Common;
using Web.Public.Features.Product.Models;

namespace Web.Public.Features.Product
{
    public sealed record GetPagedSummaryQuery
    (
        int Page = 1,
        int PageSize = 10,
        Guid? CategoryId = null,
        string? SearchTerm = null
    ) : IRequest<PagedResult<ProductSummary>>;
}
