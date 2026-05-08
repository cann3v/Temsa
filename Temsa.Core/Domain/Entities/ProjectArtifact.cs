using Temsa.Contracts.Artifacts;

namespace Temsa.Core.Domain.Entities;

public class ProjectArtifact
{
    public long Id { get; set; }
    public long ProjectId { get; set; }
    
    public ProjectArtifactType Type { get; set; }
    public ArtifactKind Kind { get; set; }

    public string Bucket { get; set; } = string.Empty;
    public string ObjectKey { get; set; } = string.Empty;
    public string? FileName { get; set; }
    public string? ContentType { get; set; }
    public long? SizeBytes { get; set; }
    
    public DateTimeOffset CreatedAt { get; set; }

    public Project Project { get; set; } = null!;
}