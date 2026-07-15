using MediatR;
using Web.Library.Application.Features.Product;
using Web.Library.Common;

namespace Web.Library.Application.Features.Product
{
    public sealed record GetPagedSummaryQuery
    (
        int Page = 1,
        int PageSize = 10,
        Guid? CategoryId = null,
        string? SearchTerm = null
    ) : IRequest<PagedResult<ProductSummary>>;
}
