using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProjectLoop.Approvals.Core;

public sealed class ApprovalDecisionConfiguration : IEntityTypeConfiguration<ApprovalDecision>
{
    public void Configure(EntityTypeBuilder<ApprovalDecision> builder)
    {
        builder.ToTable("ApprovalDecisions");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.TenantId)
            .IsRequired();

        builder.Property(d => d.ApprovalRequestId)
            .IsRequired();

        builder.Property(d => d.TargetType)
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(d => d.TargetId)
            .IsRequired();

        builder.Property(d => d.TargetVersionId);

        builder.Property(d => d.ApproverUserId)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(d => d.Decision)
            .HasConversion<string>()
            .HasMaxLength(16)
            .IsRequired();

        builder.Property(d => d.Comments)
            .HasMaxLength(2048);

        builder.Property(d => d.DecidedAtUtc)
            .IsRequired();

        builder.Property(d => d.CorrelationId)
            .HasMaxLength(128);

        // No update/delete convenience is modeled here: every property above
        // is configured init-only on the CLR type, so EF only ever emits
        // INSERT for this entity — decision history is append-only.
        builder.HasIndex(d => new { d.TenantId, d.ApprovalRequestId });

        // Restrict, not Cascade: decision evidence is immutable audit history
        // and must survive independently of the parent ApprovalRequest row.
        builder.HasOne<ApprovalRequest>()
            .WithMany()
            .HasForeignKey(d => d.ApprovalRequestId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();
    }
}
