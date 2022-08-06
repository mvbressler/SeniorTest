using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SeniorTest.DataModel.Data.Migrations
{
    public partial class CreateUserFilesTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserFiles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Path = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Filename = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    IsZipped = table.Column<bool>(type: "bit", nullable: true, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFiles", x => new { x.UserId, x.Path, x.Filename });
                    table.ForeignKey(
                        name: "FK_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserFiles"
                );
        }
    }
}
