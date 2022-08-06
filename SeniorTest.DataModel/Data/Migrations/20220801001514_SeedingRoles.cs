using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SeniorTest.DataModel.Data.Migrations
{
    public partial class SeedingRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "23c396da-447a-491b-9a7a-f48113a1b746", "105022dc-1ed9-416c-a0b1-16c781094f59", "Admin", "ADMIN" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "f8390fe4-cbf5-46ed-834c-8503740ccf4b", "1909b973-7fbe-4c95-9d52-337dca3a7e01", "User", "USER" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "23c396da-447a-491b-9a7a-f48113a1b746");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f8390fe4-cbf5-46ed-834c-8503740ccf4b");
        }
    }
}
