using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProjectLoop.Notifications.Core;

public sealed class NotificationDeliveryConfiguration : IEntityTypeConfiguration<NotificationDelivery>
{
    public void Configure(EntityTypeBuilder<NotificationDelivery> builder)
    {
        builder.ToTable("NotificationDeliveries");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.TenantId)
            .IsRequired();

        builder.Property(d => d.SourceEventId)
            .IsRequired();

        builder.Property(d => d.NotificationType)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(d => d.RecipientUserId)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(d => d.CorrelationId)
            .HasMaxLength(128);

        builder.Property(d => d.Status)
            .HasConversion<string>()
            .HasMaxLength(16)
            .IsRequired();

        builder.Property(d => d.AttemptCount)
            .IsRequired();

        builder.Property(d => d.CreatedAtUtc)
            .IsRequired();

        builder.Property(d => d.SentAtUtc);

        builder.Property(d => d.LastAttemptedAtUtc);

        builder.Property(d => d.LastError)
            .HasMaxLength(1024);

        builder.HasIndex(d => d.SourceEventId);

        builder.HasIndex(d => new { d.Status, d.CreatedAtUtc });
    }
}
