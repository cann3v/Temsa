using Microsoft.EntityFrameworkCore;
using Temsa.Core.Domain.Entities;

namespace Temsa.Core.Infrastructure.Persistence;

public class TemsaDbContext(DbContextOptions<TemsaDbContext> options): DbContext(options)
{
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<Scan> Scans => Set<Scan>();
    public DbSet<ScanArtifact> ScanArtifacts => Set<ScanArtifact>();
    public DbSet<ScanTask> ScanTasks => Set<ScanTask>();
    public DbSet<ScanEvent> ScanEvents => Set<ScanEvent>();
    public DbSet<ProjectArtifact> ProjectArtifacts => Set<ProjectArtifact>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TemsaDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
