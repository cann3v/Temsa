using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Temsa.Core.Domain.Entities;

namespace Temsa.Core.Infrastructure.Persistence.Configurations;

public class ScanEventConfiguration : IEntityTypeConfiguration<ScanEvent>
{
    public void Configure(EntityTypeBuilder<ScanEvent> builder)
    {
        builder.ToTable("scan_events");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id");

        builder.Property(x => x.ScanId)
            .HasColumnName("scan_id")
            .IsRequired();

        builder.Property(x => x.EventType)
            .HasColumnName("event_type")
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(x => x.PayloadJson)
            .HasColumnName("payload_json")
            .HasColumnType("jsonb");

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.HasOne(x => x.Scan)
            .WithMany(x => x.Events)
            .HasForeignKey(x => x.ScanId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.ScanId);
        builder.HasIndex(x => x.CreatedAt);
        builder.HasIndex(x => x.EventType);
    }
}