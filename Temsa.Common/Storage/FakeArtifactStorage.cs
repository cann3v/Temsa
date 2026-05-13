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

    public Task DownloadAsync(
        string bucket,
        string objectKey,
        Stream destination,
        CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException(
            "Fake artifact storage does not store artifact content and cannot download artifacts");
    }
}