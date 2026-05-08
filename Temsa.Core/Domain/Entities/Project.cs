namespace Temsa.Core.Domain.Entities;

public class Project
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    
    public ICollection<Scan> Scans { get; set; } = new List<Scan>();
    public ICollection<ProjectArtifact> Artifacts { get; set; } = new List<ProjectArtifact>();
}
