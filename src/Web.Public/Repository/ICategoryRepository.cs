using Web.Public.Features.Category.Models;

namespace Web.Public.Repository
{
    public interface ICategoryRepository
    {
        Task<IReadOnlyList<Category>> GetAllAsync();
        Task<IReadOnlyList<Category>> GetByIdAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
    }
}
