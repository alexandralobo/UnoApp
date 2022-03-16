using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    public partial class FixedUpperCaseAttributeName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "password",
                table: "GameLobbies",
                newName: "Password");

            migrationBuilder.RenameColumn(
                name: "order",
                table: "GameLobbies",
                newName: "Order");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Password",
                table: "GameLobbies",
                newName: "password");

            migrationBuilder.RenameColumn(
                name: "Order",
                table: "GameLobbies",
                newName: "order");
        }
    }
}
