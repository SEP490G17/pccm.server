using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCourtCluster : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MyProperty",
                table: "CourtPrices",
                newName: "ToTime");

            migrationBuilder.RenameColumn(
                name: "Location",
                table: "CourtClusters",
                newName: "WardName");

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "CourtPrices",
                type: "varchar(150)",
                maxLength: 150,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<TimeOnly>(
                name: "FromTime",
                table: "CourtPrices",
                type: "TIME",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

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

            migrationBuilder.AddColumn<string>(
                name: "District",
                table: "CourtClusters",
                type: "varchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "DistrictName",
                table: "CourtClusters",
                type: "varchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Province",
                table: "CourtClusters",
                type: "varchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ProvinceName",
                table: "CourtClusters",
                type: "varchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Ward",
                table: "CourtClusters",
                type: "varchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "CourtPrices");

            migrationBuilder.DropColumn(
                name: "FromTime",
                table: "CourtPrices");

            migrationBuilder.DropColumn(
                name: "District",
                table: "CourtClusters");

            migrationBuilder.DropColumn(
                name: "DistrictName",
                table: "CourtClusters");

            migrationBuilder.DropColumn(
                name: "Province",
                table: "CourtClusters");

            migrationBuilder.DropColumn(
                name: "ProvinceName",
                table: "CourtClusters");

            migrationBuilder.DropColumn(
                name: "Ward",
                table: "CourtClusters");

            migrationBuilder.RenameColumn(
                name: "ToTime",
                table: "CourtPrices",
                newName: "MyProperty");

            migrationBuilder.RenameColumn(
                name: "WardName",
                table: "CourtClusters",
                newName: "Location");

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
        }
    }
}
