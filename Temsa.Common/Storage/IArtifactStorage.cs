using Temsa.Contracts.Artifacts;

namespace Temsa.Common.Storage;

public interface IArtifactStorage
{
    Task<StoredArtifactDescriptor> UploadAsync(
        Stream content,
        string objectKey,
        string? fileName,
        string? contentType,
        CancellationToken cancellationToken = default);
    
    Task DownloadAsync(
        string bucket,
        string objectKey,
        Stream destination,
        CancellationToken cancellationToken = default);
}