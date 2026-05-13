using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Temsa.Core.Domain.Entities;
using Temsa.Core.Domain.Enums;

namespace Temsa.Core.Infrastructure.Persistence.Configurations;

public class ScanConfiguration: IEntityTypeConfiguration<Scan>
{
    public void Configure(EntityTypeBuilder<Scan> builder)
    {
        builder.ToTable("scans");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id");
        
        builder.Property(x => x.ProjectId)
            .HasColumnName("project_id")
            .IsRequired();
        
        builder.Property(x => x.Platform)
            .HasColumnName("platform")
            .HasConversion(
                v => v == PlatformType.Android ? "android" : "ios",
                v => v == "android" ? PlatformType.Android : PlatformType.Ios)
            .IsRequired();
        
        builder.Property(x => x.AnalysisType)
            .HasColumnName("analysis_type")
            .HasConversion(
                v => v == AnalysisType.Static ? "static" : "dynamic",
                v => v == "static" ? AnalysisType.Static : AnalysisType.Dynamic)
            .IsRequired();
        
        builder.Property(x => x.Status)
            .HasColumnName("status")
            .HasConversion(
                v => v.ToString().ToLowerInvariant(),
                v => Enum.Parse<ScanStatus>(v, true))
            .IsRequired();
        
        builder.Property(x => x.InputArtifactId)
            .HasColumnName("input_artifact_id")
            .IsRequired();
        
        builder.Property(x => x.CurrentStage)
            .HasColumnName("current_stage")
            .HasMaxLength(100);

        builder.Property(x => x.ErrorMessage)
            .HasColumnName("error_message");

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        builder.Property(x => x.StartedAt)
            .HasColumnName("started_at");

        builder.Property(x => x.FinishedAt)
            .HasColumnName("finished_at");

        builder.HasOne(x => x.Project)
            .WithMany(x => x.Scans)
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.InputArtifact)
            .WithMany()
            .HasForeignKey(x => x.InputArtifactId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.ProjectId);
        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => x.CreatedAt);
    }
}
