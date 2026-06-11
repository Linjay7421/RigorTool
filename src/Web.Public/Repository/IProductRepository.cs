using Web.Public.Common;
using Web.Public.Features.Product.Models;

namespace Web.Public.Repository
{
    public interface IProductRepository
    {
        Task<ProductSummary?> GetByIdAsync(Guid productId);
        Task<List<ProductSummary>> GetAllAsync();
        Task<PagedResult<ProductSummary>> GetPagedAsync(int pageNumber, int pageSize, Guid? categoryId = null, string? keyword = null);
    }
}
