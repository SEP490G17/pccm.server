using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class LogType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Thêm cột LogType vào bảng ProductLogs
            migrationBuilder.AddColumn<int>(
                name: "LogType",
                table: "ProductLogs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            // Thêm cột LogType vào bảng BannerLogs
            migrationBuilder.AddColumn<int>(
                name: "LogType",
                table: "BannerLogs",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Xóa cột LogType từ bảng ProductLogs
            migrationBuilder.DropColumn(
                name: "LogType",
                table: "ProductLogs");

            // Xóa cột LogType từ bảng BannerLogs
            migrationBuilder.DropColumn(
                name: "LogType",
                table: "BannerLogs");
        }
    }
}
