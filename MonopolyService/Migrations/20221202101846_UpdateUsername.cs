using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MonopolyService.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUsername : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "MonopolyGames",
                newName: "UserName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "MonopolyGames",
                newName: "UserId");
        }
    }
}
