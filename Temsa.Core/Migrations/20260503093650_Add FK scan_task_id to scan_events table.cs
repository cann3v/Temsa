using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Temsa.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddFKscan_task_idtoscan_eventstable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "scan_task_id",
                table: "scan_events",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_scan_events_scan_id_scan_task_id_created_at",
                table: "scan_events",
                columns: new[] { "scan_id", "scan_task_id", "created_at" });

            migrationBuilder.CreateIndex(
                name: "IX_scan_events_scan_task_id",
                table: "scan_events",
                column: "scan_task_id");

            migrationBuilder.AddForeignKey(
                name: "FK_scan_events_scan_tasks_scan_task_id",
                table: "scan_events",
                column: "scan_task_id",
                principalTable: "scan_tasks",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_scan_events_scan_tasks_scan_task_id",
                table: "scan_events");

            migrationBuilder.DropIndex(
                name: "IX_scan_events_scan_id_scan_task_id_created_at",
                table: "scan_events");

            migrationBuilder.DropIndex(
                name: "IX_scan_events_scan_task_id",
                table: "scan_events");

            migrationBuilder.DropColumn(
                name: "scan_task_id",
                table: "scan_events");
        }
    }
}
