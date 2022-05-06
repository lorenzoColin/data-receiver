using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace data_receiver.Migrations
{
    public partial class addtriggerstoacustomer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "name",
                schema: "Identity",
                table: "action",
                newName: "description");

            migrationBuilder.AddColumn<int>(
                name: "value",
                schema: "Identity",
                table: "UserCustomerAction",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "value",
                schema: "Identity",
                table: "UserCustomerAction");

            migrationBuilder.RenameColumn(
                name: "description",
                schema: "Identity",
                table: "action",
                newName: "name");
        }
    }
}
