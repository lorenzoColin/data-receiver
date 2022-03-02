using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace data_receiver.Migrations
{
    public partial class newproppertyinsideactiontable2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customer_Action_actionid",
                schema: "Identity",
                table: "Customer");

            migrationBuilder.AlterColumn<string>(
                name: "actionid",
                schema: "Identity",
                table: "Customer",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_Customer_Action_actionid",
                schema: "Identity",
                table: "Customer",
                column: "actionid",
                principalSchema: "Identity",
                principalTable: "Action",
                principalColumn: "id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customer_Action_actionid",
                schema: "Identity",
                table: "Customer");

            migrationBuilder.AlterColumn<string>(
                name: "actionid",
                schema: "Identity",
                table: "Customer",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Customer_Action_actionid",
                schema: "Identity",
                table: "Customer",
                column: "actionid",
                principalSchema: "Identity",
                principalTable: "Action",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
