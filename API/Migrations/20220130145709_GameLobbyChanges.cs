using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    public partial class GameLobbyChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentPlayerId",
                table: "GameLobbies");

            migrationBuilder.AddColumn<string>(
                name: "CurrentPlayer",
                table: "GameLobbies",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentPlayer",
                table: "GameLobbies");

            migrationBuilder.AddColumn<int>(
                name: "CurrentPlayerId",
                table: "GameLobbies",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
