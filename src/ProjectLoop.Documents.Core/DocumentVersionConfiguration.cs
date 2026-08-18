using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProjectLoop.Documents.Core;

public sealed class DocumentVersionConfiguration : IEntityTypeConfiguration<DocumentVersion>
{
    public void Configure(EntityTypeBuilder<DocumentVersion> builder)
    {
        builder.ToTable("DocumentVersions");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.DocumentId)
            .IsRequired();

        builder.Property(v => v.VersionNumber)
            .IsRequired();

        builder.Property(v => v.BlobObjectKey)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(v => v.MimeType)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(v => v.SizeBytes)
            .IsRequired();

        builder.Property(v => v.ContentHash)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(v => v.UploadedByUserId)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(v => v.CreatedAtUtc)
            .IsRequired();

        builder.Property(v => v.IsPublished)
            .IsRequired();

        builder.Property(v => v.PublishedAtUtc);

        builder.HasIndex(v => new { v.DocumentId, v.VersionNumber })
            .IsUnique();

        // Restrict, not Cascade: version history is immutable append-only
        // evidence and must survive independently of the parent Document row.
        builder.HasOne<Document>()
            .WithMany()
            .HasForeignKey(v => v.DocumentId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();
    }
}
