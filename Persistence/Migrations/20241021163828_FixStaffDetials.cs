using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixStaffDetials : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StaffDetails_StaffPositions_PositionId",
                table: "StaffDetails");

            migrationBuilder.AlterColumn<int>(
                name: "PositionId",
                table: "StaffDetails",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_StaffDetails_StaffPositions_PositionId",
                table: "StaffDetails",
                column: "PositionId",
                principalTable: "StaffPositions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StaffDetails_StaffPositions_PositionId",
                table: "StaffDetails");

            migrationBuilder.AlterColumn<int>(
                name: "PositionId",
                table: "StaffDetails",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_StaffDetails_StaffPositions_PositionId",
                table: "StaffDetails",
                column: "PositionId",
                principalTable: "StaffPositions",
                principalColumn: "Id");
        }
    }
}
