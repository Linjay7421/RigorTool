namespace Web.Library.Domain
{
    public sealed class StoredFile
    {
        public Guid Id { get; private set; }
        public string OriginalName { get; private set; }
        public StoredFileCategory Category { get; private set; }
        public string MimeType { get; private set; }
        public string Extension { get; private set; }
        public long SizeInBytes { get; private set; }
        public string ContentHash { get; private set; }
        public DateTimeOffset CreatedAt { get; private set; }
        public string ObjectKey { get; private set; }

        private StoredFile(
            Guid id,
            string originalName,
            StoredFileCategory category,
            string mimeType,
            string extension,
            long sizeInBytes,
            string contentHash,
            string objectKey,
            DateTimeOffset createdAt) 
        {
            Id = id;
            OriginalName = originalName;
            Category = category;
            MimeType = mimeType;
            Extension = extension;
            SizeInBytes = sizeInBytes;
            ContentHash = contentHash;
            CreatedAt = createdAt;
            ObjectKey = objectKey;
        }


        public static StoredFile Create(
            Guid id,
            string originalName,
            StoredFileCategory category,
            string mimeType,
            string extension,
            long sizeInBytes,
            string contentHash,
            string objectKey,
            DateTimeOffset? createdAt = null)
        {
            if (sizeInBytes <= 0)
                throw new ArgumentOutOfRangeException(nameof(sizeInBytes));

            ArgumentException.ThrowIfNullOrWhiteSpace(originalName);
            ArgumentException.ThrowIfNullOrWhiteSpace(objectKey);
            ArgumentException.ThrowIfNullOrWhiteSpace(mimeType);
            ArgumentException.ThrowIfNullOrWhiteSpace(extension);
            ArgumentException.ThrowIfNullOrWhiteSpace(contentHash);

            extension = extension.TrimStart('.').ToLowerInvariant();

            return new StoredFile(
                id,
                originalName,
                category,
                mimeType,
                extension,
                sizeInBytes,
                contentHash,
                objectKey,
                createdAt ?? DateTimeOffset.UtcNow);
        }
    }
}
