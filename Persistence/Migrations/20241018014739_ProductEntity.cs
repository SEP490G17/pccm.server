using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ProductEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                table: "Services",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CourtClusterId",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CourtClusterId",
                table: "CourtClusters",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Services_ProductId",
                table: "Services",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CourtClusterId",
                table: "Products",
                column: "CourtClusterId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_ProductId",
                table: "Products",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_CourtClusters_CourtClusterId",
                table: "Products",
                column: "CourtClusterId",
                principalTable: "CourtClusters",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Products_ProductId",
                table: "Products",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Services_Products_ProductId",
                table: "Services",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_CourtClusters_CourtClusterId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Products_ProductId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Services_Products_ProductId",
                table: "Services");

            migrationBuilder.DropIndex(
                name: "IX_Services_ProductId",
                table: "Services");

            migrationBuilder.DropIndex(
                name: "IX_Products_CourtClusterId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_ProductId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "CourtClusterId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "CourtClusterId",
                table: "CourtClusters");
        }
    }
}
