using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class NewsLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NewsLog_News_NewsBlogId",
                table: "NewsLog");

            migrationBuilder.DropForeignKey(
                name: "FK_NewsLog_Users_CreatorId",
                table: "NewsLog");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NewsLog",
                table: "NewsLog");

            migrationBuilder.RenameTable(
                name: "NewsLog",
                newName: "NewsLogs");

            migrationBuilder.RenameIndex(
                name: "IX_NewsLog_NewsBlogId",
                table: "NewsLogs",
                newName: "IX_NewsLogs_NewsBlogId");

            migrationBuilder.RenameIndex(
                name: "IX_NewsLog_CreatorId",
                table: "NewsLogs",
                newName: "IX_NewsLogs_CreatorId");

            migrationBuilder.AlterColumn<int>(
                name: "NewsBlogId",
                table: "NewsLogs",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LogType",
                table: "NewsLogs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_NewsLogs",
                table: "NewsLogs",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_NewsLogs_News_NewsBlogId",
                table: "NewsLogs",
                column: "NewsBlogId",
                principalTable: "News",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_NewsLogs_Users_CreatorId",
                table: "NewsLogs",
                column: "CreatorId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NewsLogs_News_NewsBlogId",
                table: "NewsLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_NewsLogs_Users_CreatorId",
                table: "NewsLogs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NewsLogs",
                table: "NewsLogs");

            migrationBuilder.DropColumn(
                name: "LogType",
                table: "NewsLogs");

            migrationBuilder.RenameTable(
                name: "NewsLogs",
                newName: "NewsLog");

            migrationBuilder.RenameIndex(
                name: "IX_NewsLogs_NewsBlogId",
                table: "NewsLog",
                newName: "IX_NewsLog_NewsBlogId");

            migrationBuilder.RenameIndex(
                name: "IX_NewsLogs_CreatorId",
                table: "NewsLog",
                newName: "IX_NewsLog_CreatorId");

            migrationBuilder.AlterColumn<int>(
                name: "NewsBlogId",
                table: "NewsLog",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddPrimaryKey(
                name: "PK_NewsLog",
                table: "NewsLog",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_NewsLog_News_NewsBlogId",
                table: "NewsLog",
                column: "NewsBlogId",
                principalTable: "News",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_NewsLog_Users_CreatorId",
                table: "NewsLog",
                column: "CreatorId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
