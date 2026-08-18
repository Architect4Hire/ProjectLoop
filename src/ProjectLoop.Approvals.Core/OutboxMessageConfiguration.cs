using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProjectLoop.Approvals.Core;

public sealed class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("OutboxMessages");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.EventId)
            .IsRequired();

        builder.Property(m => m.EventType)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(m => m.EventVersion)
            .IsRequired();

        builder.Property(m => m.Payload)
            .IsRequired();

        builder.Property(m => m.CorrelationId)
            .HasMaxLength(128);

        builder.Property(m => m.Status)
            .HasConversion<string>()
            .HasMaxLength(16)
            .IsRequired();

        builder.Property(m => m.AttemptCount)
            .IsRequired();

        builder.Property(m => m.CreatedAtUtc)
            .IsRequired();

        builder.Property(m => m.ProcessedAtUtc);

        builder.Property(m => m.LastAttemptedAtUtc);

        builder.Property(m => m.LastError)
            .HasMaxLength(1024);

        builder.HasIndex(m => m.EventId)
            .IsUnique();

        builder.HasIndex(m => new { m.Status, m.CreatedAtUtc });
    }
}
