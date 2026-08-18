using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProjectLoop.Documents.Core;

public sealed class DocumentConfiguration : IEntityTypeConfiguration<Document>
{
    public void Configure(EntityTypeBuilder<Document> builder)
    {
        builder.ToTable("Documents");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.TenantId)
            .IsRequired();

        builder.Property(d => d.ProjectId)
            .IsRequired();

        builder.Property(d => d.Title)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(d => d.Category)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(d => d.Status)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(d => d.Visibility)
            .HasConversion<string>()
            .HasMaxLength(16)
            .IsRequired();

        // CurrentVersionId is a soft pointer into DocumentVersion, not an FK:
        // an FK in both directions between Document and DocumentVersion would
        // create a multi-path cascade cycle in SQL Server.
        builder.Property(d => d.CurrentVersionId);

        builder.Property(d => d.CreatedAtUtc)
            .IsRequired();

        builder.Property(d => d.UpdatedAtUtc)
            .IsRequired()
            .IsConcurrencyToken();

        builder.HasIndex(d => new { d.TenantId, d.ProjectId });
    }
}
