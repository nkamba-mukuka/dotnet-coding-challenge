using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Levelbuild.CodingChallenge.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUserDisplayNameUniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Users_DisplayName",
                table: "Users",
                column: "DisplayName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_DisplayName",
                table: "Users");
        }
    }
}
