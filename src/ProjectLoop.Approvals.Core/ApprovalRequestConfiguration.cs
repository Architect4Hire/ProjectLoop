using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProjectLoop.Approvals.Core;

public sealed class ApprovalRequestConfiguration : IEntityTypeConfiguration<ApprovalRequest>
{
    public void Configure(EntityTypeBuilder<ApprovalRequest> builder)
    {
        builder.ToTable("ApprovalRequests");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.TenantId)
            .IsRequired();

        builder.Property(r => r.ProjectId)
            .IsRequired();

        builder.Property(r => r.TargetType)
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(r => r.TargetId)
            .IsRequired();

        builder.Property(r => r.TargetVersionId);

        builder.Property(r => r.RequestedByUserId)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(r => r.RequestedAtUtc)
            .IsRequired();

        builder.Property(r => r.Status)
            .HasConversion<string>()
            .HasMaxLength(16)
            .IsRequired();

        builder.Property(r => r.CorrelationId)
            .HasMaxLength(128);

        builder.HasIndex(r => new { r.TenantId, r.ProjectId });

        // Exact-identity lookup for the target a request was raised against —
        // an approval always binds to one specific resource (and, where
        // applicable, one specific immutable version).
        builder.HasIndex(r => new { r.TenantId, r.TargetType, r.TargetId, r.TargetVersionId });
    }
}
