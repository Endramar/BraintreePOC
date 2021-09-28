using Microsoft.EntityFrameworkCore.Migrations;

namespace BraintreePOC.Migrations
{
    public partial class credit_card_is_default : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDefault",
                table: "CreditCards",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDefault",
                table: "CreditCards");
        }
    }
}
