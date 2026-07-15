using static System.Net.WebRequestMethods;

namespace Web.Library.Application.Abstractions
{
    public interface IFileStorage
    {
        Task SaveAsync(string objectKey, Stream content, CancellationToken cancellationToken);

        Task<Stream?> OpenReadAsync(string objectKey,  CancellationToken cancellationToken);

        Task DeleteAsync(string objectKey, CancellationToken cancellationToken);
    }
}
