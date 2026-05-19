using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Temsa.Core.Domain.Entities;
using Temsa.Core.Domain.Enums;

namespace Temsa.Core.Infrastructure.Persistence.Configurations;

public class ScanTaskConfiguration : IEntityTypeConfiguration<ScanTask>
{
    public void Configure(EntityTypeBuilder<ScanTask> builder)
    {
        builder.ToTable("scan_tasks");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id");

        builder.Property(x => x.ScanId)
            .HasColumnName("scan_id")
            .IsRequired();

        builder.Property(x => x.Order)
            .HasColumnName("order")
            .IsRequired();

        builder.Property(x => x.TaskType)
            .HasColumnName("task_type")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.WorkerType)
            .HasColumnName("worker_type")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasColumnName("status")
            .HasConversion(
                v => v.ToString().ToLowerInvariant(),
                v => Enum.Parse<ScanTaskStatus>(v, true))
            .IsRequired();
        
        builder.Property(x => x.StageId)
            .HasColumnName("stage_id")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.StageOrder)
            .HasColumnName("stage_order")
            .IsRequired();

        builder.Property(x => x.StageExecution)
            .HasColumnName("stage_execution")
            .HasMaxLength(30)
            .IsRequired();
        
        builder.Property(x => x.RunPolicy)
            .HasColumnName("run_policy")
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(x => x.Tool)
            .HasColumnName("tool")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Attempt)
            .HasColumnName("attempt")
            .HasDefaultValue(1)
            .IsRequired();

        builder.Property(x => x.PayloadJson)
            .HasColumnName("payload_json")
            .HasColumnType("jsonb");

        builder.Property(x => x.ResultJson)
            .HasColumnName("result_json")
            .HasColumnType("jsonb");

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

        builder.HasOne(x => x.Scan)
            .WithMany(x => x.Tasks)
            .HasForeignKey(x => x.ScanId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.ScanId);
        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => new { x.ScanId, x.TaskType });
        builder.HasIndex(x => new { x.WorkerType, x.Status });
        builder.HasIndex(x => new { x.ScanId, x.Order });
        builder.HasIndex(x => new { x.ScanId, x.StageOrder });
        builder.HasIndex(x => new { x.ScanId, x.StageId, x.Order });
    }
}
