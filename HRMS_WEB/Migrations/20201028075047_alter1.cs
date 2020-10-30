using Microsoft.EntityFrameworkCore.Migrations;

namespace HRMS_WEB.Migrations
{
    public partial class alter1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "c",
                table: "Users",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "c",
                table: "Users");
        }
    }
}
