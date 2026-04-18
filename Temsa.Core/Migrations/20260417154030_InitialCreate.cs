using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Temsa.Core.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "projects",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_projects", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "scans",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    project_id = table.Column<long>(type: "bigint", nullable: false),
                    platform = table.Column<string>(type: "text", nullable: false),
                    analysis_type = table.Column<string>(type: "text", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false),
                    current_stage = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    error_message = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    started_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    finished_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_scans", x => x.id);
                    table.ForeignKey(
                        name: "FK_scans_projects_project_id",
                        column: x => x.project_id,
                        principalTable: "projects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "scan_artifacts",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    scan_id = table.Column<long>(type: "bigint", nullable: false),
                    kind = table.Column<string>(type: "text", nullable: false),
                    bucket = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    object_key = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    file_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    content_type = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    size_bytes = table.Column<long>(type: "bigint", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_scan_artifacts", x => x.id);
                    table.ForeignKey(
                        name: "FK_scan_artifacts_scans_scan_id",
                        column: x => x.scan_id,
                        principalTable: "scans",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "scan_events",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    scan_id = table.Column<long>(type: "bigint", nullable: false),
                    event_type = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    payload_json = table.Column<string>(type: "jsonb", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_scan_events", x => x.id);
                    table.ForeignKey(
                        name: "FK_scan_events_scans_scan_id",
                        column: x => x.scan_id,
                        principalTable: "scans",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "scan_tasks",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    scan_id = table.Column<long>(type: "bigint", nullable: false),
                    task_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    worker_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    status = table.Column<string>(type: "text", nullable: false),
                    attempt = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    payload_json = table.Column<string>(type: "jsonb", nullable: true),
                    result_json = table.Column<string>(type: "jsonb", nullable: true),
                    error_message = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    started_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    finished_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_scan_tasks", x => x.id);
                    table.ForeignKey(
                        name: "FK_scan_tasks_scans_scan_id",
                        column: x => x.scan_id,
                        principalTable: "scans",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_projects_name",
                table: "projects",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_scan_artifacts_scan_id",
                table: "scan_artifacts",
                column: "scan_id");

            migrationBuilder.CreateIndex(
                name: "IX_scan_artifacts_scan_id_kind",
                table: "scan_artifacts",
                columns: new[] { "scan_id", "kind" });

            migrationBuilder.CreateIndex(
                name: "IX_scan_events_created_at",
                table: "scan_events",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "IX_scan_events_event_type",
                table: "scan_events",
                column: "event_type");

            migrationBuilder.CreateIndex(
                name: "IX_scan_events_scan_id",
                table: "scan_events",
                column: "scan_id");

            migrationBuilder.CreateIndex(
                name: "IX_scan_tasks_scan_id",
                table: "scan_tasks",
                column: "scan_id");

            migrationBuilder.CreateIndex(
                name: "IX_scan_tasks_scan_id_task_type",
                table: "scan_tasks",
                columns: new[] { "scan_id", "task_type" });

            migrationBuilder.CreateIndex(
                name: "IX_scan_tasks_status",
                table: "scan_tasks",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_scan_tasks_worker_type_status",
                table: "scan_tasks",
                columns: new[] { "worker_type", "status" });

            migrationBuilder.CreateIndex(
                name: "IX_scans_created_at",
                table: "scans",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "IX_scans_project_id",
                table: "scans",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "IX_scans_status",
                table: "scans",
                column: "status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "scan_artifacts");

            migrationBuilder.DropTable(
                name: "scan_events");

            migrationBuilder.DropTable(
                name: "scan_tasks");

            migrationBuilder.DropTable(
                name: "scans");

            migrationBuilder.DropTable(
                name: "projects");
        }
    }
}
