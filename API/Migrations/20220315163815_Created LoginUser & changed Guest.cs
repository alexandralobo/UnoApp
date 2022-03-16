using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    public partial class CreatedLoginUserchangedGuest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_GuestId",
                table: "AspNetUserRoles");

            migrationBuilder.RenameColumn(
                name: "GuestId",
                table: "AspNetUserRoles",
                newName: "LoginUserId");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUserRoles_GuestId",
                table: "AspNetUserRoles",
                newName: "IX_AspNetUserRoles_LoginUserId");

            migrationBuilder.CreateTable(
                name: "Guests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserName = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Guests", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_LoginUserId",
                table: "AspNetUserRoles",
                column: "LoginUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_LoginUserId",
                table: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "Guests");

            migrationBuilder.RenameColumn(
                name: "LoginUserId",
                table: "AspNetUserRoles",
                newName: "GuestId");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUserRoles_LoginUserId",
                table: "AspNetUserRoles",
                newName: "IX_AspNetUserRoles_GuestId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_GuestId",
                table: "AspNetUserRoles",
                column: "GuestId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
