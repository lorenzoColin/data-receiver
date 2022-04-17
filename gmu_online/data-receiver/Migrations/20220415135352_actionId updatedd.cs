using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace data_receiver.Migrations
{
    public partial class actionIdupdatedd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customer_action_actionId",
                schema: "Identity",
                table: "Customer");

            migrationBuilder.AlterColumn<int>(
                name: "actionId",
                schema: "Identity",
                table: "Customer",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Customer_action_actionId",
                schema: "Identity",
                table: "Customer",
                column: "actionId",
                principalSchema: "Identity",
                principalTable: "action",
                principalColumn: "id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customer_action_actionId",
                schema: "Identity",
                table: "Customer");

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
    }
}
