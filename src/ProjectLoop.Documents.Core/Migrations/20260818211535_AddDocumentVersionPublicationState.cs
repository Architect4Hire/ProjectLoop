using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectLoop.Documents.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddDocumentVersionPublicationState : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPublished",
                schema: "documents",
                table: "DocumentVersions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "PublishedAtUtc",
                schema: "documents",
                table: "DocumentVersions",
                type: "datetimeoffset",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPublished",
                schema: "documents",
                table: "DocumentVersions");

            migrationBuilder.DropColumn(
                name: "PublishedAtUtc",
                schema: "documents",
                table: "DocumentVersions");
        }
    }
}
