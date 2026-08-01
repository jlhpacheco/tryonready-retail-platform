using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TryOnReady.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddBoutiqueApplicationEmailGuardrail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_boutique_applications_Email",
                table: "boutique_applications",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_boutique_applications_Email",
                table: "boutique_applications");
        }
    }
}
