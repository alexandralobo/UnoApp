﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    public partial class AddedOrderPropertyToGameLobby : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "order",
                table: "GameLobbies",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "order",
                table: "GameLobbies");
        }
    }
}
