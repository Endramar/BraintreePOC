using Microsoft.EntityFrameworkCore.Migrations;

namespace BraintreePOC.Migrations
{
    public partial class transaction_and_refunds : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BraintreeTransactionId = table.Column<string>(nullable: true),
                    Amount = table.Column<decimal>(type: "DECIMAL(10,2)", nullable: false),
                    WasSuccessfull = table.Column<bool>(nullable: false),
                    IsTotallyRefunded = table.Column<bool>(nullable: false),
                    CustomerId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transactions_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TransactionRefunds",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Amount = table.Column<decimal>(type: "DECIMAL(10,2)", nullable: false),
                    IsPartial = table.Column<bool>(nullable: false),
                    CustomerTransactionId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionRefunds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransactionRefunds_Transactions_CustomerTransactionId",
                        column: x => x.CustomerTransactionId,
                        principalTable: "Transactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TransactionRefunds_CustomerTransactionId",
                table: "TransactionRefunds",
                column: "CustomerTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_CustomerId",
                table: "Transactions",
                column: "CustomerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TransactionRefunds");

            migrationBuilder.DropTable(
                name: "Transactions");
        }
    }
}
