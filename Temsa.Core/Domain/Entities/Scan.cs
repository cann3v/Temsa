using Temsa.Core.Domain.Enums;

namespace Temsa.Core.Domain.Entities;

public class Scan
{
    public long Id { get; set; }
    public long ProjectId { get; set; }
    
    public PlatformType Platform { get; set; }
    public AnalysisType AnalysisType { get; set; }
    public ScanStatus Status { get; set; }
    public long InputArtifactId { get; set; }
    
    public string? CurrentStage { get; set; }
    public string? ErrorMessage { get; set; }
    
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public DateTimeOffset? StartedAt { get; set; }
    public DateTimeOffset? FinishedAt { get; set; }

    public Project Project { get; set; } = null!;
    public ICollection<ScanArtifact> Artifacts { get; set; } = new List<ScanArtifact>();
    public ICollection<ScanEvent> Events { get; set; } = new List<ScanEvent>();
    public ICollection<ScanTask> Tasks { get; set; } = new List<ScanTask>();
    public ProjectArtifact InputArtifact { get; set; } = null!;
}
