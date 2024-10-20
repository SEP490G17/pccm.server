using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class StaffPositionForKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PositionId",
                table: "StaffDetails",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StaffDetails_PositionId",
                table: "StaffDetails",
                column: "PositionId");

            migrationBuilder.AddForeignKey(
                name: "FK_StaffDetails_StaffPositions_PositionId",
                table: "StaffDetails",
                column: "PositionId",
                principalTable: "StaffPositions",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StaffDetails_StaffPositions_PositionId",
                table: "StaffDetails");

            migrationBuilder.DropIndex(
                name: "IX_StaffDetails_PositionId",
                table: "StaffDetails");

            migrationBuilder.DropColumn(
                name: "PositionId",
                table: "StaffDetails");
        }
    }
}
