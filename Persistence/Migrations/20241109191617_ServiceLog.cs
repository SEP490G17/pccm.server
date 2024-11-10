using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ServiceLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceLog_CourtClusters_CourtClusterId",
                table: "ServiceLog");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceLog_Services_ServiceId",
                table: "ServiceLog");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceLog_Users_CreatorId",
                table: "ServiceLog");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ServiceLog",
                table: "ServiceLog");

            migrationBuilder.RenameTable(
                name: "ServiceLog",
                newName: "ServiceLogs");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceLog_ServiceId",
                table: "ServiceLogs",
                newName: "IX_ServiceLogs_ServiceId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceLog_CreatorId",
                table: "ServiceLogs",
                newName: "IX_ServiceLogs_CreatorId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceLog_CourtClusterId",
                table: "ServiceLogs",
                newName: "IX_ServiceLogs_CourtClusterId");

            migrationBuilder.AlterColumn<int>(
                name: "ServiceId",
                table: "ServiceLogs",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LogType",
                table: "ServiceLogs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ServiceLogs",
                table: "ServiceLogs",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceLogs_CourtClusters_CourtClusterId",
                table: "ServiceLogs",
                column: "CourtClusterId",
                principalTable: "CourtClusters",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceLogs_Services_ServiceId",
                table: "ServiceLogs",
                column: "ServiceId",
                principalTable: "Services",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceLogs_Users_CreatorId",
                table: "ServiceLogs",
                column: "CreatorId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceLogs_CourtClusters_CourtClusterId",
                table: "ServiceLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceLogs_Services_ServiceId",
                table: "ServiceLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceLogs_Users_CreatorId",
                table: "ServiceLogs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ServiceLogs",
                table: "ServiceLogs");

            migrationBuilder.DropColumn(
                name: "LogType",
                table: "ServiceLogs");

            migrationBuilder.RenameTable(
                name: "ServiceLogs",
                newName: "ServiceLog");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceLogs_ServiceId",
                table: "ServiceLog",
                newName: "IX_ServiceLog_ServiceId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceLogs_CreatorId",
                table: "ServiceLog",
                newName: "IX_ServiceLog_CreatorId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceLogs_CourtClusterId",
                table: "ServiceLog",
                newName: "IX_ServiceLog_CourtClusterId");

            migrationBuilder.AlterColumn<int>(
                name: "ServiceId",
                table: "ServiceLog",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ServiceLog",
                table: "ServiceLog",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceLog_CourtClusters_CourtClusterId",
                table: "ServiceLog",
                column: "CourtClusterId",
                principalTable: "CourtClusters",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceLog_Services_ServiceId",
                table: "ServiceLog",
                column: "ServiceId",
                principalTable: "Services",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceLog_Users_CreatorId",
                table: "ServiceLog",
                column: "CreatorId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
