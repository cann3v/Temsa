using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Temsa.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddScanTaskStageFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "run_policy",
                table: "scan_tasks",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "stage_execution",
                table: "scan_tasks",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "stage_id",
                table: "scan_tasks",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "stage_order",
                table: "scan_tasks",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_scans_input_artifact_id",
                table: "scans",
                column: "input_artifact_id");

            migrationBuilder.CreateIndex(
                name: "IX_scan_tasks_scan_id_stage_id_order",
                table: "scan_tasks",
                columns: new[] { "scan_id", "stage_id", "order" });

            migrationBuilder.CreateIndex(
                name: "IX_scan_tasks_scan_id_stage_order",
                table: "scan_tasks",
                columns: new[] { "scan_id", "stage_order" });

            migrationBuilder.AddForeignKey(
                name: "FK_scans_project_artifacts_input_artifact_id",
                table: "scans",
                column: "input_artifact_id",
                principalTable: "project_artifacts",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_scans_project_artifacts_input_artifact_id",
                table: "scans");

            migrationBuilder.DropIndex(
                name: "IX_scans_input_artifact_id",
                table: "scans");

            migrationBuilder.DropIndex(
                name: "IX_scan_tasks_scan_id_stage_id_order",
                table: "scan_tasks");

            migrationBuilder.DropIndex(
                name: "IX_scan_tasks_scan_id_stage_order",
                table: "scan_tasks");

            migrationBuilder.DropColumn(
                name: "run_policy",
                table: "scan_tasks");

            migrationBuilder.DropColumn(
                name: "stage_execution",
                table: "scan_tasks");

            migrationBuilder.DropColumn(
                name: "stage_id",
                table: "scan_tasks");

            migrationBuilder.DropColumn(
                name: "stage_order",
                table: "scan_tasks");
        }
    }
}
