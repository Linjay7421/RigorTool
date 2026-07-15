using Web.Library.Application.Abstractions;

namespace Web.Infrastructure.Providers
{
    public sealed class DefaultObjectKeyGenerator : IObjectKeyGenerator
    {
        public string Generate(string prefix, Guid fileId, string extension)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(prefix);
            ArgumentException.ThrowIfNullOrWhiteSpace(extension);

            extension = extension.TrimStart('.').ToLowerInvariant();

            prefix = NormalizePrefix(prefix);

            return $"{prefix}/{fileId}.{extension}";
        }

        private static string NormalizePrefix(string prefix)
        {
            return prefix.Trim('/');
        }
    }
}
