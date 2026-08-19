using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProjectLoop.Audit.Core;

/// <summary>
/// Append-only-oriented mapping for AuditRecord. There is deliberately no
/// concurrency token or mutable-update support surfaced here: the
/// repository layer that will sit on top of this mapping only ever inserts
/// new rows, never updates or deletes existing ones.
/// </summary>
public sealed class AuditRecordConfiguration : IEntityTypeConfiguration<AuditRecord>
{
    public void Configure(EntityTypeBuilder<AuditRecord> builder)
    {
        builder.ToTable("AuditRecords");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.TenantId)
            .IsRequired();

        builder.Property(r => r.ActorUserId)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(r => r.Action)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(r => r.ResourceType)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(r => r.ResourceId)
            .IsRequired();

        builder.Property(r => r.ResourceVersionId);

        builder.Property(r => r.OccurredAtUtc)
            .IsRequired();

        builder.Property(r => r.CorrelationId)
            .HasMaxLength(128);

        builder.Property(r => r.CausationId)
            .HasMaxLength(128);

        builder.Property(r => r.Source)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(r => r.BeforeMetadata)
            .HasMaxLength(2048);

        builder.Property(r => r.AfterMetadata)
            .HasMaxLength(2048);

        // Supports tenant-scoped audit history queries ordered by time.
        builder.HasIndex(r => new { r.TenantId, r.OccurredAtUtc });

        // Supports "show audit history for this resource" queries.
        builder.HasIndex(r => new { r.TenantId, r.ResourceType, r.ResourceId });
    }
}
