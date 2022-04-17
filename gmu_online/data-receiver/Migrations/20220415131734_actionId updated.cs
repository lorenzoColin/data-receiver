using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace data_receiver.Migrations
{
    public partial class actionIdupdated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customer_action_Actionid",
                schema: "Identity",
                table: "Customer");

            migrationBuilder.RenameColumn(
                name: "Actionid",
                schema: "Identity",
                table: "Customer",
                newName: "actionId");

            migrationBuilder.RenameIndex(
                name: "IX_Customer_Actionid",
                schema: "Identity",
                table: "Customer",
                newName: "IX_Customer_actionId");

            migrationBuilder.AlterColumn<int>(
                name: "actionId",
                schema: "Identity",
                table: "Customer",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Customer_action_actionId",
                schema: "Identity",
                table: "Customer",
                column: "actionId",
                principalSchema: "Identity",
                principalTable: "action",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customer_action_actionId",
                schema: "Identity",
                table: "Customer");

            migrationBuilder.RenameColumn(
                name: "actionId",
                schema: "Identity",
                table: "Customer",
                newName: "Actionid");

            migrationBuilder.RenameIndex(
                name: "IX_Customer_actionId",
                schema: "Identity",
                table: "Customer",
                newName: "IX_Customer_Actionid");

            migrationBuilder.AlterColumn<int>(
                name: "Actionid",
                schema: "Identity",
                table: "Customer",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Customer_action_Actionid",
                schema: "Identity",
                table: "Customer",
                column: "Actionid",
                principalSchema: "Identity",
                principalTable: "action",
                principalColumn: "id");
        }
    }
}
