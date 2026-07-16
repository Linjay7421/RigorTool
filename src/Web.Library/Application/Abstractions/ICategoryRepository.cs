using Web.Library.Application.Features.Category;

namespace Web.Library.Application.Abstractions
{
    public interface ICategoryRepository
    {
        Task<IReadOnlyList<Category>> GetLookupAsync();
        Task<IReadOnlyList<Category>> GetByIdAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
    }
}
