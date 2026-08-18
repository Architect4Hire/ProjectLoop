using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProjectLoop.Identity.Core;

public sealed class ClientInvitationConfiguration : IEntityTypeConfiguration<ClientInvitation>
{
    public void Configure(EntityTypeBuilder<ClientInvitation> builder)
    {
        builder.ToTable("ClientInvitations");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.TenantId)
            .IsRequired();

        builder.Property(i => i.Email)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(i => i.TokenHash)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(i => i.ExpiresAtUtc)
            .IsRequired();

        builder.Property(i => i.Status)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(i => i.InvitedByUserId)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(i => i.CreatedAtUtc)
            .IsRequired();

        builder.Property(i => i.UpdatedAtUtc)
            .IsRequired()
            .IsConcurrencyToken();

        builder.HasIndex(i => i.TokenHash)
            .IsUnique();

        builder.HasIndex(i => new { i.TenantId, i.Email });
    }
}
