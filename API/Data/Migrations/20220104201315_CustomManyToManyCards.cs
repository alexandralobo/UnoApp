using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Data.Migrations
{
    public partial class CustomManyToManyCards : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CardConnection",
                columns: table => new
                {
                    CardsCardId = table.Column<int>(type: "INTEGER", nullable: false),
                    ConnectionsConnectionId = table.Column<string>(type: "TEXT", nullable: false)
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
        }
    }
}
