using Web.Library.Domain;

namespace Web.Library.Application.Abstractions
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
