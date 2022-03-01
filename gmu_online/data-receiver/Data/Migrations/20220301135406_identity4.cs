using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace data_receiver.Data.Migrations
{
    public partial class identity4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                schema: "Identity",
                table: "Teams",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Teams_UserId",
                schema: "Identity",
                table: "Teams",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Teams_User_UserId",
                schema: "Identity",
                table: "Teams",
                column: "UserId",
                principalSchema: "Identity",
                principalTable: "User",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Teams_User_UserId",
                schema: "Identity",
                table: "Teams");

            migrationBuilder.DropIndex(
                name: "IX_Teams_UserId",
                schema: "Identity",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "UserId",
                schema: "Identity",
                table: "Teams");
        }
    }
}
