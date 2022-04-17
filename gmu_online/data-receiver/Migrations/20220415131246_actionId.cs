using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace data_receiver.Migrations
{
    public partial class actionId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Actionid",
                schema: "Identity",
                table: "Customer",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customer_Actionid",
                schema: "Identity",
                table: "Customer",
                column: "Actionid");

            migrationBuilder.AddForeignKey(
                name: "FK_Customer_action_Actionid",
                schema: "Identity",
                table: "Customer",
                column: "Actionid",
                principalSchema: "Identity",
                principalTable: "action",
                principalColumn: "id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customer_action_Actionid",
                schema: "Identity",
                table: "Customer");

            migrationBuilder.DropIndex(
                name: "IX_Customer_Actionid",
                schema: "Identity",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "Actionid",
                schema: "Identity",
                table: "Customer");
        }
    }
}
