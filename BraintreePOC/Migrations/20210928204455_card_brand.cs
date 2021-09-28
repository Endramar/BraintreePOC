using Microsoft.EntityFrameworkCore.Migrations;

namespace BraintreePOC.Migrations
{
    public partial class card_brand : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Brand",
                table: "CreditCards",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Brand",
                table: "CreditCards");
        }
    }
}
