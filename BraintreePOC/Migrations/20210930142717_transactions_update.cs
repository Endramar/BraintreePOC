using Microsoft.EntityFrameworkCore.Migrations;

namespace BraintreePOC.Migrations
{
    public partial class transactions_update : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsTotallyRefunded",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "IsPartial",
                table: "TransactionRefunds");

            migrationBuilder.AddColumn<string>(
                name: "ProcessorResponseCode",
                table: "Transactions",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProcessorResponseText",
                table: "Transactions",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Transactions",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProcessorResponseCode",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "ProcessorResponseText",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Transactions");

            migrationBuilder.AddColumn<bool>(
                name: "IsTotallyRefunded",
                table: "Transactions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPartial",
                table: "TransactionRefunds",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
