using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EclipseService.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUsername : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "userId",
                table: "EclipseGames",
                newName: "UserName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "EclipseGames",
                newName: "userId");
        }
    }
}
