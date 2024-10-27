using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Courts_CourtId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_StaffDetails_StaffId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Users_UserId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Orders_OrderId",
                table: "OrderDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_StaffDetails_StaffId",
                table: "Orders");

            migrationBuilder.DropTable(
                name: "Shifts");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_StaffId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "OrderDate",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "StaffId",
                table: "Bookings");

            migrationBuilder.RenameColumn(
                name: "StartTime",
                table: "Orders",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "StaffId",
                table: "Orders",
                newName: "CreatedBy");

            migrationBuilder.RenameIndex(
                name: "IX_Orders_StaffId",
                table: "Orders",
                newName: "IX_Orders_CreatedBy");

            migrationBuilder.RenameColumn(
                name: "Price",
                table: "OrderDetails",
                newName: "TotalPrice");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Bookings",
                newName: "AppUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Bookings_UserId",
                table: "Bookings",
                newName: "IX_Bookings_AppUserId");

            migrationBuilder.AddColumn<DateTime>(
                name: "JoiningDate",
                table: "Users",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Services",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Services",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatorId",
                table: "Services",
                type: "varchar(255)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Services",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "Services",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "DeleterId",
                table: "Services",
                type: "varchar(255)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Services",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Services",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "UpdaterId",
                table: "Services",
                type: "varchar(255)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "Products",
                type: "decimal(10,2)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Products",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Products",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatorId",
                table: "Products",
                type: "varchar(255)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Products",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "Products",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "DeleterId",
                table: "Products",
                type: "varchar(255)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<decimal>(
                name: "ImportFee",
                table: "Products",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Products",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Products",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "UpdaterId",
                table: "Products",
                type: "varchar(255)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<float>(
                name: "Discount",
                table: "Orders",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PaymentMethod",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<float>(
                name: "Quantity",
                table: "OrderDetails",
                type: "float",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "OrderId",
                table: "OrderDetails",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "News",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatorId",
                table: "News",
                type: "varchar(255)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "News",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "News",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "DeleterId",
                table: "News",
                type: "varchar(255)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "News",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "News",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "UpdaterId",
                table: "News",
                type: "varchar(255)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "CourtId",
                table: "Bookings",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Bookings",
                type: "varchar(12)",
                maxLength: 12,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Banners",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatorId",
                table: "Banners",
                type: "varchar(255)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Banners",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "Banners",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "DeleterId",
                table: "Banners",
                type: "varchar(255)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Banners",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Banners",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "UpdaterId",
                table: "Banners",
                type: "varchar(255)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "BannerLog",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ImageUrl = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Status = table.Column<int>(type: "int", nullable: false),
                    BannerType = table.Column<int>(type: "int", nullable: false),
                    BannerInPage = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LinkUrl = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    StartDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    BannerId = table.Column<int>(type: "int", nullable: true),
                    CreatedBy = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatorId = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BannerLog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BannerLog_Banners_BannerId",
                        column: x => x.BannerId,
                        principalTable: "Banners",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BannerLog_Users_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "Users",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "NewsLog",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Thumbnail = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Content = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    StartTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Location = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Tags = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NewsBlogId = table.Column<int>(type: "int", nullable: true),
                    CreatedBy = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatorId = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsLog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NewsLog_News_NewsBlogId",
                        column: x => x.NewsBlogId,
                        principalTable: "News",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_NewsLog_Users_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "Users",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ProductLog",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CourtClusterId = table.Column<int>(type: "int", nullable: true),
                    CategoryId = table.Column<int>(type: "int", nullable: true),
                    ThumbnailUrl = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ProductName = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: true),
                    CreatedBy = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatorId = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductLog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductLog_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProductLog_CourtClusters_CourtClusterId",
                        column: x => x.CourtClusterId,
                        principalTable: "CourtClusters",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProductLog_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProductLog_Users_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "Users",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ServiceLog",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ServiceName = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Price = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    CourtClusterId = table.Column<int>(type: "int", nullable: true),
                    ServiceId = table.Column<int>(type: "int", nullable: true),
                    CreatedBy = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatorId = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceLog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceLog_CourtClusters_CourtClusterId",
                        column: x => x.CourtClusterId,
                        principalTable: "CourtClusters",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ServiceLog_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ServiceLog_Users_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "Users",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Services_CreatorId",
                table: "Services",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Services_DeleterId",
                table: "Services",
                column: "DeleterId");

            migrationBuilder.CreateIndex(
                name: "IX_Services_UpdaterId",
                table: "Services",
                column: "UpdaterId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CreatorId",
                table: "Products",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_DeleterId",
                table: "Products",
                column: "DeleterId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_UpdaterId",
                table: "Products",
                column: "UpdaterId");

            migrationBuilder.CreateIndex(
                name: "IX_News_CreatorId",
                table: "News",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_News_DeleterId",
                table: "News",
                column: "DeleterId");

            migrationBuilder.CreateIndex(
                name: "IX_News_UpdaterId",
                table: "News",
                column: "UpdaterId");

            migrationBuilder.CreateIndex(
                name: "IX_Courts_CourtClusterId",
                table: "Courts",
                column: "CourtClusterId");

            migrationBuilder.CreateIndex(
                name: "IX_Banners_CreatorId",
                table: "Banners",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Banners_DeleterId",
                table: "Banners",
                column: "DeleterId");

            migrationBuilder.CreateIndex(
                name: "IX_Banners_UpdaterId",
                table: "Banners",
                column: "UpdaterId");

            migrationBuilder.CreateIndex(
                name: "IX_BannerLog_BannerId",
                table: "BannerLog",
                column: "BannerId");

            migrationBuilder.CreateIndex(
                name: "IX_BannerLog_CreatorId",
                table: "BannerLog",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_NewsLog_CreatorId",
                table: "NewsLog",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_NewsLog_NewsBlogId",
                table: "NewsLog",
                column: "NewsBlogId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductLog_CategoryId",
                table: "ProductLog",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductLog_CourtClusterId",
                table: "ProductLog",
                column: "CourtClusterId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductLog_CreatorId",
                table: "ProductLog",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductLog_ProductId",
                table: "ProductLog",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceLog_CourtClusterId",
                table: "ServiceLog",
                column: "CourtClusterId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceLog_CreatorId",
                table: "ServiceLog",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceLog_ServiceId",
                table: "ServiceLog",
                column: "ServiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Banners_Users_CreatorId",
                table: "Banners",
                column: "CreatorId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Banners_Users_DeleterId",
                table: "Banners",
                column: "DeleterId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Banners_Users_UpdaterId",
                table: "Banners",
                column: "UpdaterId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Courts_CourtId",
                table: "Bookings",
                column: "CourtId",
                principalTable: "Courts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Users_AppUserId",
                table: "Bookings",
                column: "AppUserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Courts_CourtClusters_CourtClusterId",
                table: "Courts",
                column: "CourtClusterId",
                principalTable: "CourtClusters",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_News_Users_CreatorId",
                table: "News",
                column: "CreatorId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_News_Users_DeleterId",
                table: "News",
                column: "DeleterId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_News_Users_UpdaterId",
                table: "News",
                column: "UpdaterId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_Orders_OrderId",
                table: "OrderDetails",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_StaffDetails_CreatedBy",
                table: "Orders",
                column: "CreatedBy",
                principalTable: "StaffDetails",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Users_CreatorId",
                table: "Products",
                column: "CreatorId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Users_DeleterId",
                table: "Products",
                column: "DeleterId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Users_UpdaterId",
                table: "Products",
                column: "UpdaterId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Services_Users_CreatorId",
                table: "Services",
                column: "CreatorId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Services_Users_DeleterId",
                table: "Services",
                column: "DeleterId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Services_Users_UpdaterId",
                table: "Services",
                column: "UpdaterId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Banners_Users_CreatorId",
                table: "Banners");

            migrationBuilder.DropForeignKey(
                name: "FK_Banners_Users_DeleterId",
                table: "Banners");

            migrationBuilder.DropForeignKey(
                name: "FK_Banners_Users_UpdaterId",
                table: "Banners");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Courts_CourtId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Users_AppUserId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Courts_CourtClusters_CourtClusterId",
                table: "Courts");

            migrationBuilder.DropForeignKey(
                name: "FK_News_Users_CreatorId",
                table: "News");

            migrationBuilder.DropForeignKey(
                name: "FK_News_Users_DeleterId",
                table: "News");

            migrationBuilder.DropForeignKey(
                name: "FK_News_Users_UpdaterId",
                table: "News");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Orders_OrderId",
                table: "OrderDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_StaffDetails_CreatedBy",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Users_CreatorId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Users_DeleterId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Users_UpdaterId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Services_Users_CreatorId",
                table: "Services");

            migrationBuilder.DropForeignKey(
                name: "FK_Services_Users_DeleterId",
                table: "Services");

            migrationBuilder.DropForeignKey(
                name: "FK_Services_Users_UpdaterId",
                table: "Services");

            migrationBuilder.DropTable(
                name: "BannerLog");

            migrationBuilder.DropTable(
                name: "NewsLog");

            migrationBuilder.DropTable(
                name: "ProductLog");

            migrationBuilder.DropTable(
                name: "ServiceLog");

            migrationBuilder.DropIndex(
                name: "IX_Services_CreatorId",
                table: "Services");

            migrationBuilder.DropIndex(
                name: "IX_Services_DeleterId",
                table: "Services");

            migrationBuilder.DropIndex(
                name: "IX_Services_UpdaterId",
                table: "Services");

            migrationBuilder.DropIndex(
                name: "IX_Products_CreatorId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_DeleterId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_UpdaterId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_News_CreatorId",
                table: "News");

            migrationBuilder.DropIndex(
                name: "IX_News_DeleterId",
                table: "News");

            migrationBuilder.DropIndex(
                name: "IX_News_UpdaterId",
                table: "News");

            migrationBuilder.DropIndex(
                name: "IX_Courts_CourtClusterId",
                table: "Courts");

            migrationBuilder.DropIndex(
                name: "IX_Banners_CreatorId",
                table: "Banners");

            migrationBuilder.DropIndex(
                name: "IX_Banners_DeleterId",
                table: "Banners");

            migrationBuilder.DropIndex(
                name: "IX_Banners_UpdaterId",
                table: "Banners");

            migrationBuilder.DropColumn(
                name: "JoiningDate",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "DeleterId",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "UpdaterId",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "DeleterId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ImportFee",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "UpdaterId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Discount",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "News");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "News");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "News");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "News");

            migrationBuilder.DropColumn(
                name: "DeleterId",
                table: "News");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "News");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "News");

            migrationBuilder.DropColumn(
                name: "UpdaterId",
                table: "News");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Banners");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "Banners");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Banners");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Banners");

            migrationBuilder.DropColumn(
                name: "DeleterId",
                table: "Banners");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Banners");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Banners");

            migrationBuilder.DropColumn(
                name: "UpdaterId",
                table: "Banners");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "Orders",
                newName: "StaffId");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Orders",
                newName: "StartTime");

            migrationBuilder.RenameIndex(
                name: "IX_Orders_CreatedBy",
                table: "Orders",
                newName: "IX_Orders_StaffId");

            migrationBuilder.RenameColumn(
                name: "TotalPrice",
                table: "OrderDetails",
                newName: "Price");

            migrationBuilder.RenameColumn(
                name: "AppUserId",
                table: "Bookings",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Bookings_AppUserId",
                table: "Bookings",
                newName: "IX_Bookings_UserId");

            migrationBuilder.AlterColumn<int>(
                name: "Quantity",
                table: "Products",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndTime",
                table: "Orders",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "OrderDate",
                table: "Orders",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<int>(
                name: "Quantity",
                table: "OrderDetails",
                type: "int",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "float");

            migrationBuilder.AlterColumn<int>(
                name: "OrderId",
                table: "OrderDetails",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "CourtId",
                table: "Bookings",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StaffId",
                table: "Bookings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Shifts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    EndTime = table.Column<TimeSpan>(type: "time(6)", nullable: false),
                    ShiftName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    StartTime = table.Column<TimeSpan>(type: "time(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shifts", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_StaffId",
                table: "Bookings",
                column: "StaffId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Courts_CourtId",
                table: "Bookings",
                column: "CourtId",
                principalTable: "Courts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_StaffDetails_StaffId",
                table: "Bookings",
                column: "StaffId",
                principalTable: "StaffDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Users_UserId",
                table: "Bookings",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_Orders_OrderId",
                table: "OrderDetails",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_StaffDetails_StaffId",
                table: "Orders",
                column: "StaffId",
                principalTable: "StaffDetails",
                principalColumn: "Id");
        }
    }
}
