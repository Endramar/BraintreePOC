using Microsoft.EntityFrameworkCore.Migrations;

namespace BraintreePOC.Migrations
{
    public partial class braintree_address_id : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BraintreeId",
                table: "Addresses",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BraintreeId",
                table: "Addresses");
        }
    }
}
