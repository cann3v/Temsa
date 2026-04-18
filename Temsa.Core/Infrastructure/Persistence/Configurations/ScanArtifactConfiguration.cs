using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Temsa.Core.Domain.Entities;
using Temsa.Core.Domain.Enums;

namespace Temsa.Core.Infrastructure.Persistence.Configurations;

public class ScanArtifactConfiguration : IEntityTypeConfiguration<ScanArtifact>
{
    public void Configure(EntityTypeBuilder<ScanArtifact> builder)
    {
        builder.ToTable("scan_artifacts");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id");

        builder.Property(x => x.ScanId)
            .HasColumnName("scan_id")
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

        builder.HasOne(x => x.Scan)
            .WithMany(x => x.Artifacts)
            .HasForeignKey(x => x.ScanId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.ScanId);
        builder.HasIndex(x => new { x.ScanId, x.Kind });
    }
}