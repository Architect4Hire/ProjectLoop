using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProjectLoop.Identity.Core;

public sealed class TenantMembershipConfiguration : IEntityTypeConfiguration<TenantMembership>
{
    public void Configure(EntityTypeBuilder<TenantMembership> builder)
    {
        builder.ToTable("TenantMemberships");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.UserId)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(m => m.TenantId)
            .IsRequired();

        builder.Property(m => m.Role)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(m => m.Status)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(m => m.CreatedAtUtc)
            .IsRequired();

        builder.Property(m => m.UpdatedAtUtc)
            .IsRequired()
            .IsConcurrencyToken();

        // Only one active membership per user/tenant pair; a revoked
        // membership does not block re-invitation of the same user.
        builder.HasIndex(m => new { m.UserId, m.TenantId, m.Status })
            .IsUnique()
            .HasFilter("[Status] = 'Active'");

        builder.HasIndex(m => m.TenantId);
    }
}
