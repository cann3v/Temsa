using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Temsa.Core.Migrations
{
    /// <inheritdoc />
    public partial class ScanandScanTaskmodelsupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "started_at",
                table: "scans",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "finished_at",
                table: "scans",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AddColumn<long>(
                name: "input_artifact_id",
                table: "scans",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<int>(
                name: "order",
                table: "scan_tasks",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "tool",
                table: "scan_tasks",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_scan_tasks_scan_id_order",
                table: "scan_tasks",
                columns: new[] { "scan_id", "order" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_scan_tasks_scan_id_order",
                table: "scan_tasks");

            migrationBuilder.DropColumn(
                name: "input_artifact_id",
                table: "scans");

            migrationBuilder.DropColumn(
                name: "order",
                table: "scan_tasks");

            migrationBuilder.DropColumn(
                name: "tool",
                table: "scan_tasks");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "started_at",
                table: "scans",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "finished_at",
                table: "scans",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldNullable: true);
        }
    }
}
