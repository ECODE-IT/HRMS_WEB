using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace HRMS_WEB.Migrations
{
    public partial class addedprojectentity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProjectID",
                table: "SubLevels",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<int>(nullable: false),
                    AssignedDateTime = table.Column<DateTime>(nullable: false),
                    Customer = table.Column<string>(nullable: true),
                    Deadline = table.Column<DateTime>(nullable: false),
                    Remarks = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SubLevels_ProjectID",
                table: "SubLevels",
                column: "ProjectID");

            migrationBuilder.AddForeignKey(
                name: "FK_SubLevels_Projects_ProjectID",
                table: "SubLevels",
                column: "ProjectID",
                principalTable: "Projects",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubLevels_Projects_ProjectID",
                table: "SubLevels");

            migrationBuilder.DropTable(
                name: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_SubLevels_ProjectID",
                table: "SubLevels");

            migrationBuilder.DropColumn(
                name: "ProjectID",
                table: "SubLevels");
        }
    }
}
