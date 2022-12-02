using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EclipseService.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EclipseGames",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DateOfGame = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Town = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WinningScore = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EclipseGames", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EclipseGames");
        }
    }
}
