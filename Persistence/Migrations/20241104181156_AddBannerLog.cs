using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddBannerLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BannerLog_Banners_BannerId",
                table: "BannerLog");

            migrationBuilder.DropForeignKey(
                name: "FK_BannerLog_Users_CreatorId",
                table: "BannerLog");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BannerLog",
                table: "BannerLog");

            migrationBuilder.RenameTable(
                name: "BannerLog",
                newName: "BannerLogs");

            migrationBuilder.RenameIndex(
                name: "IX_BannerLog_CreatorId",
                table: "BannerLogs",
                newName: "IX_BannerLogs_CreatorId");

            migrationBuilder.RenameIndex(
                name: "IX_BannerLog_BannerId",
                table: "BannerLogs",
                newName: "IX_BannerLogs_BannerId");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "CourtClusters",
                type: "Text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldMaxLength: 255,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "BannerId",
                table: "BannerLogs",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_BannerLogs",
                table: "BannerLogs",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BannerLogs_Banners_BannerId",
                table: "BannerLogs",
                column: "BannerId",
                principalTable: "Banners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BannerLogs_Users_CreatorId",
                table: "BannerLogs",
                column: "CreatorId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BannerLogs_Banners_BannerId",
                table: "BannerLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_BannerLogs_Users_CreatorId",
                table: "BannerLogs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BannerLogs",
                table: "BannerLogs");

            migrationBuilder.RenameTable(
                name: "BannerLogs",
                newName: "BannerLog");

            migrationBuilder.RenameIndex(
                name: "IX_BannerLogs_CreatorId",
                table: "BannerLog",
                newName: "IX_BannerLog_CreatorId");

            migrationBuilder.RenameIndex(
                name: "IX_BannerLogs_BannerId",
                table: "BannerLog",
                newName: "IX_BannerLog_BannerId");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "CourtClusters",
                type: "varchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "Text",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "BannerId",
                table: "BannerLog",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BannerLog",
                table: "BannerLog",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BannerLog_Banners_BannerId",
                table: "BannerLog",
                column: "BannerId",
                principalTable: "Banners",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BannerLog_Users_CreatorId",
                table: "BannerLog",
                column: "CreatorId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
