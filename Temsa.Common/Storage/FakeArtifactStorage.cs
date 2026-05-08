using Temsa.Contracts.Artifacts;

namespace Temsa.Common.Storage;

public class FakeArtifactStorage : IArtifactStorage
{
    private const string BucketName = "temsa-artifacts";

    public Task<StoredArtifactDescriptor> UploadAsync(
        Stream content,
        string objectKey,
        string? fileName,
        string? contentType,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new StoredArtifactDescriptor(
            Bucket: BucketName,
            ObjectKey: objectKey,
            FileName: fileName,
            ContentType: contentType,
            SizeBytes: content.CanSeek ? content.Length : null));
    }
}