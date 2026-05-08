using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Temsa.Contracts.Artifacts;
using Temsa.Core.Domain.Entities;

namespace Temsa.Core.Infrastructure.Persistence.Configurations;

public class ProjectArtifactConfiguration : IEntityTypeConfiguration<ProjectArtifact>
{
    public void Configure(EntityTypeBuilder<ProjectArtifact> builder)
    {
        builder.ToTable("project_artifacts");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id");

        builder.Property(x => x.ProjectId)
            .HasColumnName("project_id")
            .IsRequired();

        builder.Property(x => x.Type)
            .HasColumnName("type")
            .HasConversion(
                v => v.ToString().ToLowerInvariant(),
                v => Enum.Parse<ProjectArtifactType>(v, true))
            .IsRequired();

        builder.Property(x => x.Kind)
            .HasColumnName("kind")
            .HasConversion(
                v => v.ToString().ToLowerInvariant(),
                v => Enum.Parse<ArtifactKind>(v, true))
            .IsRequired();

        builder.Property(x => x.Bucket)
            .HasColumnName("bucket")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.ObjectKey)
            .HasColumnName("object_key")
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(x => x.FileName)
            .HasColumnName("file_name")
            .HasMaxLength(255);

        builder.Property(x => x.ContentType)
            .HasColumnName("content_type")
            .HasMaxLength(255);

        builder.Property(x => x.SizeBytes)
            .HasColumnName("size_bytes");

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();
        
        builder.HasOne(x => x.Project)
            .WithMany(x => x.Artifacts)
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.ProjectId);
        builder.HasIndex(x => new { x.ProjectId, x.Type });
    }
}