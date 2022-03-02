using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace data_receiver.Migrations
{
    public partial class newmigrationwithdevicetype : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_customer_action_actionid",
                schema: "Identity",
                table: "customer");

            migrationBuilder.DropForeignKey(
                name: "FK_UserCustomer_customer_customerId",
                schema: "Identity",
                table: "UserCustomer");

            migrationBuilder.DropPrimaryKey(
                name: "PK_customer",
                schema: "Identity",
                table: "customer");

            migrationBuilder.DropPrimaryKey(
                name: "PK_action",
                schema: "Identity",
                table: "action");

            migrationBuilder.DropColumn(
                name: "name",
                schema: "Identity",
                table: "action");

            migrationBuilder.RenameTable(
                name: "customer",
                schema: "Identity",
                newName: "Customer",
                newSchema: "Identity");

            migrationBuilder.RenameTable(
                name: "action",
                schema: "Identity",
                newName: "Action",
                newSchema: "Identity");

            migrationBuilder.RenameIndex(
                name: "IX_customer_actionid",
                schema: "Identity",
                table: "Customer",
                newName: "IX_Customer_actionid");

            migrationBuilder.AddColumn<int>(
                name: "type",
                schema: "Identity",
                table: "Action",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Customer",
                schema: "Identity",
                table: "Customer",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Action",
                schema: "Identity",
                table: "Action",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_Customer_Action_actionid",
                schema: "Identity",
                table: "Customer",
                column: "actionid",
                principalSchema: "Identity",
                principalTable: "Action",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserCustomer_Customer_customerId",
                schema: "Identity",
                table: "UserCustomer",
                column: "customerId",
                principalSchema: "Identity",
                principalTable: "Customer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customer_Action_actionid",
                schema: "Identity",
                table: "Customer");

            migrationBuilder.DropForeignKey(
                name: "FK_UserCustomer_Customer_customerId",
                schema: "Identity",
                table: "UserCustomer");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Customer",
                schema: "Identity",
                table: "Customer");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Action",
                schema: "Identity",
                table: "Action");

            migrationBuilder.DropColumn(
                name: "type",
                schema: "Identity",
                table: "Action");

            migrationBuilder.RenameTable(
                name: "Customer",
                schema: "Identity",
                newName: "customer",
                newSchema: "Identity");

            migrationBuilder.RenameTable(
                name: "Action",
                schema: "Identity",
                newName: "action",
                newSchema: "Identity");

            migrationBuilder.RenameIndex(
                name: "IX_Customer_actionid",
                schema: "Identity",
                table: "customer",
                newName: "IX_customer_actionid");

            migrationBuilder.AddColumn<string>(
                name: "name",
                schema: "Identity",
                table: "action",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_customer",
                schema: "Identity",
                table: "customer",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_action",
                schema: "Identity",
                table: "action",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_customer_action_actionid",
                schema: "Identity",
                table: "customer",
                column: "actionid",
                principalSchema: "Identity",
                principalTable: "action",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserCustomer_customer_customerId",
                schema: "Identity",
                table: "UserCustomer",
                column: "customerId",
                principalSchema: "Identity",
                principalTable: "customer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
