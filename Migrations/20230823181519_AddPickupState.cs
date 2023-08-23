using Microsoft.EntityFrameworkCore.Migrations;

namespace QL_HS.Migrations
{
    public partial class AddPickupState : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "Pickups",
                type: "nvarchar(100)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "State",
                table: "Pickups");
        }
    }
}
