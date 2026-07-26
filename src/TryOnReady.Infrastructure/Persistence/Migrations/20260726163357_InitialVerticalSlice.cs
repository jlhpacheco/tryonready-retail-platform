using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TryOnReady.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialVerticalSlice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "boutique_applications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BoutiqueName = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    OwnerName = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    Email = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: false),
                    EmployeeCount = table.Column<int>(type: "integer", nullable: false),
                    PrimarySalesChannel = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Website = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    SubmittedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DecidedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_boutique_applications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "products",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BoutiqueApplicationId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: false),
                    Sku = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Category = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Brand = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Color = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Material = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    SizeRange = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    Price = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true),
                    Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    ProductUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    ReadinessPassed = table.Column<bool>(type: "boolean", nullable: false),
                    GarmentAssetId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    FileName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    MediaType = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    ByteLength = table.Column<long>(type: "bigint", nullable: false),
                    PixelWidth = table.Column<int>(type: "integer", nullable: false),
                    PixelHeight = table.Column<int>(type: "integer", nullable: false),
                    SubmittedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DecisionNotes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    DecidedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_products_boutique_applications_BoutiqueApplicationId",
                        column: x => x.BoutiqueApplicationId,
                        principalTable: "boutique_applications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "try_on_jobs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Message = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    RequestFingerprint = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    DuplicateRequestCount = table.Column<int>(type: "integer", nullable: false),
                    PersonAssetId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    PersonMediaType = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    ConsentVersion = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    ConsentAcceptedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ProviderReference = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ProviderErrorCode = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: true),
                    ResultAssetId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    ApiUnitsReserved = table.Column<int>(type: "integer", nullable: false),
                    ApiUnitsConsumed = table.Column<int>(type: "integer", nullable: false),
                    AttemptCount = table.Column<int>(type: "integer", nullable: false),
                    NextAttemptAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CompletedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_try_on_jobs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_try_on_jobs_products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_boutique_applications_SubmittedAtUtc",
                table: "boutique_applications",
                column: "SubmittedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_products_BoutiqueApplicationId_Sku",
                table: "products",
                columns: new[] { "BoutiqueApplicationId", "Sku" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_products_Status",
                table: "products",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_try_on_jobs_ProductId",
                table: "try_on_jobs",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_try_on_jobs_RequestFingerprint",
                table: "try_on_jobs",
                column: "RequestFingerprint",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_try_on_jobs_Status_NextAttemptAtUtc",
                table: "try_on_jobs",
                columns: new[] { "Status", "NextAttemptAtUtc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "try_on_jobs");

            migrationBuilder.DropTable(
                name: "products");

            migrationBuilder.DropTable(
                name: "boutique_applications");
        }
    }
}
