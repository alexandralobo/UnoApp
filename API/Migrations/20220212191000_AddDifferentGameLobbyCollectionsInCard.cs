using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    public partial class AddDifferentGameLobbyCollectionsInCard : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CardGameLobbyDrawable");

            migrationBuilder.DropTable(
                name: "CardGameLobbyInPot");

            migrationBuilder.CreateTable(
                name: "CardGameLobby",
                columns: table => new
                {
                    CardPotCardId = table.Column<int>(type: "INTEGER", nullable: false),
                    GameLobbyPotsGameLobbyId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardGameLobby", x => new { x.CardPotCardId, x.GameLobbyPotsGameLobbyId });
                    table.ForeignKey(
                        name: "FK_CardGameLobby_Cards_CardPotCardId",
                        column: x => x.CardPotCardId,
                        principalTable: "Cards",
                        principalColumn: "CardId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CardGameLobby_GameLobbies_GameLobbyPotsGameLobbyId",
                        column: x => x.GameLobbyPotsGameLobbyId,
                        principalTable: "GameLobbies",
                        principalColumn: "GameLobbyId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CardGameLobby1",
                columns: table => new
                {
                    DrawableCardsCardId = table.Column<int>(type: "INTEGER", nullable: false),
                    GameLobbyDrawablesGameLobbyId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardGameLobby1", x => new { x.DrawableCardsCardId, x.GameLobbyDrawablesGameLobbyId });
                    table.ForeignKey(
                        name: "FK_CardGameLobby1_Cards_DrawableCardsCardId",
                        column: x => x.DrawableCardsCardId,
                        principalTable: "Cards",
                        principalColumn: "CardId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CardGameLobby1_GameLobbies_GameLobbyDrawablesGameLobbyId",
                        column: x => x.GameLobbyDrawablesGameLobbyId,
                        principalTable: "GameLobbies",
                        principalColumn: "GameLobbyId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CardGameLobby_GameLobbyPotsGameLobbyId",
                table: "CardGameLobby",
                column: "GameLobbyPotsGameLobbyId");

            migrationBuilder.CreateIndex(
                name: "IX_CardGameLobby1_GameLobbyDrawablesGameLobbyId",
                table: "CardGameLobby1",
                column: "GameLobbyDrawablesGameLobbyId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CardGameLobby");

            migrationBuilder.DropTable(
                name: "CardGameLobby1");

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
                name: "IX_CardGameLobbyDrawable_GameLobbyId",
                table: "CardGameLobbyDrawable",
                column: "GameLobbyId");

            migrationBuilder.CreateIndex(
                name: "IX_CardGameLobbyInPot_GameLobbyId",
                table: "CardGameLobbyInPot",
                column: "GameLobbyId");
        }
    }
}
