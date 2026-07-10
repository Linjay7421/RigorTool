using Web.Public.Domains;

namespace Web.Public.Repository
{
    public interface IStoredFileRepository
    {
        Task AddAsync(StoredFile file, CancellationToken cancellationToken);

        Task<StoredFile?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

        Task<StoredFile?> GetByObjectKeyAsync(
            string objectKey,
            CancellationToken cancellationToken);
    }
}
