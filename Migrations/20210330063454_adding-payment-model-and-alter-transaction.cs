using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WalletQ.Migrations
{
    public partial class addingpaymentmodelandaltertransaction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    creatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    paymentState = table.Column<int>(type: "int", nullable: false),
                    transactionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    validationTime = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payments_Transactions_transactionId",
                        column: x => x.transactionId,
                        principalTable: "Transactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Payments_Users_creatorId",
                        column: x => x.creatorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Payments_creatorId",
                table: "Payments",
                column: "creatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_transactionId",
                table: "Payments",
                column: "transactionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Payments");
        }
    }
}
