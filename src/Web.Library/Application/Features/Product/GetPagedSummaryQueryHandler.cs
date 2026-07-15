using MediatR;
using Web.Library.Application.Abstractions;
using Web.Library.Application.Features.Product;
using Web.Library.Common;

namespace Web.Library.Application.Features.Product
{
    public class GetPagedSummaryQueryHandler : IRequestHandler<GetPagedSummaryQuery, PagedResult<ProductSummary>>
    {
        private readonly IProductRepository _productReader;
        private readonly ICategoryRepository _categoryReader;

        public GetPagedSummaryQueryHandler(IProductRepository productRepository, ICategoryRepository categoryRepository)
        {
            _productReader = productRepository;
            _categoryReader = categoryRepository;
        }

        public Task<PagedResult<ProductSummary>> Handle(GetPagedSummaryQuery request, CancellationToken cancellationToken)
        {
            // Check category id existance
            if(request.CategoryId.HasValue)
            {
                // Verify that the category exists
                var categoryExists = _categoryReader.ExistsAsync(request.CategoryId.Value).Result;
                if (!categoryExists)
                {
                    throw new InvalidOperationException("Category not found");
                }
            }

            var result = _productReader.GetPagedAsync(request.Page, request.PageSize, request.CategoryId, request.SearchTerm);

            return result;
        }
    }
}
