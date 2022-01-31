using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    public partial class AddingAgainToCards : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cards_Connections_ConnectionId",
                table: "Cards");

            migrationBuilder.DropForeignKey(
                name: "FK_Cards_GameLobbies_GameLobbyId",
                table: "Cards");

            migrationBuilder.DropForeignKey(
                name: "FK_Cards_GameLobbies_GameLobbyId1",
                table: "Cards");

            migrationBuilder.DropIndex(
                name: "IX_Cards_ConnectionId",
                table: "Cards");

            migrationBuilder.DropIndex(
                name: "IX_Cards_GameLobbyId",
                table: "Cards");

            migrationBuilder.DropIndex(
                name: "IX_Cards_GameLobbyId1",
                table: "Cards");

            migrationBuilder.DropColumn(
                name: "ConnectionId",
                table: "Cards");

            migrationBuilder.DropColumn(
                name: "GameLobbyId",
                table: "Cards");

            migrationBuilder.DropColumn(
                name: "GameLobbyId1",
                table: "Cards");

            migrationBuilder.CreateTable(
                name: "CardConnection",
                columns: table => new
                {
                    CardsCardId = table.Column<int>(type: "INTEGER", nullable: false),
                    ConnectionsConnectionId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardConnection", x => new { x.CardsCardId, x.ConnectionsConnectionId });
                    table.ForeignKey(
                        name: "FK_CardConnection_Cards_CardsCardId",
                        column: x => x.CardsCardId,
                        principalTable: "Cards",
                        principalColumn: "CardId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CardConnection_Connections_ConnectionsConnectionId",
                        column: x => x.ConnectionsConnectionId,
                        principalTable: "Connections",
                        principalColumn: "ConnectionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CardGameLobbyDrawable",
                columns: table => new
                {
                    CardId = table.Column<int>(type: "INTEGER", nullable: false),
                    GameLobbyId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardGameLobbyDrawable", x => new { x.CardId, x.GameLobbyId });
                    table.ForeignKey(
                        name: "FK_CardGameLobbyDrawable_Cards_CardId",
                        column: x => x.CardId,
                        principalTable: "Cards",
                        principalColumn: "CardId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CardGameLobbyDrawable_GameLobbies_GameLobbyId",
                        column: x => x.GameLobbyId,
                        principalTable: "GameLobbies",
                        principalColumn: "GameLobbyId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CardGameLobbyInPot",
                columns: table => new
                {
                    CardId = table.Column<int>(type: "INTEGER", nullable: false),
                    GameLobbyId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardGameLobbyInPot", x => new { x.CardId, x.GameLobbyId });
                    table.ForeignKey(
                        name: "FK_CardGameLobbyInPot_Cards_CardId",
                        column: x => x.CardId,
                        principalTable: "Cards",
                        principalColumn: "CardId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CardGameLobbyInPot_GameLobbies_GameLobbyId",
                        column: x => x.GameLobbyId,
                        principalTable: "GameLobbies",
                        principalColumn: "GameLobbyId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CardConnection_ConnectionsConnectionId",
                table: "CardConnection",
                column: "ConnectionsConnectionId");

            migrationBuilder.CreateIndex(
                name: "IX_CardGameLobbyDrawable_GameLobbyId",
                table: "CardGameLobbyDrawable",
                column: "GameLobbyId");

            migrationBuilder.CreateIndex(
                name: "IX_CardGameLobbyInPot_GameLobbyId",
                table: "CardGameLobbyInPot",
                column: "GameLobbyId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CardConnection");

            migrationBuilder.DropTable(
                name: "CardGameLobbyDrawable");

            migrationBuilder.DropTable(
                name: "CardGameLobbyInPot");

            migrationBuilder.AddColumn<int>(
                name: "ConnectionId",
                table: "Cards",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GameLobbyId",
                table: "Cards",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GameLobbyId1",
                table: "Cards",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cards_ConnectionId",
                table: "Cards",
                column: "ConnectionId");

            migrationBuilder.CreateIndex(
                name: "IX_Cards_GameLobbyId",
                table: "Cards",
                column: "GameLobbyId");

            migrationBuilder.CreateIndex(
                name: "IX_Cards_GameLobbyId1",
                table: "Cards",
                column: "GameLobbyId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Cards_Connections_ConnectionId",
                table: "Cards",
                column: "ConnectionId",
                principalTable: "Connections",
                principalColumn: "ConnectionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cards_GameLobbies_GameLobbyId",
                table: "Cards",
                column: "GameLobbyId",
                principalTable: "GameLobbies",
                principalColumn: "GameLobbyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cards_GameLobbies_GameLobbyId1",
                table: "Cards",
                column: "GameLobbyId1",
                principalTable: "GameLobbies",
                principalColumn: "GameLobbyId");
        }
    }
}
