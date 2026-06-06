using Web.Public.Common;

namespace Web.Public.Repository
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllProductsAsync();
        Task<Product> GetProductByIdAsync(Guid id);
        Task<IEnumerable<Product>> GetProductsByCategoryAsync(string category);
        Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm);
        Task<PagedResult<Product>> GetProductsPagedAsync(int pageNumber, int pageSize);
    }
}
