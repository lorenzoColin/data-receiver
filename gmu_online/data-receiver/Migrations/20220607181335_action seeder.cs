using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace data_receiver.Migrations
{
    public partial class actionseeder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "Identity",
                table: "action",
                columns: new[] { "id", "actionName", "description" },
                values: new object[,]
                {
                    { 1, "Currentbudget", "example: At 50% of the month you want to email an update with the status of the budget." },
                    { 2, "Latest_videocall", "example: after 3 months I want to receive an email after the last video call" },
                    { 3, "Latest_contact", "example: after 3 months I want to receive an email after the last contact call" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "action",
                keyColumn: "id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "action",
                keyColumn: "id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "action",
                keyColumn: "id",
                keyValue: 3);
        }
    }
}
