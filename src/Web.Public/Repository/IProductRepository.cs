using Web.Public.Common;

namespace Web.Public.Repository
{
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(Guid productId);
        Task<List<Product>> GetAllAsync();
        Task<PagedResult<Product>> GetPagedAsync(int pageNumber, int pageSize, Guid? categoryId = null, string? keyword = null);
    }
}
