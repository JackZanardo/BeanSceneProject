using Microsoft.EntityFrameworkCore.Migrations;

namespace BeanSceneProject.Migrations
{
    public partial class DefaultTimeSittingType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DefaultCloseTime",
                table: "SittingTypes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DefaultOpenTime",
                table: "SittingTypes",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DefaultCloseTime",
                table: "SittingTypes");

            migrationBuilder.DropColumn(
                name: "DefaultOpenTime",
                table: "SittingTypes");
        }
    }
}
