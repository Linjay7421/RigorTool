using Web.Library.Application.Features.Product;
using Web.Library.Common;

namespace Web.Library.Application.Abstractions
{
    public interface IProductRepository
    {
        Task<ProductSummary?> GetByIdAsync(Guid productId);
        Task<List<ProductSummary>> GetAllAsync();
        Task<PagedResult<ProductSummary>> GetPagedAsync(int pageNumber, int pageSize, Guid? categoryId = null, string? keyword = null);
    }
}
