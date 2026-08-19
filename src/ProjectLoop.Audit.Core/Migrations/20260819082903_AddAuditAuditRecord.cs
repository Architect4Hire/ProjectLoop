using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectLoop.Audit.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditAuditRecord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "audit");

            migrationBuilder.CreateTable(
                name: "AuditRecords",
                schema: "audit",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActorUserId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Action = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ResourceType = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ResourceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ResourceVersionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    OccurredAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CorrelationId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CausationId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Source = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    BeforeMetadata = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    AfterMetadata = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditRecords", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuditRecords_TenantId_OccurredAtUtc",
                schema: "audit",
                table: "AuditRecords",
                columns: new[] { "TenantId", "OccurredAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_AuditRecords_TenantId_ResourceType_ResourceId",
                schema: "audit",
                table: "AuditRecords",
                columns: new[] { "TenantId", "ResourceType", "ResourceId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditRecords",
                schema: "audit");
        }
    }
}
