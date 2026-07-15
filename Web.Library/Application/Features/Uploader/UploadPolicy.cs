namespace Web.Library.Application.Features.Uploader
{
    public enum UploadCategory
    {
        Image,
        Document
    }

    public sealed record UploadPolicy
    {
        public UploadCategory Category { get; init; }
        public long MaxSizeInBytes { get; init; }

        public HashSet<string> AllowedContentTypes { get; init; } = [];

        public bool IsAllowed(string contentType, long sizeInBytes)
        {
            return AllowedContentTypes.Contains(contentType) 
                && sizeInBytes <= MaxSizeInBytes;
        }
    }
}
