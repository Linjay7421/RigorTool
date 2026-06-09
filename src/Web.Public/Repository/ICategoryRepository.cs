namespace Web.Public.Repository
{
    public interface ICategoryRepository
    {
        Task<IReadOnlyList<Category>> GetAllAsync();
        Task<IReadOnlyList<Category>> GetByIdAsync(Guid id);
    }
}
