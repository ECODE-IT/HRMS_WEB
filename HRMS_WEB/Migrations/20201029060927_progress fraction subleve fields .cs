using Microsoft.EntityFrameworkCore.Migrations;

namespace HRMS_WEB.Migrations
{
    public partial class progressfractionsublevefields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "progressFraction",
                table: "SubLevels",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "progressFraction",
                table: "SubLevels");
        }
    }
}
