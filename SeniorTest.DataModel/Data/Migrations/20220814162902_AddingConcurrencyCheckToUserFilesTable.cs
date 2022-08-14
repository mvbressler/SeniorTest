using Microsoft.EntityFrameworkCore.Migrations;
using NuGet.Packaging.Signing;

#nullable disable

namespace SeniorTest.DataModel.Data.Migrations
{
    public partial class AddingConcurrencyCheckToUserFilesTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>("ConcurrencyCheck", "UserFiles", "Rowversion");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn("ConcurrencyCheck", "UserFiles");
        }
    }
}
