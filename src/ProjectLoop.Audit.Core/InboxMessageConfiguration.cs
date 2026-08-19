using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProjectLoop.Audit.Core;

public sealed class InboxMessageConfiguration : IEntityTypeConfiguration<InboxMessage>
{
    public void Configure(EntityTypeBuilder<InboxMessage> builder)
    {
        builder.ToTable("InboxMessages");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.MessageId)
            .IsRequired();

        builder.Property(m => m.EventType)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(m => m.CorrelationId)
            .HasMaxLength(128);

        builder.Property(m => m.ProcessedAtUtc)
            .IsRequired();

        builder.HasIndex(m => m.MessageId)
            .IsUnique();
    }
}
