using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Temsa.Common.Configuration;
using Temsa.Contracts.Artifacts;

namespace Temsa.Common.Storage;

public class S3ArtifactStorage(
    IAmazonS3 s3Client,
    IOptions<ArtifactStorageOptions> options,
    ILogger<S3ArtifactStorage> logger) : IArtifactStorage
{
    private readonly IAmazonS3 _s3Client = s3Client;
    private readonly ArtifactStorageOptions _options = options.Value;
    private readonly ILogger<S3ArtifactStorage> _logger = logger;

    public async Task<StoredArtifactDescriptor> UploadAsync(
        Stream content,
        string objectKey,
        string? fileName,
        string? contentType,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(objectKey);

        if (_options.EnsureBucketExists)
        {
            await EnsureBucketExistsAsync(cancellationToken);
        }

        long? sizeBytes = content.CanSeek ? content.Length : null;

        if (content.CanSeek)
        {
            content.Position = 0;
        }

        var request = new PutObjectRequest
        {
            BucketName = _options.BucketName,
            Key = objectKey,
            InputStream = content,
            ContentType = string.IsNullOrWhiteSpace(contentType)
                ? "application/octet-stream"
                : contentType,
            AutoCloseStream = false
        };

        if (!string.IsNullOrWhiteSpace(fileName))
        {
            request.Metadata["file-name"] = fileName;
        }

        await _s3Client.PutObjectAsync(request, cancellationToken);

        _logger.LogInformation("Uploaded artifact to S3 bucket {Bucket}, object key {ObjectKey}",
            _options.BucketName,
            objectKey);

        return new StoredArtifactDescriptor(
            Bucket: _options.BucketName,
            ObjectKey: objectKey,
            FileName: fileName,
            ContentType: contentType,
            SizeBytes: sizeBytes);
    }

    private async Task EnsureBucketExistsAsync(CancellationToken cancellationToken)
    {
        var exists = await AmazonS3Util.DoesS3BucketExistV2Async(
            _s3Client,
            _options.BucketName);

        if (exists)
        {
            return;
        }

        await _s3Client.PutBucketAsync(
            new PutBucketRequest
            {
                BucketName = _options.BucketName
            },
            cancellationToken);
        
        _logger.LogInformation("Created S3 bucket {Bucket}", _options.BucketName);
    }
}