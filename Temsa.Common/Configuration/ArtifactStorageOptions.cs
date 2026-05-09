namespace Temsa.Common.Configuration;

public class ArtifactStorageOptions
{
    public const string SectionName = "ArtifactStorage";

    public string Endpoint { get; init; } = string.Empty;
    public string AccessKey { get; init; } = string.Empty;
    public string SecretKey { get; init; } = string.Empty;
    public string BucketName { get; init; } = "temsa-artifacts";
    public string Region { get; init; } = "us-east-1";
    public bool ForcePathStyle { get; init; } = true;
    public bool EnsureBucketExists { get; init; } = true;
}