using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddProductLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductLog_Categories_CategoryId",
                table: "ProductLog");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductLog_CourtClusters_CourtClusterId",
                table: "ProductLog");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductLog_Products_ProductId",
                table: "ProductLog");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductLog_Users_CreatorId",
                table: "ProductLog");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductLog",
                table: "ProductLog");

            migrationBuilder.RenameTable(
                name: "ProductLog",
                newName: "ProductLogs");

            migrationBuilder.RenameIndex(
                name: "IX_ProductLog_ProductId",
                table: "ProductLogs",
                newName: "IX_ProductLogs_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductLog_CreatorId",
                table: "ProductLogs",
                newName: "IX_ProductLogs_CreatorId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductLog_CourtClusterId",
                table: "ProductLogs",
                newName: "IX_ProductLogs_CourtClusterId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductLog_CategoryId",
                table: "ProductLogs",
                newName: "IX_ProductLogs_CategoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductLogs",
                table: "ProductLogs",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductLogs_Categories_CategoryId",
                table: "ProductLogs",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductLogs_CourtClusters_CourtClusterId",
                table: "ProductLogs",
                column: "CourtClusterId",
                principalTable: "CourtClusters",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductLogs_Products_ProductId",
                table: "ProductLogs",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductLogs_Users_CreatorId",
                table: "ProductLogs",
                column: "CreatorId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductLogs_Categories_CategoryId",
                table: "ProductLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductLogs_CourtClusters_CourtClusterId",
                table: "ProductLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductLogs_Products_ProductId",
                table: "ProductLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductLogs_Users_CreatorId",
                table: "ProductLogs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductLogs",
                table: "ProductLogs");

            migrationBuilder.RenameTable(
                name: "ProductLogs",
                newName: "ProductLog");

            migrationBuilder.RenameIndex(
                name: "IX_ProductLogs_ProductId",
                table: "ProductLog",
                newName: "IX_ProductLog_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductLogs_CreatorId",
                table: "ProductLog",
                newName: "IX_ProductLog_CreatorId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductLogs_CourtClusterId",
                table: "ProductLog",
                newName: "IX_ProductLog_CourtClusterId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductLogs_CategoryId",
                table: "ProductLog",
                newName: "IX_ProductLog_CategoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductLog",
                table: "ProductLog",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductLog_Categories_CategoryId",
                table: "ProductLog",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductLog_CourtClusters_CourtClusterId",
                table: "ProductLog",
                column: "CourtClusterId",
                principalTable: "CourtClusters",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductLog_Products_ProductId",
                table: "ProductLog",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductLog_Users_CreatorId",
                table: "ProductLog",
                column: "CreatorId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
