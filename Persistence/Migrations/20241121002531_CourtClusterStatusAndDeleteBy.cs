using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CourtClusterStatusAndDeleteBy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeleteAt",
                table: "CourtClusters",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeleteById",
                table: "CourtClusters",
                type: "varchar(255)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "IsVisible",
                table: "CourtClusters",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_CourtClusters_DeleteById",
                table: "CourtClusters",
                column: "DeleteById");

            migrationBuilder.AddForeignKey(
                name: "FK_CourtClusters_Users_DeleteById",
                table: "CourtClusters",
                column: "DeleteById",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourtClusters_Users_DeleteById",
                table: "CourtClusters");

            migrationBuilder.DropIndex(
                name: "IX_CourtClusters_DeleteById",
                table: "CourtClusters");

            migrationBuilder.DropColumn(
                name: "DeleteAt",
                table: "CourtClusters");

            migrationBuilder.DropColumn(
                name: "DeleteById",
                table: "CourtClusters");

            migrationBuilder.DropColumn(
                name: "IsVisible",
                table: "CourtClusters");
        }
    }
}
